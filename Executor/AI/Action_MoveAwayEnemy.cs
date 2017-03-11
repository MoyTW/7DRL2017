using System;
using System.Collections.Generic;

namespace Executor.AI
{
    [Serializable()]
    class Action_MoveAwayEnemy : AIAction
    {
        public override bool CanExecuteOn(GameQuery_Command commandQuery)
        {
            return commandQuery.CommandEntity.HasComponentOfType<Component_Skeleton>() &&
                this.GenerateCommand(commandQuery) != null;
        }

        public override CommandStub GenerateCommand(GameQuery_Command commandQuery)
        {
            Entity target = commandQuery.ArenaState.Player;

            // TODO: Wow this is awkward!?
            var commandPos = commandQuery.CommandEntity.TryGetPosition();
            var commandCell = commandQuery.ArenaState.ArenaMap.GetCell(commandPos.X, commandPos.Y);
            var targetPos = target.TryGetPosition();
            var targetCell = commandQuery.ArenaState.ArenaMap.GetCell(targetPos.X, targetPos.Y);

            var shortestPath = commandQuery.ArenaState.ShortestPath(commandCell, targetCell);
            if (shortestPath == null)
                return null;

            var myDist = shortestPath.Length;
            var currDist = myDist;
            int awayX = 0, awayY = 0;
            // TODO: This is incredibly, absurdly inefficient! However it's sure to be optimal in a one-move timeframe.
            for (int x = commandPos.X - 1; x < commandPos.X + 2; x++)
            {
                for (int y = commandPos.Y - 1; y < commandPos.Y + 2; y++)
                {
                    if (commandQuery.ArenaState.ArenaMap.IsWalkable(x, y) && !(x == targetPos.X && y == targetPos.Y))
                    {
                        var candidateCell = commandQuery.ArenaState.ArenaMap.GetCell(x, y);
                        var path = commandQuery.ArenaState.ShortestPath(candidateCell, targetCell);
                        if (path == null)
                            return null;
                        if (path.Length > currDist)
                        {
                            awayX = x;
                            awayY = y;
                            currDist = path.Length;
                        }
                    }
                }
            }

            if (currDist > myDist)
            {
                return new CommandStub_MoveSingle(commandQuery.CommandEntity.EntityID, awayX - commandPos.X, 
                    awayY - commandPos.Y);
            }
            else
                return null;
        }
    }
}

