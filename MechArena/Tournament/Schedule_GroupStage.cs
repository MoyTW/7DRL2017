using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MechArena.Tournament
{
    public class Schedule_GroupStage : ISchedule
    {
        public int GroupSize { get; }
        public int WinnersPerGroup { get; }

        private IEnumerable<ICompetitor> allEntreants;
        private Dictionary<string, ISchedule> entreantsToSchedules;
        private List<ISchedule> groupStagesSchedules;

        private void ScheduleGroupStages(IMapPicker picker)
        {
            if (groupStagesSchedules.Count() > 0)
                throw new InvalidOperationException("Can't double-gen schdules for group stages!");
            
            HashSet<ICompetitor> groupList = new HashSet<ICompetitor>();
            foreach (var e in this.allEntreants)
            {
                groupList.Add(e);
                if (groupList.Count == this.GroupSize)
                {
                    // TODO: Always uses round robin - fine for now but be aware!
                    var newSchedule = new Schedule_RoundRobin(this.WinnersPerGroup, groupList, picker);
                    groupStagesSchedules.Add(newSchedule);
                    foreach(var groupEntreant in groupList)
                    {
                        this.entreantsToSchedules.Add(groupEntreant.CompetitorID, newSchedule);
                    }
                    groupList.Clear();
                }
            }
        }

        public Schedule_GroupStage(int groupSize, int winnersPerGroup, IEnumerable<ICompetitor> entreants,
            IMapPicker picker)
        {
            if (entreants.Count() % groupSize != 0)
                throw new ArgumentException("Cannot evenly divide " + entreants.Count() +
                    " entreants into groups of " + groupSize);

            this.GroupSize = groupSize;
            this.WinnersPerGroup = winnersPerGroup;

            this.allEntreants = entreants;
            this.entreantsToSchedules = new Dictionary<string, ISchedule>();
            this.groupStagesSchedules = new List<ISchedule>();

            this.ScheduleGroupStages(picker);
        }

        #region ISchedule Fns

        public Match FindMatch(string matchID)
        {
            return this.groupStagesSchedules.Select(s => s.FindMatch(matchID))
                .Where(m => m != null)
                .FirstOrDefault();
        }

        // Matches are retrieved one group at a time
        public Match NextMatch()
        {
            foreach (var subSchedule in this.groupStagesSchedules)
            {
                if (subSchedule.NextMatch() != null)
                    return subSchedule.NextMatch();
            }
            return null;
        }

        public void ReportResult(MatchResult result)
        {
            this.entreantsToSchedules[result.Winner.CompetitorID].ReportResult(result);
        }

        public IList<ICompetitor> Winners()
        {
            var winners = new List<ICompetitor>();
            foreach (var s in this.groupStagesSchedules)
            {
                winners.AddRange(s.Winners());
            }
            return winners;
        }

        public bool IsEliminated(string competitorID)
        {
            if (this.entreantsToSchedules.ContainsKey(competitorID))
                return this.entreantsToSchedules[competitorID].IsEliminated(competitorID);
            else
                return true;
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
            if (this.entreantsToSchedules.ContainsKey(competitorID))
                return this.entreantsToSchedules[competitorID].ScheduledMatches(competitorID);
            return new List<Match>();
        }

        public IList<MatchResult> MatchHistory(string competitorID)
        {
            if (this.entreantsToSchedules.ContainsKey(competitorID))
                return this.entreantsToSchedules[competitorID].MatchHistory(competitorID);
            else
                return new List<MatchResult>();
        }

        #endregion
    }
}
