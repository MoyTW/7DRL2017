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
            this.EntityToSlot = entityToSlot;
            this.EntityContainer = container;
        }
    }
}
