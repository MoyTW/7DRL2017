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
            this.EntityToUnslot = entityToUnslot;
            this.EntityContainer = container;
        }
    }
}
