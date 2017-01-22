using System;
using System.Collections.Generic;

namespace MechArena
{
    [Serializable()]
    public class Component_Slottable : Component
    {
        private int slotsRequired;
        public int SlotsRequired { get { return this.slotsRequired; } }

        public Component_Slottable(int slotsRequired)
        {
            this.slotsRequired = slotsRequired;
        }

        protected override ISet<SubEntitiesSelector> _MatchingSelectors()
        {
            return new HashSet<SubEntitiesSelector>();
        }
    }
}
