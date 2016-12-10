using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MechArena
{
    public class GameEvent_Slot : GameEvent
    {
        public Entity EntityToSlot { get; }
        public Entity EntityContainer { get; }

        public GameEvent_Slot(Entity entityToSlot, Entity container)
        {
            if (!container.HasComponentOfType<Component_SlottedContainer>())
                throw new ArgumentException("Can't slot to item without slots!");
            if (!entityToSlot.HasComponentOfType<Component_Slottable>())
                throw new ArgumentException("Can't slot unslottable item!");

            this.EntityToSlot = entityToSlot;
            this.EntityContainer = container;
        }
    }
}
