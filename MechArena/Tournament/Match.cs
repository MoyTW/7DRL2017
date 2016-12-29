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
            this.Competitor1 = competitor1;
            this.Competitor2 = competitor2;
            this.IsTieBreaker = isTieBreaker;
        }

        public bool HasCompetitor(Competitor c)
        {
            return this.Competitor1 == c || this.Competitor2 == c;
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
