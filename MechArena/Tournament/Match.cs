using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MechArena.Tournament
{
    class Match
    {
        public Competitor Competitor1 { get; }
        public Competitor Competitor2 { get; }

        public Match(Competitor competitor1, Competitor competitor2)
        {
            this.Competitor1 = competitor1;
            this.Competitor2 = competitor2;
        }
    }
}
