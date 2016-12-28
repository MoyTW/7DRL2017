using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MechArena.Tournament
{
    class Competitor
    {
        public string PilotLabel { get; }
        public string MechLabel { get; }
        public string Label { get { return this.PilotLabel + " (" + this.MechLabel + ")"; } }

        public Competitor(string pilotLabel, string mechLabel)
        {
            this.PilotLabel = pilotLabel;
            this.MechLabel = mechLabel;
        }
    }
}
