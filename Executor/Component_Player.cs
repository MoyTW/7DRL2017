using System;
using System.Collections.Immutable;

namespace Executor
{
    [Serializable()]
    class Component_Player : Component
    {
        protected override IImmutableSet<SubEntitiesSelector> _MatchingSelectors()
        {
            return ImmutableHashSet<SubEntitiesSelector>.Empty;
        }
    }
}
