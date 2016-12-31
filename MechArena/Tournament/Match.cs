using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MechArena.Tournament
{
    public class Match
    {
        public Competitor Competitor1 { get; }
        public Competitor Competitor2 { get; }
        public bool IsTieBreaker { get; }

        public Match(Competitor competitor1, Competitor competitor2, bool isTieBreaker=false)
        {
            // We don't want the competitors to get changed - for example, if the player later swaps out his mech, we
            // want to ensure that when we replay the Match he still has the original mech for the history. I don't
            // think there will be memory issues, but we'll see!
            this.Competitor1 = competitor1.DeepCopy();
            this.Competitor2 = competitor2.DeepCopy();
            this.IsTieBreaker = isTieBreaker;
        }

        public Competitor CompetitorByID(string id)
        {
            if (this.Competitor1.CompetitorID == id)
                return this.Competitor1;
            else if (this.Competitor2.CompetitorID == id)
                return this.Competitor2;
            else
                return null;
        }

        public Competitor OpponentOf(string competitorID)
        {
            if (this.Competitor1.CompetitorID == competitorID)
                return this.Competitor2;
            else
                return this.Competitor1;
        }

        public bool HasCompetitor(string competitorID)
        {
            return this.Competitor1.CompetitorID == competitorID ||
                this.Competitor2.CompetitorID == competitorID;
        }

        public MatchResult BuildResult(Competitor winner, int seed)
        {
            return new MatchResult(this, winner, seed);
        }

        public MatchResult BuildResult(string winnerID, int seed)
        {
            return this.BuildResult(this.CompetitorByID(winnerID), seed);
        }

        public override string ToString()
        {
            if (this.IsTieBreaker)
                return "[" + Competitor1.Label + " : " + Competitor2.Label + "] (TB)";
            else
                return "[" + Competitor1.Label + " : " + Competitor2.Label + "]";
        }
    }
}
