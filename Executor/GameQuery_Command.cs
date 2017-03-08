using RogueSharp.Random;
using System;

namespace Executor
{
    public class GameQuery_Command : GameQuery
    {
        public Entity CommandEntity { get; }
        public ArenaState ArenaState { get; }

        public CommandStub Command { get; private set; }

        public GameQuery_Command(Entity commandEntity, ArenaState arenaState)
        {
            this.CommandEntity = commandEntity;
            this.ArenaState = arenaState;
        }

        public void RegisterCommand(CommandStub stub)
        {
            if (this.Command != null)
                throw new InvalidOperationException("Can't double-register commands!");

            this.Command = stub;
            this.Completed = true;
        }
    }
}
