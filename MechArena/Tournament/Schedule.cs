using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MechArena.Tournament
{
    public interface Schedule
    {
        void ReportResult(MatchResult result);
        bool IsEliminated(Competitor c);
        Match NextMatch();
        IList<Match> ScheduledMatches(Competitor c);
        IList<MatchResult> MatchHistory(Competitor c);
    }
}
