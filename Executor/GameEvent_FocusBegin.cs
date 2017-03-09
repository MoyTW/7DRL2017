using System;

namespace Executor
{
    public class CommandStub_FocusBegin : CommandStub
    {
        public CommandStub_FocusBegin(string commandEID) : base(commandEID) { }

        public override GameEvent_Command ReifyStub(ArenaState arena)
        {
            return new GameEvent_FocusBegin(arena.CurrentTick, arena.ResolveEID(this.CommandEID));
        }

        public override string ToString()
        {
            return string.Format("Begin Focus");
        }
    }

    public class GameEvent_FocusBegin : GameEvent_Command
    {
        public override bool ShouldLog { get { return true; } }
        protected override string _LogMessage
        {
            get
            {
                return string.Format("{0}'s focus has started", this.CommandEntity.Label);
            }
        }

        public GameEvent_FocusBegin(int commandTick, Entity commandEntity)
            : base(commandTick, Config.ONE, commandEntity)
        {
        }
    }
}

