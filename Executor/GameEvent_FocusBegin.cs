using System;

namespace Executor
{
    public class CommandStub_FocusBegin : CommandStub
    {
        public CommandStub_FocusBegin(Entity commandEntity) : base(commandEntity) { }

        public override GameEvent_Command ReifyStub(ArenaState arena)
        {
            return new GameEvent_FocusBegin(arena.CurrentTick, this.CommandEntity);
        }
    }

    public class GameEvent_FocusBegin : GameEvent_Command
    {
        public GameEvent_FocusBegin(int commandTick, Entity commandEntity)
            : base(commandTick, Config.ONE, commandEntity)
        {
        }
    }
}

