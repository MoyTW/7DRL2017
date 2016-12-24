using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MechArena
{
    public class GameEvent_Slot : GameEvent_Command
    {
        public Entity EntityToSlot { get; }

        public GameEvent_Slot(Entity topContainer, Entity subContainer, Entity entityToSlot)
            : base(topContainer, subContainer)
        {
            if (!subContainer.HasComponentOfType<Component_SlottedContainer>())
                throw new ArgumentException("Can't slot to item without slots!");
            if (!entityToSlot.HasComponentOfType<Component_Slottable>())
                throw new ArgumentException("Can't slot unslottable item!");

            this.EntityToSlot = entityToSlot;
        }
    }
}
