using System;

namespace Executor
{
    public class CommandStub_FocusEnd : CommandStub
    {
        public CommandStub_FocusEnd(Entity commandEntity) : base(commandEntity) { }

        public override GameEvent_Command ReifyStub(ArenaState arena)
        {
            return new GameEvent_FocusEnd(arena.CurrentTick, this.CommandEntity);
        }
    }

    public class GameEvent_FocusEnd : GameEvent_Command
    {
        public GameEvent_FocusEnd(int commandTick, Entity commandEntity)
            : base(commandTick, Config.ZERO, commandEntity)
        {
        }
    }
}

