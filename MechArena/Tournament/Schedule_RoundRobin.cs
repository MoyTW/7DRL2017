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
        public static List<Match> ScheduleCompetitors(List<Competitor> competitors)
        {
            List<Match> matches = new List<Match>();
            Dictionary<Competitor, HashSet<Competitor>> played = new Dictionary<Competitor, HashSet<Competitor>>();
            foreach (var c in competitors)
            {
                played[c] = new HashSet<Competitor>();
            }

            HashSet<Competitor> playingThisRound = new HashSet<Competitor>();
            for (int i = 0; i < competitors.Count - 1; i++)
            {
                playingThisRound.Clear();

                foreach (var c in competitors)
                {
                    if (!playingThisRound.Contains(c))
                    {
                        var unplayed = competitors.Where(o => o != c)
                            .Where(o => !playingThisRound.Contains(o))
                            .Where(o => !played[c].Contains(o))
                            .First();

                        matches.Add(new Match(c, unplayed));

                        played[c].Add(unplayed);
                        played[unplayed].Add(c);

                        playingThisRound.Add(c);
                        playingThisRound.Add(unplayed);
                    }

                }
            }
            return matches;
        }

        public Schedule_RoundRobin(List<Competitor> competitors)
        {
            this.matches = ScheduleCompetitors(competitors);
        }
    }
}
