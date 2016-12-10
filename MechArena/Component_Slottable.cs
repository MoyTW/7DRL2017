using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MechArena
{
    public class Component_Slottable : Component
    {
        private int slotsRequired;
        public int SlotsRequired { get { return this.slotsRequired; } }

        public Component_Slottable(int slotsRequired)
        {
            this.slotsRequired = slotsRequired;
        }
    }
}
