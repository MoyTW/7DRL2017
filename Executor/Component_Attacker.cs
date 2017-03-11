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
                // If it's out of range, then the attack misses
                int weaponRange = ev.ExecutorEntity.TryGetAttribute(EntityAttributeType.MAX_RANGE, ev.ExecutorEntity)
                    .Value;
                int distance = ArenaState.DistanceBetweenEntities(ev.Target, ev.CommandEntity);
                if (distance > weaponRange)
                {
                    ev.RegisterResult("MISS (out of range)");
                    return;
                }

                // TODO: This sequence of early exists is crufty because there's a lot of "before-exit-do" stuff here!
                // If any of the cells isn't walkable, then your shot is blocked and the attack stops
                var targetPos = ev.Target.TryGetPosition();
                var attackerPos = ev.CommandEntity.TryGetPosition();
                var lineCells = ev.GameMap.GetCellsAlongLine(attackerPos.X, attackerPos.Y, targetPos.X, targetPos.Y);

                if (lineCells.Any(c => !c.IsWalkable))
                {
                    ev.RegisterResult("MISS (no straight shot)");
                    return;
                }

                // Forward the attack to the target
                var receiveAttack = new GameEvent_ReceiveAttack(ev);
                ev.Target.HandleEvent(receiveAttack);
                if (!receiveAttack.Completed)
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
