using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MechArena
{
    class Component_BodyPartLocation : Component
    {
        public BodyPartLocation Location { get; }

        public Component_BodyPartLocation(BodyPartLocation location)
        {
            this.Location = location;
        }
    }
}
