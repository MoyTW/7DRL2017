using RogueSharp.Random;
using System;

namespace Executor
{
    public class GameQuery_Command : GameQuery
    {
        private GameEvent_Command command;

        public Entity CommandEntity { get; }
        public ArenaState ArenaState { get; }
        public IRandom Rand { get { return this.ArenaState.SeededRand; } }

        public GameEvent_Command Command { get { return this.command; } }

        public GameQuery_Command(Entity commandEntity, ArenaState arenaState)
        {
            this.CommandEntity = commandEntity;
            this.ArenaState = arenaState;
        }

        public void RegisterCommand(GameEvent_Command command)
        {
            if (this.command != null)
                throw new InvalidOperationException("Can't double-register commands!");

            this.command = command;
            this.Completed = true;
        }
    }
}
