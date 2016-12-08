using MechArena;

using System;
using System.Collections.Generic;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace MechArenaTests
{
    [TestClass]
    public class GameRandomTest
    {
        [TestMethod]
        public void TestRandomByWeight()
        {
            Dictionary<String, int> resultCounts = new Dictionary<string, int>();
            List<String> items = new List<String>() { "A", "BCD", "12345" };
            foreach(var s in items)
            {
                resultCounts[s] = 0;
            }

            for (int i = 0; i < 5000; i++)
            {
                String result = GameRandom.RandomByWeight<String>(items, (c => c.Length));
                resultCounts[result]++;
            }

            Assert.IsTrue(499 < resultCounts["A"] && resultCounts["A"] < 611);
            Assert.IsTrue(1499 < resultCounts["BCD"] && resultCounts["BCD"] < 1832);
            Assert.IsTrue(2499 < resultCounts["12345"] && resultCounts["12345"] < 3055);
        }
    }
}
