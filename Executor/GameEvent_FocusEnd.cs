using System;

namespace Executor
{
    public class CommandStub_FocusEnd : CommandStub
    {
        public CommandStub_FocusEnd(string commandEID) : base(commandEID) { }

        public override GameEvent_Command ReifyStub(ArenaState arena)
        {
            return new GameEvent_FocusEnd(arena.CurrentTick, arena.ResolveEID(this.CommandEID));
        }

        public override string ToString()
        {
            return string.Format("End Focus");
        }
    }

    public class GameEvent_FocusEnd : GameEvent_Command
    {
        public override bool ShouldLog { get { return true; } }
        protected override string _LogMessage
        {
            get
            {
                return string.Format("{0}'s focus has ended", this.CommandEntity.Label);
            }
        }
        
        public GameEvent_FocusEnd(int commandTick, Entity commandEntity)
            : base(commandTick, Config.ZERO, commandEntity)
        {
        }
    }
}

