using System;
using System.Linq;

namespace Executor
{
    class GameEvent_Activation : GameEvent
    {
        public int CommandTick { get; }
        public int APCost { get; }
        public Entity CommandEntity { get; }
        public Entity ExecutorEntity { get; }

        public GameEvent_Activation(GameEvent_Command ev)
        {
            if (ev.CommandEntity != ev.ExecutorEntity &&
                !ev.CommandEntity.TryGetSubEntities(SubEntitiesSelector.ALL).Contains(ev.ExecutorEntity))
            {
                throw new ArgumentException("Cannot construct activation notification, commandEntity "
                    + ev.CommandEntity + " doesn't contain executorEntity " + ev.ExecutorEntity);
            }

            this.CommandTick = ev.CommandTick;
            this.APCost = ev.APCost;
            this.CommandEntity = ev.CommandEntity;
            this.ExecutorEntity = ev.ExecutorEntity;
        }
    }
}
