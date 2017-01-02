using MechArena.Tournament;

using System;
using System.Collections.Generic;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace MechArenaTests.Tournament
{
    [TestClass]
    public class Schedule_GroupStageTest
    {
        // TODO: Util fn
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
        public void TestSplits()
        {
            var comps = this.BuildComps(24);
            var sgs = new Schedule_GroupStage(8, 1, comps);
            
            Assert.AreEqual(84, sgs.ScheduledMatches().Count);
            int breaker = 9995;
            Random r = new Random();
            while(breaker > 0 && sgs.NextMatch() != null)
            {
                if (r.Next(100) % 2 == 0)
                    sgs.ReportResult(new MatchResult(sgs.NextMatch(), sgs.NextMatch().Competitor1, 0, 0));
                else
                    sgs.ReportResult(new MatchResult(sgs.NextMatch(), sgs.NextMatch().Competitor2, 0, 0));

                breaker--;
            }
            Assert.AreEqual(3, sgs.Winners().Count);
        }
    }
}
