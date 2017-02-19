using System;
using System.Collections.Immutable;

namespace MechArena
{
    [Serializable()]
    public class Component_Piloted : Component
    {
        public Entity Pilot { get; }

        public Component_Piloted() { }

        public Component_Piloted(Entity pilot)
        {
            this.Pilot = pilot;
        }

        protected override IImmutableSet<SubEntitiesSelector> _MatchingSelectors()
        {
            return ImmutableHashSet<SubEntitiesSelector>.Empty;
        }

        protected override GameQuery _HandleQuery(GameQuery q)
        {
            if (this.Pilot == null)
                throw new InvalidOperationException("Can't query piloted " + this.Parent + " with no Pilot assigned!");

            return Pilot.HandleQuery(q);
        }
    }
}

