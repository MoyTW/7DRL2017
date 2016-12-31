using RogueSharp;
using RogueSharp.Random;

using System;
using System.Collections.Generic;

namespace MechArena
{
    public static class ArenaBuilder
    {
        private static Dictionary<int, Tuple<IMap, PathFinder>> seedsToMaps = new Dictionary<int, Tuple<IMap, PathFinder>>();

        // WE ASSUME THE MAP, ONCE GENERATED, NEVER CHANGES!
        // THIS MEANS NO TERRAIN DEFORMATION WITHOUT CACHING EDITS!
        public static ArenaState BuildArena(int width, int height, int seed, CompetitorEntity entreant1,
            CompetitorEntity entreant2)
        {
            if (!seedsToMaps.ContainsKey(seed))
            {
                var map = Map.Create(new RogueSharp.MapCreation.CaveMapCreationStrategy<Map>(width, height,
                    45, 4, 3, new DotNetRandom(seed)));
                var pathFinder = new PathFinder(map);
                seedsToMaps[seed] = new Tuple<IMap, PathFinder>(map, pathFinder);
            }

            var mech1 = entreant1.Mech.DeepCopy();
            var mech2 = entreant2.Mech.DeepCopy();
            ArenaState arena = new ArenaState(mech1, mech2, seedsToMaps[seed].Item1, seedsToMaps[seed].Item2, seed);

            arena.PlaceEntityNear(mech1, 25, 25);
            arena.PlaceEntityNear(mech2, 25, 25);

            return arena;
        }
    }
}
