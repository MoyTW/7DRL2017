using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MechArena
{
    class Component_Mountable : Component
    {
        public MountSize SizeRequired { get; }

        public Component_Mountable(MountSize sizeRequired)
        {
            this.SizeRequired = sizeRequired;
        }
    }
}
