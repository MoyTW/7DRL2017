using System;
using System.Collections.Immutable;

namespace Executor
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

        protected override IImmutableSet<SubEntitiesSelector> _MatchingSelectors()
        {
            return ImmutableHashSet<SubEntitiesSelector>.Empty.Add(SubEntitiesSelector.SLOTTABLE);
        }
    }
}
