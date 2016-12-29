using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MechArena.Tournament
{
    public class Schedule_RoundRobin
    {
        private List<Match> matches;
        public IList<Match> ScheduledMatches { get { return matches.AsReadOnly(); } }

        // The implementation here is kind of silly!
        public static List<Match> ScheduleCompetitors(List<Competitor> originalCompetitors)
        {
            var competitors = new List<Competitor>(originalCompetitors);

            if (competitors.Count < 2)
                throw new ArgumentException("Can't schedule if only 1 competitor!");

            var bye = new Competitor("TOURNAMENT_BYE", "TOURNAMENT_BYE");
            if (competitors.Count % 2 != 0)
            {
                competitors.Add(bye);
            }

            List<Match> matches = new List<Match>();

            List<Competitor> teams = new List<Competitor>(competitors);
            teams.RemoveAt(0);

            int teamsSize = teams.Count;

            for (int day = 0; day < competitors.Count - 1; day++)
            {
                int teamIdx = day % teamsSize;

                matches.Add(new Match(competitors[0], teams[teamIdx]));

                for (int idx = 1; idx < competitors.Count / 2; idx++)
                {
                    var match = new Match(teams[(day + idx) % teamsSize],
                        teams[(day + teamsSize - idx) % teamsSize]);
                    matches.Add(match);
                }
            }

            return matches.Where(m => !m.HasCompetitor(bye)).ToList();
        }

        public Schedule_RoundRobin(List<Competitor> competitors)
        {
            this.matches = ScheduleCompetitors(competitors);
        }
    }
}
