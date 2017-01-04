using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MechArena.Tournament
{
    public class MatchResult
    {
        public string MatchID { get { return this.OriginalMatch.MatchID; } }
        public ICompetitor Competitor1 { get { return this.OriginalMatch.Competitor1; } }
        public ICompetitor Competitor2 { get { return this.OriginalMatch.Competitor2; } }
        public bool IsTieBreaker { get { return this.OriginalMatch.IsTieBreaker; } }

        public Match OriginalMatch { get; }
        public ICompetitor Winner { get; }
        public string MapID { get; }
        public int ArenaSeed { get; }

        public MatchResult(Match originalMatch, ICompetitor winner, string mapID, int arenaSeed)
        {
            this.OriginalMatch = originalMatch;
            this.Winner = winner;
            this.MapID = mapID;
            this.ArenaSeed = arenaSeed;
        }

        public bool HasCompetitor(string competitorID)
        {
            return this.OriginalMatch.HasCompetitor(competitorID);
        }

        public ICompetitor OpponentOf(string competitorID)
        {
            return this.OriginalMatch.OpponentOf(competitorID);
        }
    }
}
