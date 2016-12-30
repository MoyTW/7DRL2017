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
        public int Seed { get; }

        public MatchResult(Match originalMatch, Competitor winner, int seed)
        {
            this.OriginalMatch = originalMatch;
            this.Winner = winner;
            this.Seed = seed;
        }
    }
}
