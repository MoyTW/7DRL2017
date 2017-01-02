using MechArena.Tournament;

using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace MechArenaTests.Tournament
{
    [TestClass]
    public class SchedulerTest
    {
        private void AssertCounts(int numComps, int countMatches, int countMatchesPerComp)
        {
            List<ICompetitor> comps = new List<ICompetitor>();
            for (int i = 0; i < numComps; i++)
            {
                comps.Add(new CompetitorPlaceholder(i.ToString(), i.ToString()));
            }

            var scheduled = Scheduler.ScheduleRoundRobin(comps);

            Assert.AreEqual(countMatches, scheduled.Count);
            foreach (var c in comps.Select(c => c.CompetitorID))
            {
                Assert.AreEqual(countMatchesPerComp, scheduled.Where(m => m.HasCompetitor(c)).Count());
                foreach (var o in comps.Where(o => o.CompetitorID != c).Select(o => o.CompetitorID))
                {
                    Assert.AreEqual(1, scheduled.Where(m => m.HasCompetitor(c) && m.HasCompetitor(o))
                        .Count());
                }
            }
        }

        [TestMethod]
        public void TestRoundRobin()
        {
            this.AssertCounts(2, 1, 1);
            this.AssertCounts(3, 3, 2);
            this.AssertCounts(5, 10, 4);
            this.AssertCounts(8, 28, 7);
        }
    }
}
