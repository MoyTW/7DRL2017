using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MechArena.Tournament
{
    public class Schedule_Tournament : Schedule
    {
        private const int expectedNumEntreants = 256;
        private const int groupStageSize = 8;
        private const int numFirstStageWinners = 1;
        private const int numSecondStageWinners = 2;

        private IEnumerable<Competitor> entreants;
        private List<Schedule> rounds;

        public Schedule_Tournament(IEnumerable<Competitor> entreants)
        {
            if (entreants.Count() != expectedNumEntreants)
                throw new ArgumentException("Wrong number of entreants into tournament! Should be 256!");

            this.entreants = entreants;
            this.rounds = new List<Schedule>();
            rounds.Add(new Schedule_GroupStage(groupStageSize, numFirstStageWinners, entreants));
        }

        public int RoundNum()
        {
            return this.rounds.Count;
        }

        public bool IsEliminated(string competitorID)
        {
            return rounds.Last().IsEliminated(competitorID);
        }

        public IList<Competitor> Winners()
        {
            return rounds.Last().Winners();
        }

        public IList<Competitor> Winners(int round)
        {
            return rounds[round - 1].Winners();
        }
        
        public IList<Match> ScheduledMatches()
        {
            return rounds.Last().ScheduledMatches();
        }

        public IList<Match> ScheduledMatches(string competitorID)
        {
            return rounds.Last().ScheduledMatches(competitorID);
        }

        public Match NextMatch()
        {
            return rounds.Last().NextMatch();
        }

        // Earliest first
        public IList<MatchResult> MatchHistory(string competitorID)
        {
            var results = new List<MatchResult>();
            foreach(var round in this.rounds)
            {
                results.AddRange(round.MatchHistory(competitorID));
            }
            return results.AsReadOnly();
        }

        public void ReportResult(MatchResult result)
        {
            rounds.Last().ReportResult(result);
            if (rounds.Last().NextMatch() == null)
            {
                // Stage 2 (4 groups of 8, 2 winners each)
                if (rounds.Count == 1)
                {
                    rounds.Add(new Schedule_GroupStage(8, 2, this.Winners()));
                }
                // Stage 3 (1 group of 8, 1 winner)
                else if (rounds.Count == 2)
                {
                    rounds.Add(new Schedule_RoundRobin(1, this.Winners()));
                }
            }
        }
    }
}
