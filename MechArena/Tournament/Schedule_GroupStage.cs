using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MechArena.Tournament
{
    public class Schedule_GroupStage : Schedule
    {
        public int GroupSize { get; }
        public int WinnersPerGroup { get; }

        private IEnumerable<Competitor> allEntreants;
        private Dictionary<string, Schedule> entreantsToSchedules;
        private List<Schedule> groupStagesSchedules;

        private void ScheduleGroupStages()
        {
            if (groupStagesSchedules.Count() > 0)
                throw new InvalidOperationException("Can't double-gen schdules for group stages!");
            
            HashSet<Competitor> groupList = new HashSet<Competitor>();
            foreach (var e in this.allEntreants)
            {
                groupList.Add(e);
                if (groupList.Count == this.GroupSize)
                {
                    // TODO: Always uses round robin - fine for now but be aware!
                    var newSchedule = new Schedule_RoundRobin(this.WinnersPerGroup, groupList);
                    groupStagesSchedules.Add(newSchedule);
                    foreach(var groupEntreant in groupList)
                    {
                        this.entreantsToSchedules.Add(groupEntreant.CompetitorID, newSchedule);
                    }
                    groupList.Clear();
                }
            }
        }

        public Schedule_GroupStage(int groupSize, int winnersPerGroup, IEnumerable<Competitor> entreants)
        {
            if (entreants.Count() % groupSize != 0)
                throw new ArgumentException("Cannot evenly divide " + entreants.Count() +
                    " entreants into groups of " + groupSize);

            this.GroupSize = groupSize;
            this.WinnersPerGroup = winnersPerGroup;

            this.allEntreants = entreants;
            this.entreantsToSchedules = new Dictionary<string, Schedule>();
            this.groupStagesSchedules = new List<Schedule>();

            this.ScheduleGroupStages();
        }

        public bool IsEliminated(string competitorID)
        {
            if (this.entreantsToSchedules.ContainsKey(competitorID))
                return this.entreantsToSchedules[competitorID].IsEliminated(competitorID);
            else
                return true;
        }

        public IList<Competitor> Winners()
        {
            var winners = new List<Competitor>();
            foreach(var s in this.groupStagesSchedules)
            {
                winners.AddRange(s.Winners());
            }
            return winners;
        }

        public IList<Match> ScheduledMatches()
        {
            var allScheduledMatches = new List<Match>();
            foreach(var s in this.groupStagesSchedules)
            {
                allScheduledMatches.AddRange(s.ScheduledMatches());
            }
            return allScheduledMatches;
        }

        public IList<Match> ScheduledMatches(string competitorID)
        {
            return this.entreantsToSchedules[competitorID].ScheduledMatches(competitorID);
        }

        // Matches are retrieved one group at a time
        public Match NextMatch()
        {
            foreach(var subSchedule in this.groupStagesSchedules)
            {
                if (subSchedule.NextMatch() != null)
                    return subSchedule.NextMatch();
            }
            return null;
        }

        public IList<MatchResult> MatchHistory(string competitorID)
        {
            return this.entreantsToSchedules[competitorID].MatchHistory(competitorID);
        }

        public void ReportResult(MatchResult result)
        {
            this.entreantsToSchedules[result.Winner.CompetitorID].ReportResult(result);
        }
    }
}
