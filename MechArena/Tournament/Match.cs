using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MechArena.Tournament
{
    public class Match
    {
        public ICompetitor Competitor1 { get; }
        public ICompetitor Competitor2 { get; }
        public bool IsTieBreaker { get; }
        public string MapID { get; }

        public Match(ICompetitor competitor1, ICompetitor competitor2, string mapID, bool isTieBreaker=false)
        {
            // We don't want the competitors to get changed - for example, if the player later swaps out his mech, we
            // want to ensure that when we replay the Match he still has the original mech for the history. I don't
            // think there will be memory issues, but we'll see!
            this.Competitor1 = competitor1.DeepCopy();
            this.Competitor2 = competitor2.DeepCopy();
            this.MapID = mapID;
            this.IsTieBreaker = isTieBreaker;
        }

        public ICompetitor CompetitorByID(string id)
        {
            if (this.Competitor1.CompetitorID == id)
                return this.Competitor1;
            else if (this.Competitor2.CompetitorID == id)
                return this.Competitor2;
            else
                return null;
        }

        public ICompetitor OpponentOf(string competitorID)
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

        public MatchResult BuildResult(ICompetitor winner, int mapSeed, int arenaSeed)
        {
            Console.WriteLine("Winner " + winner + " map " + mapSeed + " arena " + arenaSeed);
            return new MatchResult(this, winner, mapSeed, arenaSeed);
        }

        public MatchResult BuildResult(string winnerID, int mapSeed, int arenaSeed)
        {
            return this.BuildResult(this.CompetitorByID(winnerID), mapSeed, arenaSeed);
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
