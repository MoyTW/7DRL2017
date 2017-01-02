using MechArena.Tournament;

using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace MechArenaTests.Tournament
{
    [TestClass]
    public class Schedule_RoundRobinTest
    {
        private List<ICompetitor> BuildComps(int numComps)
        {
            List<ICompetitor> comps = new List<ICompetitor>();
            for (int i = 0; i < numComps; i++)
            {
                comps.Add(new CompetitorPlaceholder(i.ToString(), i.ToString()));
            }
            return comps;
        }

        [TestMethod]
        public void TestTiebreakerSimple()
        {
            var comps = this.BuildComps(3);
            var srr = new Schedule_RoundRobin(1, comps, new MapPickerPlaceholder());

            // Match ordering: [0:1] [0:2] [1:2]
            srr.ReportResult(new MatchResult(srr.NextMatch(), comps[0], "", 0));
            srr.ReportResult(new MatchResult(srr.NextMatch(), comps[2], "", 0));
            srr.ReportResult(new MatchResult(srr.NextMatch(), comps[1], "", 0));

            Assert.AreEqual(3, srr.ScheduledMatches().Count);
            Assert.AreEqual(3, srr.ScheduledMatches().Where(m => m.IsTieBreaker).Count());
        }

        [TestMethod]
        public void TestTiebreakerModerate()
        {
            var comps = this.BuildComps(4);
            var srr = new Schedule_RoundRobin(3, comps, new MapPickerPlaceholder());

            // Match ordering: [0:1] [2:3] [0:2] [3:1] [0:3] [1:2]
            // 0 wins 3, 1 wins 1, 2 wins 1, 3 wins 0
            srr.ReportResult(new MatchResult(srr.NextMatch(), comps[0], "", 0));
            srr.ReportResult(new MatchResult(srr.NextMatch(), comps[2], "", 0));
            srr.ReportResult(new MatchResult(srr.NextMatch(), comps[0], "", 0));
            srr.ReportResult(new MatchResult(srr.NextMatch(), comps[3], "", 0));
            srr.ReportResult(new MatchResult(srr.NextMatch(), comps[0], "", 0));
            srr.ReportResult(new MatchResult(srr.NextMatch(), comps[1], "", 0));

            Assert.AreEqual(3, srr.ScheduledMatches().Count);
            Assert.AreEqual(3, srr.ScheduledMatches().Where(m => m.IsTieBreaker).Count());
        }

        [TestMethod]
        public void TestTiebreakerComplex()
        {
            var comps = this.BuildComps(5);
            var srr = new Schedule_RoundRobin(2, comps, new MapPickerPlaceholder());

            // Match ordering: [0:1] [3:4] [0:2] [3:1] [0:3] [4:2] [0:4] [1:2] [1:4] [2:3]
            // 0 -> [4 0]
            // 1 -> [2 2]
            // 2 -> [2 2]
            // 3 -> [1 3]
            // 4 -> [1 3]
            srr.ReportResult(new MatchResult(srr.NextMatch(), comps[0], "", 0));
            srr.ReportResult(new MatchResult(srr.NextMatch(), comps[3], "", 0));
            srr.ReportResult(new MatchResult(srr.NextMatch(), comps[0], "", 0));
            srr.ReportResult(new MatchResult(srr.NextMatch(), comps[1], "", 0));
            srr.ReportResult(new MatchResult(srr.NextMatch(), comps[0], "", 0));
            srr.ReportResult(new MatchResult(srr.NextMatch(), comps[4], "", 0));
            srr.ReportResult(new MatchResult(srr.NextMatch(), comps[0], "", 0));
            srr.ReportResult(new MatchResult(srr.NextMatch(), comps[2], "", 0));
            srr.ReportResult(new MatchResult(srr.NextMatch(), comps[1], "", 0));
            srr.ReportResult(new MatchResult(srr.NextMatch(), comps[2], "", 0));

            Assert.AreEqual(1, srr.ScheduledMatches().Count);
            Assert.AreEqual(1, srr.ScheduledMatches().Where(m => m.IsTieBreaker).Count());
            Assert.IsTrue(srr.IsEliminated(comps[3].CompetitorID));
            Assert.IsTrue(srr.IsEliminated(comps[4].CompetitorID));

            srr.ReportResult(new MatchResult(srr.NextMatch(), comps[1], "", 0));
            Assert.IsTrue(srr.Winners().Contains(comps[0]));
            Assert.IsTrue(srr.Winners().Contains(comps[1]));
        }
    }
}
