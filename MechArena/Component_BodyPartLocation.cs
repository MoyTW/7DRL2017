using System;
using System.Collections.Generic;

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

        protected override ISet<SubEntitiesSelector> _MatchingSelectors()
        {
            return new HashSet<SubEntitiesSelector>() { SubEntitiesSelector.BODY_PART };
        }

    }
}
