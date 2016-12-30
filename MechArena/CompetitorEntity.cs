using MechArena.Tournament;

using System;

namespace MechArena
{
    class CompetitorEntity : Competitor
    {
        public Entity Pilot { get; }
        public Entity Mech { get; }

        public CompetitorEntity(Entity pilot, Entity mech) : this(pilot, mech, Guid.NewGuid()) { }

        public CompetitorEntity(Entity pilot, Entity mech, Guid competitorID) : base(pilot.Label, mech.Label, competitorID)
        {
            this.Pilot = pilot;
            this.Mech = mech;
        }
    }
}
