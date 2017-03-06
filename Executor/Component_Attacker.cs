using System;
using System.Collections.Immutable;
using System.Linq;

namespace Executor
{
    [Serializable()]
    class Component_Attacker : Component
    {
        protected override IImmutableSet<SubEntitiesSelector> _MatchingSelectors()
        {
            return ImmutableHashSet<SubEntitiesSelector>.Empty;
        }

        private void HandleAttack(GameEvent_PrepareAttack ev)
        {
            if (ev.ExecutorEntity == this.Parent)
            {
                // Get all intervening modifiers (Inspect map for LOS & Terrain Bonuses)
                var targetPos = ev.Target.TryGetPosition();
                var attackerPos = ev.CommandEntity.TryGetPosition();
                var lineCells = ev.GameMap.GetCellsAlongLine(attackerPos.X, attackerPos.Y, targetPos.X, targetPos.Y);

                // If it's out of range, then the attack misses
                int weaponRange = ev.ExecutorEntity.TryGetAttribute(EntityAttributeType.MAX_RANGE, ev.ExecutorEntity)
                    .Value;
                var distance = lineCells.Count() - 1;
                if (distance > weaponRange)
                {
                    Log.DebugLine("Attack missed due to range! Distance: " + distance + " Range: " + weaponRange);
                    ev.Completed = true;
                    return;
                }

                // TODO: This sequence of early exists is crufty because there's a lot of "before-exit-do" stuff here!
                // If any of the cells isn't walkable, then your shot is blocked and the attack stops
                if (lineCells.Any(c => !c.IsWalkable))
                {
                    Log.DebugLine("Attack missed due to intervening terrain!");
                    ev.Completed = true;
                    return;
                }

                // Forward the attack to the target
                var receiveAttack = new GameEvent_ReceiveAttack(ev);
                ev.Target.HandleEvent(receiveAttack);
                if (receiveAttack.Completed)
                    ev.Completed = true;
                else
                    Log.DebugLine("Could not resolve attack against " + ev.Target.ToString());
            }
        }

        protected override GameEvent _HandleEvent(GameEvent ev)
        {
            if (ev is GameEvent_PrepareAttack)
                this.HandleAttack((GameEvent_PrepareAttack)ev);

            return ev;
        }
    }
}
