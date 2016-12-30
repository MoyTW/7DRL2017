using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MechArena.Tournament
{
    public interface Competitor
    {
        string PilotLabel { get; }
        string MechLabel { get; }
        string Label { get; }
        string CompetitorID { get; }

        Competitor DeepCopy();
    }
}
