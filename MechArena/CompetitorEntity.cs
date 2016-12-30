using MechArena.Tournament;

using System;

namespace MechArena
{
    public class CompetitorEntity : Competitor
    {
        public Entity Pilot { get; }
        public Entity Mech { get; }

        public string PilotLabel { get { return this.Pilot.Label; } }
        public string MechLabel { get { return this.Mech.Label; } }
        public string Label { get { return this.PilotLabel + " (" + this.MechLabel + ")"; } }
        public string CompetitorID { get { return this.Mech.EntityID; } }

        public CompetitorEntity(Entity pilot, Entity mech)
        {
            this.Pilot = pilot;
            this.Mech = mech;
        }

        public Competitor DeepCopy()
        {
            return new CompetitorEntity(this.Pilot.DeepCopy(), this.Mech.DeepCopy());
        }

        public override string ToString()
        {
            return this.Label;
        }
    }
}
