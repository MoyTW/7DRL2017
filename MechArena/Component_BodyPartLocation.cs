using System;

namespace MechArena
{
    [Serializable()]
    class Component_BodyPartLocation : Component
    {
        public BodyPartLocation Location { get; }

        public Component_BodyPartLocation(BodyPartLocation location)
        {
            this.Location = location;
        }
    }
}
