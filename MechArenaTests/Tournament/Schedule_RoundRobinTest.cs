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
        [TestMethod]
        public void TestRoundRobin()
        {
            List<Competitor> comps = new List<Competitor>();
            for (int i = 0; i < 8; i++)
            {
                comps.Add(new Competitor(i.ToString(), i.ToString()));
            }

            var s = new Schedule_RoundRobin(comps);

            Assert.AreEqual(28, s.ScheduledMatches.Count);
            foreach(var c in comps)
            {
                Assert.AreEqual(7, s.ScheduledMatches.Where(m => m.HasCompetitor(c)).Count());
                foreach (var o in comps.Where(o => o != c))
                {
                    Assert.AreEqual(1, s.ScheduledMatches.Where(m => m.HasCompetitor(c) && m.HasCompetitor(o))
                        .Count());
                }
            }
        }
    }
}
