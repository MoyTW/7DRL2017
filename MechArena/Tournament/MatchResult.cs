using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MechArena.Tournament
{
    public class MatchResult
    {
        public Match OriginalMatch { get; }
        public Competitor Winner { get; }

        public MatchResult(Match originalMatch, Competitor winner)
        {
            this.OriginalMatch = originalMatch;
            this.Winner = winner;
        }
    }
}
