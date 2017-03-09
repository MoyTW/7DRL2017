using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Executor
{
    public class GameEvent_Slot : GameEvent_Command
    {
        public Entity EntityToSlot { get; }

        public override bool ShouldLog { get { return true; } }
        protected override string _LogMessage
        {
            get
            {
                return string.Format("{0} equipped {1} on {2}", this.CommandEntity.Label, this.EntityToSlot.Label,
                    this.ExecutorEntity.Label);
            }
        }

        // TODO: Slot's currently "free" so the tick is always 0!
        public GameEvent_Slot(Entity topContainer, Entity subContainer, Entity entityToSlot, int commandTick=0)
            : base(commandTick, Config.ZERO, topContainer, subContainer)
        {
            if (!subContainer.HasComponentOfType<Component_SlottedContainer>() &&
                !subContainer.HasComponentOfType<Component_AttachPoint>())
            {
                throw new ArgumentException("Can't slot to item without slots!");
            }
            if (!entityToSlot.HasComponentOfType<Component_Slottable>() &&
                !entityToSlot.HasComponentOfType<Component_Attachable>())
            {
                throw new ArgumentException("Can't slot unslottable item!");
            }

            this.EntityToSlot = entityToSlot;
        }
    }
}
