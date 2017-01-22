using System;
using System.Collections.Generic;

namespace MechArena
{
    [Serializable()]
    class Component_Player : Component
    {
        protected override ISet<SubEntitiesSelector> _MatchingSelectors()
        {
            return new HashSet<SubEntitiesSelector>();
        }
    }
}
