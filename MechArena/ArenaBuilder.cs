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
        public static ArenaState BuildArena(int width, int height, int mapSeed, int arenaSeed, CompetitorEntity entreant1,
            CompetitorEntity entreant2)
        {
            if (!seedsToMaps.ContainsKey(mapSeed))
            {
                var map = Map.Create(new RogueSharp.MapCreation.CaveMapCreationStrategy<Map>(width, height,
                    45, 4, 3, new DotNetRandom(mapSeed)));
                var pathFinder = new PathFinder(map);
                seedsToMaps[mapSeed] = new Tuple<IMap, PathFinder>(map, pathFinder);
            }

            var mech1 = entreant1.Mech.DeepCopy();
            var mech2 = entreant2.Mech.DeepCopy();
            ArenaState arena = new ArenaState(mech1, mech2, mapSeed, seedsToMaps[mapSeed].Item1, seedsToMaps[mapSeed].Item2, arenaSeed);

            arena.PlaceEntityNear(mech1, width - 15, height - 15);
            arena.PlaceEntityNear(mech2, 15, 15);

            return arena;
        }
    }
}
