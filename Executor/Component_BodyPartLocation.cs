using System;
using System.Collections.Immutable;

namespace Executor
{
    [Serializable()]
    class Component_BodyPartLocation : Component
    {
        public BodyPartLocation Location { get; }

        public Component_BodyPartLocation(BodyPartLocation location)
        {
            this.Location = location;
        }

        protected override IImmutableSet<SubEntitiesSelector> _MatchingSelectors()
        {
            return ImmutableHashSet<SubEntitiesSelector>.Empty.Add(SubEntitiesSelector.BODY_PART);
        }
    }
}
