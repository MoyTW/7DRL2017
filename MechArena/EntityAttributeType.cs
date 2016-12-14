using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MechArena
{
    public enum EntityAttributeType
    {
        DESTROYED = 0,
        STRUCTURE,

        // Mech Attribute Types
        DODGE,

        // Weapon Attribute Types
        TO_HIT,
        DAMAGE,
        MAX_RANGE,
        REFIRE_TICKS
    }
}
