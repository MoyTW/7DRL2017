using RogueSharp;
using System;
using System.Linq;

namespace MechArena
{
    [Serializable()]
    class Component_AI : Component
    {
        private void RegisterPathTowards(GameQuery_Command q, Entity target)
        {
            // TODO: Wow this is awkward!?
            var commandPos = q.CommandEntity.TryGetPosition();
            var commandCell = q.ArenaState.ArenaMap.GetCell(commandPos.X, commandPos.Y);
            var targetPos = target.TryGetPosition();
            var targetCell = q.ArenaState.ArenaMap.GetCell(targetPos.X, targetPos.Y);

            var shortestPath = q.ArenaState.ArenaPathFinder.ShortestPath(commandCell, targetCell);
            var nextCell = shortestPath.CurrentStep;

            var moveCommand = new GameEvent_MoveSingle(q.CommandEntity, q.ArenaState.CurrentTick,
                nextCell.X - commandPos.X, nextCell.Y - commandPos.Y, q.ArenaState);
            q.RegisterCommand(moveCommand);
        }

        private bool IsInRange(GameQuery_Command q, Entity target)
        {
            // Get all intervening modifiers (Inspect map for LOS & Terrain Bonuses)
            var targetPos = target.TryGetPosition();
            var attackerPos = q.CommandEntity.TryGetPosition();
            var lineCells = q.ArenaState.ArenaMap.GetCellsAlongLine(attackerPos.X, attackerPos.Y, targetPos.X,
                targetPos.Y);

            // If it's out of range, then the attack misses
            int weaponRange = q.ExecutorEntity.TryGetAttribute(EntityAttributeType.MAX_RANGE, q.ExecutorEntity)
                .Value;
            var distance = lineCells.Count() - 1;
            return weaponRange >= distance;
        }

        private void RegisterWeaponCommand(GameQuery_Command q, Entity target)
        {
            if (this.IsInRange(q, target))
            {
                var attackCommand = new GameEvent_Attack(q.ArenaState.CurrentTick, q.CommandEntity, target,
                    q.ExecutorEntity, q.ArenaState.ArenaMap, q.Rand);
                q.RegisterCommand(attackCommand);
            }
            else
            {
                var delayCommand = new GameEvent_Delay(q.ArenaState.CurrentTick, q.CommandEntity, q.ExecutorEntity,
                    DelayDuration.NEXT_ACTION);
                q.RegisterCommand(delayCommand);
            }
        }

        private void HandleQueryCommand(GameQuery_Command q)
        {
            Entity target;
            if (q.CommandEntity == q.ArenaState.Mech1)
                target = q.ArenaState.Mech2;
            else
                target = q.ArenaState.Mech1;

            if (q.ExecutorEntity.HasComponentOfType<Component_MechSkeleton>())
                this.RegisterPathTowards(q, target);
            else if (q.ExecutorEntity.HasComponentOfType<Component_Weapon>())
                this.RegisterWeaponCommand(q, target);
            else
                throw new ArgumentException("Entity " + q.ExecutorEntity + " is not Mech, Weapon! Cannot command!");
        }

        protected override GameQuery _HandleQuery(GameQuery q)
        {
            if (q is GameQuery_Command)
                this.HandleQueryCommand((GameQuery_Command)q);

            return q;

        }
    }
}
