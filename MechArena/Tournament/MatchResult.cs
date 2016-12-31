using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MechArena.Tournament
{
    public class MatchResult
    {
        public Competitor Competitor1 { get { return this.OriginalMatch.Competitor1; } }
        public Competitor Competitor2 { get { return this.OriginalMatch.Competitor2; } }
        public bool IsTieBreaker { get { return this.OriginalMatch.IsTieBreaker; } }

        public Match OriginalMatch { get; }
        public Competitor Winner { get; }
        public int Seed { get; }

        public MatchResult(Match originalMatch, Competitor winner, int seed)
        {
            this.OriginalMatch = originalMatch;
            this.Winner = winner;
            this.Seed = seed;
        }

        public bool HasCompetitor(string competitorID)
        {
            return this.OriginalMatch.HasCompetitor(competitorID);
        }

        public Competitor OpponentOf(string competitorID)
        {
            return this.OriginalMatch.OpponentOf(competitorID);
        }
    }
}
