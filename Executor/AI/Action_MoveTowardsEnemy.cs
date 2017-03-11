using System;

namespace Executor.AI
{
    [Serializable()]
    class Action_MoveTowardsEnemy : AIAction
    {
        [NonSerialized()]
        private RogueSharp.Path lastPath;

        public override bool CanExecuteOn(GameQuery_Command commandQuery)
        {
            return commandQuery.CommandEntity.HasComponentOfType<Component_Skeleton>();
        }

        private static RogueSharp.Path GeneratePath(GameQuery_Command commandQuery, GameQuery_Position targetPos)
        {
            // TODO: Wow this is awkward!?
            var commandPos = commandQuery.CommandEntity.TryGetPosition();
            var commandCell = commandQuery.ArenaState.ArenaMap.GetCell(commandPos.X, commandPos.Y);
            var targetCell = commandQuery.ArenaState.ArenaMap.GetCell(targetPos.X, targetPos.Y);

            return commandQuery.ArenaState.ShortestPath(commandCell, targetCell);
        }

        private CommandStub MoveEventForPath(GameQuery_Command commandQuery, RogueSharp.Path path)
        {
            var commandPos = commandQuery.CommandEntity.TryGetPosition();
            var nextCell = path.CurrentStep;

            if (path.CurrentStep != path.End)
            {
                path.StepForward();
            }

            return new CommandStub_MoveSingle(commandQuery.CommandEntity.EntityID, nextCell.X - commandPos.X,
                nextCell.Y - commandPos.Y);
        }

        private bool EnemyOnPath(GameQuery_Position targetPos)
        {
            bool future = false;
            foreach (var step in this.lastPath.Steps)
            {
                if (step == this.lastPath.CurrentStep)
                    future = true;

                if (future && step.X == targetPos.X && step.Y == targetPos.Y)
                    return true;
            }
            return false;
        }

        public override CommandStub GenerateCommand(GameQuery_Command commandQuery)
        {
            Entity target = commandQuery.ArenaState.Player;
            var targetPos = target.TryGetPosition();

            if (this.lastPath == null || !this.EnemyOnPath(targetPos))
                this.lastPath = Action_MoveTowardsEnemy.GeneratePath(commandQuery, targetPos);
            return this.MoveEventForPath(commandQuery, this.lastPath);
        }
    }
}

