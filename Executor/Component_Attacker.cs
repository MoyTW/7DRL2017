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

        private void HandleAttack(GameEvent_Attack ev)
        {
            if (ev.CommandEntity == this.Parent)
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
                    ev.ExecutorEntity.GetComponentOfType<Component_TracksTime>().RegisterActivated(ev.CommandTick);
                    return;
                }

                // TODO: This sequence of early exists is crufty because there's a lot of "before-exit-do" stuff here!
                // If any of the cells isn't walkable, then your shot is blocked and the attack stops
                if (lineCells.Any(c => !c.IsWalkable))
                {
                    Log.DebugLine("Attack missed due to intervening terrain!");
                    ev.Completed = true;
                    ev.ExecutorEntity.GetComponentOfType<Component_TracksTime>().RegisterActivated(ev.CommandTick);
                    return;
                }

                // Forward the attack to the target
                ev.Target.HandleEvent(ev);
                if (!ev.Completed)
                {
                    Log.DebugLine("Could not resolve attack against " + ev.Target.ToString());
                    ev.Completed = true;
                }

                ev.ExecutorEntity.GetComponentOfType<Component_TracksTime>().RegisterActivated(ev.CommandTick);
                ev.CommandEntity.GetComponentOfType<Component_TracksTime>().RegisterActivated(ev.CommandTick);
            }
        }

        protected override GameEvent _HandleEvent(GameEvent ev)
        {
            if (ev is GameEvent_Attack)
                this.HandleAttack((GameEvent_Attack)ev);

            return ev;
        }
    }
}
