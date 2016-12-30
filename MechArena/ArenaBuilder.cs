using RogueSharp;
using RogueSharp.Random;

using System.Collections.Generic;

namespace MechArena
{
    public static class ArenaBuilder
    {
        private static Dictionary<int, IMap> seedsToMaps = new Dictionary<int, IMap>();

        public static ArenaState BuildArena(int width, int height, int seed, CompetitorEntity entreant1,
            CompetitorEntity entreant2)
        {
            IMap arenaMap = new Map(width, height);
            if (!seedsToMaps.ContainsKey(seed))
            {
                seedsToMaps[seed] = Map.Create(new RogueSharp.MapCreation.CaveMapCreationStrategy<Map>(width, height,
                    45, 4, 3, new DotNetRandom(seed)));
            }
            arenaMap.Copy(seedsToMaps[seed]);

            var mech1 = entreant1.Mech.DeepCopy();
            var mech2 = entreant2.Mech.DeepCopy();
            ArenaState arena = new ArenaState(mech1, mech2, arenaMap, seed);

            arena.PlaceEntityNear(mech1, 25, 25);
            arena.PlaceEntityNear(mech2, 25, 25);

            return arena;
        }
    }
}
