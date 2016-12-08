using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MechArena
{
    public static class GameRandom
    {
        private static Random rand = new Random();

        public static void SeedRandomizer(int seed)
        {
            rand = new Random(seed);
        }

        public static T RandomByWeight<T>(IEnumerable<T> items, Func<T, int> weight)
        {
            int totalweight = items.Sum(weight);
            int choice = rand.Next(totalweight);
            int weightIndex = 0;

            foreach (var item in items)
            {
                weightIndex += weight(item);

                if (weightIndex > choice)
                    return item;
            }

            return default(T);
        }
    }
}
