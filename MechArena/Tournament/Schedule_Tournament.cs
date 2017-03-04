using RogueSharp.Random;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Executor.Tournament
{
    public class Schedule_Tournament : ISchedule
    {
        private const int expectedNumEntreants = 256;
        private const int groupStageSize = 8;
        private const int numFirstStageWinners = 1;
        private const int numSecondStageWinners = 2;

        private IEnumerable<ICompetitor> entreants;
        private IRandom seeder;
        private IMapPicker picker;
        private List<ISchedule> rounds;

        public Schedule_Tournament(IEnumerable<ICompetitor> entreants, IRandom seeder, IMapPicker picker)
        {
            if (entreants.Count() != expectedNumEntreants)
                throw new ArgumentException("Wrong number of entreants into tournament! Should be 256!");

            this.entreants = entreants;
            this.seeder = seeder;
            this.picker = picker;

            this.rounds = new List<ISchedule>();
            rounds.Add(new Schedule_GroupStage(groupStageSize, numFirstStageWinners, entreants, picker));
        }

        #region ISchedule Fns

        public Match FindMatch(string matchID)
        {
            return rounds.Select(r => r.FindMatch(matchID)).Where(m => m != null).FirstOrDefault();
        }

        public Match NextMatch()
        {
            return rounds.Last().NextMatch();
        }

        public void ReportResult(MatchResult result)
        {
            if (this.FindMatchResult(result.MatchID) == null)
            {
                rounds.Last().ReportResult(result);
                if (rounds.Last().NextMatch() == null)
                {
                    // Stage 2 (4 groups of 8, 2 winners each)
                    if (rounds.Count == 1)
                    {
                        Log.DebugLine("Progressing to round 2!");
                        rounds.Add(new Schedule_GroupStage(8, 2, this.Winners(), this.picker));
                    }
                    // Stage 3 (1 group of 8, 1 winner)
                    else if (rounds.Count == 2)
                    {
                        Log.DebugLine("Progressing to round 3!");
                        rounds.Add(new Schedule_RoundRobin(1, this.Winners(), this.picker));
                    }
                }
                Log.DebugLine("Match result " + result + " reported!");
            }
            else
            {
                Log.DebugLine("Attempting to report already-reported match result " + result);
            }
        }

        public IList<ICompetitor> Winners()
        {
            return rounds.Last().Winners();
        }

        public bool IsEliminated(string competitorID)
        {
            return rounds.Any(r => r.IsEliminated(competitorID));
        }

        public IList<Match> ScheduledMatches()
        {
            return rounds.Last().ScheduledMatches();
        }

        public IList<Match> ScheduledMatches(string competitorID)
        {
            return rounds.Last().ScheduledMatches(competitorID);
        }

        // Earliest first
        public IList<MatchResult> MatchHistory(string competitorID)
        {
            var results = new List<MatchResult>();
            foreach (var round in this.rounds)
            {
                results.AddRange(round.MatchHistory(competitorID));
            }
            return results.AsReadOnly();
        }

        #endregion

        public int GenArenaSeed()
        {
            return this.seeder.Next(Int16.MaxValue);
        }

        public string PickMapID()
        {
            return this.picker.PickMapID();
        }

        public IList<ICompetitor> AllCompetitors()
        {
            return this.entreants.ToList().AsReadOnly();
        }

        public int RoundNum()
        {
            return this.rounds.Count;
        }

        public IList<ICompetitor> Winners(int round)
        {
            return rounds[round - 1].Winners();
        }

        public void ReportResult(string matchID, string winnerID, string mapID, int arenaSeed)
        {
            var result = this.FindMatch(matchID).BuildResult(winnerID, mapID, arenaSeed);
            this.ReportResult(result);
        }

        public MatchResult FindMatchResult(string matchID)
        {
            // This implementation is somewhat dubious!
            var match = this.FindMatch(matchID);
            return this.MatchHistory(match.Competitor1.CompetitorID).Where(r => r.MatchID == matchID).FirstOrDefault();
        }
    }
}
