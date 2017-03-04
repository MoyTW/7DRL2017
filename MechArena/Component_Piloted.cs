using System;
using System.Collections.Immutable;

namespace Executor
{
    [Serializable()]
    public class Component_Piloted : Component
    {
        private Entity pilot;
        public Entity Pilot
        {
            get { return this.pilot; }
            set
            {
                if (this.pilot != null)
                    Log.DebugLine("Swapping pilot " + this.pilot.Label + " of " + this.Parent + " with " + value.Label);
                this.pilot = value;
            }
        }

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

