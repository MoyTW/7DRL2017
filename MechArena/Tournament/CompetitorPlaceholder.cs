using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MechArena.Tournament
{
    public class CompetitorPlaceholder : Competitor
    {
        public string PilotLabel { get; }
        public string MechLabel { get; }
        public string Label { get { return this.PilotLabel + " (" + this.MechLabel + ")"; } }
        public string CompetitorID { get; }

        public CompetitorPlaceholder(string pilotLabel, string mechLabel)
            : this(pilotLabel, mechLabel, Guid.NewGuid().ToString())
        { }

        public CompetitorPlaceholder(string pilotLabel, string mechLabel, string competitorID)
        {
            this.PilotLabel = pilotLabel;
            this.MechLabel = mechLabel;
            this.CompetitorID = competitorID;
        }

        public Competitor DeepCopy()
        {
            return new CompetitorPlaceholder(this.PilotLabel, this.MechLabel, this.CompetitorID);
        }

        public override string ToString()
        {
            return this.Label;
        }
    }
}
