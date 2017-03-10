using System;
using System.Linq;

namespace Executor
{
    abstract public class CommandStub
    {
        public string CommandEID { get; }

        public CommandStub(string commandEID)
        {
            this.CommandEID = commandEID;
        }

        abstract public GameEvent_Command ReifyStub(ArenaState arena);
    }

    public abstract class GameEvent_Command : GameEvent
    {
        public int CommandTick { get; }
        public int APCost { get; private set; }
        public Entity CommandEntity { get; }
        public Entity ExecutorEntity { get; }
        public abstract bool ShouldLog { get; }
        public String LogMessage
        {
            get
            {
                if (!this.Completed)
                    throw new InvalidOperationException("Can't log an uncompleted event!");
                return this._LogMessage;
            }
        }
        protected abstract String _LogMessage { get; }

        public override bool Completed
        {
            set
            {
                base.Completed = value;
                this.CommandEntity.HandleEvent(new GameEvent_Activation(this));
            }
        }

        public GameEvent_Command(int commandTick, int APCost, Entity commandEntity)
            : this(commandTick, APCost, commandEntity, commandEntity) { }

        public GameEvent_Command(int commandTick, int APCost, Entity commandEntity, Entity executorEntity)
        {
            if (CommandEntity != ExecutorEntity &&
                !commandEntity.TryGetSubEntities(SubEntitiesSelector.ALL).Contains(executorEntity))
            {
                throw new ArgumentException("Cannot construct command, commandEntity " + commandEntity +
                    " doesn't contain executorEntity " + executorEntity);
            }

            this.CommandTick = commandTick;
            this.APCost = APCost;
            this.CommandEntity = commandEntity;
            this.ExecutorEntity = executorEntity;
        }

        public void MakeFreeAction()
        {
            this.APCost = 0;
        }
    }
}
