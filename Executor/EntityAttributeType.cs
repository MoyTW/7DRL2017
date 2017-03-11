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
        CURRENT_AP,
        MAX_AP,
        SPEED,
        DETECTION_RADIUS,
        SCAN_REQUIRED_RADIUS,

        // Weapon Attribute Types
        DAMAGE,
        MAX_RANGE,
        REFIRE_TICKS
    }
}
