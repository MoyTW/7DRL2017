﻿using MechArena.Tournament;

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
            List<Competitor> comps = new List<Competitor>();
            for (int i = 0; i < numComps; i++)
            {
                comps.Add(new Competitor(i.ToString(), i.ToString()));
            }

            var scheduled = Scheduler.ScheduleRoundRobin(comps);

            Assert.AreEqual(countMatches, scheduled.Count);
            foreach (var c in comps)
            {
                Assert.AreEqual(countMatchesPerComp, scheduled.Where(m => m.HasCompetitor(c)).Count());
                foreach (var o in comps.Where(o => o != c))
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
