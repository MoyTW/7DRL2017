using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MechArena.Tournament
{
    class Schedule
    {
        private List<Competitor> allCompetitors;

        private Schedule(List<Competitor> allCompetitors)
        {
            this.allCompetitors = allCompetitors;
        }

        public static Schedule ScheduleTournament(IEnumerable<Competitor> competitors)
        {
            throw new NotImplementedException();
        }

        public void ReportResult(MatchResult result)
        {
            throw new NotImplementedException();
        }

        public Match NextMatch()
        {
            throw new NotImplementedException();
        }
    }
}
