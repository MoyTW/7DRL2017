using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MechArena.Tournament
{
    public interface Schedule
    {
        // Match Info & Reporting
        Match NextMatch();
        void ReportResult(MatchResult result);
        IList<Competitor> Winners();

        // Competitor Info
        bool IsEliminated(Competitor c);
        IList<Match> ScheduledMatches();
        IList<Match> ScheduledMatches(Competitor c);
        IList<MatchResult> MatchHistory(Competitor c);
    }
}
