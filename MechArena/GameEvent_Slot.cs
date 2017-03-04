using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Executor
{
    public class GameEvent_Slot : GameEvent_Command
    {
        public Entity EntityToSlot { get; }

        public GameEvent_Slot(Entity topContainer, Entity subContainer, Entity entityToSlot)
            : base(topContainer, subContainer)
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
