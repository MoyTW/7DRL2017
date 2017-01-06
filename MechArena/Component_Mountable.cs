using System;

namespace MechArena
{
    [Serializable()]
    class Component_Mountable : Component
    {
        public MountSize SizeRequired { get; }

        public Component_Mountable(MountSize sizeRequired)
        {
            this.SizeRequired = sizeRequired;
        }
    }
}
