using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Executor
{
    public class GameEvent_Unslot : GameEvent_Command
    {
        public Entity EntityToUnslot { get; }

        // TODO: Unslot's currently "free" so the tick's always 0!
        public GameEvent_Unslot(Entity topContainer, Entity subContainer, Entity entityToUnslot, int commandTick=0)
            : base(commandTick, Config.ZERO, topContainer, subContainer)
        {
            if (!subContainer.HasComponentOfType<Component_SlottedContainer>() &&
                !subContainer.HasComponentOfType<Component_AttachPoint>())
                throw new ArgumentException("Can't slot to item without slots!");
            if (!entityToUnslot.HasComponentOfType<Component_Slottable>() &&
                !entityToUnslot.HasComponentOfType<Component_Attachable>())
                throw new ArgumentException("Can't slot unslottable item!");

            this.EntityToUnslot = entityToUnslot;
        }
    }
}
