using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MechArena
{
    public class GameEvent_Unslot : GameEvent_Command
    {
        public Entity EntityToUnslot { get; }

        public GameEvent_Unslot(Entity topContainer, Entity subContainer, Entity entityToUnslot)
            : base(topContainer, subContainer)
        {
            if (!subContainer.HasComponentOfType<Component_SlottedContainer>())
                throw new ArgumentException("Can't slot to item without slots!");
            if (!entityToUnslot.HasComponentOfType<Component_Slottable>())
                throw new ArgumentException("Can't slot unslottable item!");

            this.EntityToUnslot = entityToUnslot;
        }
    }
}
