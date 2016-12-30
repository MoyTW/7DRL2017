using MechArena.Tournament;

using System;

namespace MechArena
{
    public class CompetitorEntity : Competitor
    {
        public Entity Pilot { get; }
        public Entity Mech { get; }

        public CompetitorEntity(Entity pilot, Entity mech) : base(pilot.Label, mech.Label, mech.EntityID)
        {
            this.Pilot = pilot;
            this.Mech = mech;
        }
    }
}
