using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Executor.Tournament
{
    public interface ISchedule
    {
        // Match Info & Reporting
        Match FindMatch(string matchID);
        Match NextMatch();
        void ReportResult(MatchResult result);
        IList<ICompetitor> Winners();

        // Competitor Info
        bool IsEliminated(string competitorID);
        IList<Match> ScheduledMatches();
        IList<Match> ScheduledMatches(string competitorID);
        IList<MatchResult> MatchHistory(string competitorID);
    }
}
