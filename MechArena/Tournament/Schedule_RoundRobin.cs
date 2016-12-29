using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MechArena.Tournament
{
    class Schedule_RoundRobin : Schedule
    {
        private HashSet<Competitor> remainingEntreants;
        private HashSet<Competitor> eliminatedEntreants;

        private List<Match> upcomingMatches;
        private List<MatchResult> matchResults;

        // TODO: Possibly rename competitor->entreants because it's easier for me to spell
        public Schedule_RoundRobin(IEnumerable<Competitor> entreants)
        {
            this.remainingEntreants = new HashSet<Competitor>(entreants);
            this.eliminatedEntreants = new HashSet<Competitor>();

            this.upcomingMatches = Scheduler.ScheduleRoundRobin(entreants.ToList());
            this.matchResults = new List<MatchResult>();
        }

        public bool IsEliminated(Competitor c)
        {
            if (this.upcomingMatches.Count > 0)
            {
                Console.WriteLine("Should not call this until it's done!");
                throw new InvalidOperationException("Can't call when matches still running, this is bad desgin!");
            }
            else
            {
                throw new NotImplementedException();
            }
        }

        public Match NextMatch()
        {
            return upcomingMatches.First();
        }

        public List<Competitor> Winners()
        {
            if (this.upcomingMatches.Count > 0)
            {
                Console.WriteLine("Should not call this until it's done!");
                return null;
            }
            {
                throw new NotImplementedException();
            }
        }

        public void ReportResult(MatchResult result)
        {
            if (!this.upcomingMatches.Contains(result.OriginalMatch))
                throw new InvalidOperationException("Cannot report result of non-upcoming match!");

            this.upcomingMatches.Remove(result.OriginalMatch);
            this.matchResults.Add(result);
        }

        public IList<Match> ScheduledMatches(Competitor c)
        {
            return this.upcomingMatches.Where(m => m.HasCompetitor(c)).ToList().AsReadOnly();
        }

        public IList<MatchResult> MatchHistory(Competitor c)
        {
            return this.matchResults.Where(r => r.OriginalMatch.HasCompetitor(c)).ToList().AsReadOnly();
        }
    }
}
