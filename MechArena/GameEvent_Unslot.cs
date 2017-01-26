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
            if (!subContainer.HasComponentOfType<Component_SlottedContainer>() &&
                !subContainer.HasComponentOfType<Component_Mount>() &&
                !subContainer.HasComponentOfType<Component_Holster>())
                throw new ArgumentException("Can't slot to item without slots!");
            if (!entityToUnslot.HasComponentOfType<Component_Slottable>() &&
                !entityToUnslot.HasComponentOfType<Component_Mountable>())
                throw new ArgumentException("Can't slot unslottable item!");

            this.EntityToUnslot = entityToUnslot;
        }
    }
}
