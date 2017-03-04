using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Executor
{
    public enum EntityAttributeType
    {
        DESTROYED = 0,
        STRUCTURE,

        // Mech Attribute Types
        SPEED,

        // Weapon Attribute Types
        DAMAGE,
        MAX_RANGE,
        REFIRE_TICKS
    }
}
