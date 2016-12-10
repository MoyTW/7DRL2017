using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MechArena
{
    public class GameEvent_Unslot : GameEvent
    {
        public Entity EntityToUnslot { get; }
        public Entity EntityContainer { get; }

        public GameEvent_Unslot(Entity entityToUnslot, Entity container)
        {
            if (!container.HasComponentOfType<Component_SlottedContainer>())
                throw new ArgumentException("Can't slot to item without slots!");
            if (!entityToUnslot.HasComponentOfType<Component_Slottable>())
                throw new ArgumentException("Can't slot unslottable item!");

            this.EntityToUnslot = entityToUnslot;
            this.EntityContainer = container;
        }
    }
}
