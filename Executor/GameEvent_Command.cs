using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Executor
{
    public class GameEvent_Command : GameEvent
    {
        public int CommandTick { get; }
        public Entity CommandEntity { get; }
        public Entity ExecutorEntity { get; }

        public GameEvent_Command(int commandTick, Entity commandEntity)
            : this(commandTick, commandEntity, commandEntity) { }

        public GameEvent_Command(int commandTick, Entity commandEntity, Entity executorEntity)
        {
            if (CommandEntity != ExecutorEntity &&
                !commandEntity.TryGetSubEntities(SubEntitiesSelector.ALL).Contains(executorEntity))
            {
                throw new ArgumentException("Cannot construct command, commandEntity " + commandEntity +
                    " doesn't contain executorEntity " + executorEntity);
            }

            this.CommandTick = commandTick;
            this.CommandEntity = commandEntity;
            this.ExecutorEntity = executorEntity;
        }
    }
}
