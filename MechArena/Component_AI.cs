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

        private void RegisterAttack(GameQuery_Command q, Entity target)
        {
            // TODO: Ability to query for LOS/Distance?
            var attackCommand = new GameEvent_Attack(q.ArenaState.CurrentTick, q.CommandEntity, target,
                q.ExecutorEntity, q.ArenaState.ArenaMap, q.Rand);
            q.RegisterCommand(attackCommand);
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
                this.RegisterAttack(q, target);
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
