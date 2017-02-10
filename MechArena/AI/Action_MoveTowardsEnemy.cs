using System;

namespace MechArena.AI
{
    class Action_MoveTowardsEnemy : Action
    {
        public override bool CanExecuteOn(GameQuery_Command commandQuery)
        {
            return commandQuery.ExecutorEntity.HasComponentOfType<Component_MechSkeleton>();
        }

        public override GameEvent_Command GenerateCommand(GameQuery_Command commandQuery)
        {
            Entity target;
            if (commandQuery.CommandEntity == commandQuery.ArenaState.Mech1)
                target = commandQuery.ArenaState.Mech2;
            else
                target = commandQuery.ArenaState.Mech1;

            // TODO: Wow this is awkward!?
            var commandPos = commandQuery.CommandEntity.TryGetPosition();
            var commandCell = commandQuery.ArenaState.ArenaMap.GetCell(commandPos.X, commandPos.Y);
            var targetPos = target.TryGetPosition();
            var targetCell = commandQuery.ArenaState.ArenaMap.GetCell(targetPos.X, targetPos.Y);

            var shortestPath = commandQuery.ArenaState.ArenaPathFinder.ShortestPath(commandCell, targetCell);
            var nextCell = shortestPath.CurrentStep;

            return new GameEvent_MoveSingle(commandQuery.CommandEntity, commandQuery.ArenaState.CurrentTick,
                nextCell.X - commandPos.X, nextCell.Y - commandPos.Y, commandQuery.ArenaState);
        }
    }
}

