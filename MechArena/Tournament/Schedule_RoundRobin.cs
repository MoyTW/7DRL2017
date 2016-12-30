using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MechArena.Tournament
{
    public class Schedule_RoundRobin : Schedule
    {
        public int NumWinners { get; }

        private HashSet<Competitor> remainingEntreants;
        private HashSet<Competitor> winningEntreants;
        private HashSet<Competitor> eliminatedEntreants;

        private List<Match> upcomingMatches;
        private List<MatchResult> matchResults;

        // TODO: Possibly rename competitor->entreants because it's easier for me to spell
        public Schedule_RoundRobin(int numWinners, IEnumerable<Competitor> entreants)
        {
            if (numWinners >= entreants.Count())
                throw new ArgumentException("Num winners is equal to or greater to the number of entreants!");
            else if (numWinners <= 0)
                throw new ArgumentException("Num winners must be a non-zero, positive integer!");

            this.NumWinners = numWinners;

            this.remainingEntreants = new HashSet<Competitor>(entreants);
            this.winningEntreants = new HashSet<Competitor>();
            this.eliminatedEntreants = new HashSet<Competitor>();

            this.upcomingMatches = Scheduler.ScheduleRoundRobin(entreants.ToList());
            this.matchResults = new List<MatchResult>();
        }

        public bool IsEliminated(string competitorID)
        {
            return this.eliminatedEntreants.Where(c => c.CompetitorID == competitorID).Count() != 0;
        }

        public Match NextMatch()
        {
            return this.upcomingMatches.FirstOrDefault();
        }

        public int Wins(string competitorID)
        {
            return MatchHistory(competitorID).Where(m => m.Winner.CompetitorID == competitorID).Count();
        }

        private void ResolveTopScorers()
        {
            if (this.NextMatch() != null)
                throw new InvalidOperationException("Can't resolve scores when not done!");

            int numTopScorers = this.winningEntreants.Count;

            // TODO: This logic is hard to understand from the code!
            var walker = this.remainingEntreants.GroupBy(c => this.Wins(c.CompetitorID)).OrderByDescending(kv => kv.Key).GetEnumerator();
            while (numTopScorers < this.NumWinners && walker.MoveNext())
            {
                Console.WriteLine("Walker current count" + walker.Current.Count());
                numTopScorers += walker.Current.Count();

                if (numTopScorers <= this.NumWinners)
                {
                    foreach (var c in walker.Current)
                    {
                        this.remainingEntreants.Remove(c);
                        this.winningEntreants.Add(c);
                    }
                }
            }
            while (walker.MoveNext())
            {
                foreach (var c in walker.Current)
                {
                    this.remainingEntreants.Remove(c);
                    this.eliminatedEntreants.Add(c);
                }
            }
        }

        public IList<Competitor> Winners()
        {
            if (this.remainingEntreants.Count() != 0)
                throw new InvalidOperationException("Can't call winners before round resolved!");
            else
                return this.winningEntreants.ToList().AsReadOnly();
        }

        public IList<Match> ScheduledMatches()
        {
            return this.upcomingMatches.AsReadOnly();
        }

        public IList<Match> ScheduledMatches(string competitorID)
        {
            return this.upcomingMatches.Where(m => m.HasCompetitor(competitorID)).ToList().AsReadOnly();
        }

        public IList<MatchResult> MatchHistory(string competitorID)
        {
            return this.matchResults.Where(r => r.OriginalMatch.HasCompetitor(competitorID)).ToList().AsReadOnly();
        }

        public void ReportResult(MatchResult result)
        {
            if (!this.upcomingMatches.Contains(result.OriginalMatch))
                throw new InvalidOperationException("Cannot report result of non-upcoming match!");

            this.upcomingMatches.Remove(result.OriginalMatch);
            this.matchResults.Add(result);

            if (this.upcomingMatches.Count == 0)
            {
                // Calculate winners
                this.ResolveTopScorers();
                // If there's a tie, you need to re-generate with your remaining entreants
                if (this.remainingEntreants.Count != 0)
                {
                    Console.WriteLine("Tiebreaker match!");
                    this.upcomingMatches = Scheduler.ScheduleRoundRobin(this.remainingEntreants.ToList(), true);
                }
            }
        }
    }
}
