﻿using RogueSharp;
using RogueSharp.Random;

using System;
using System.Collections.Concurrent;
using System.Collections.Generic;

namespace Executor
{
    public static class ArenaBuilder
    {
        // TODO: Concurrent Dictionary
        private static ConcurrentDictionary<string, Tuple<IMap, PathFinder>> seedsToMaps =
            new ConcurrentDictionary<string, Tuple<IMap, PathFinder>>(Config.NumThreads(), Config.NumMaps());

        public static ArenaState TestArena(Entity baseMech1, Entity baseMech2)
        {
            var arenaMap = Map.Create(new RogueSharp.MapCreation.BorderOnlyMapCreationStrategy<Map>(15, 15));
            var mapEntities = new List<Entity>() { baseMech1, baseMech2 };
            ArenaState arena = new ArenaState(mapEntities, "test", arenaMap, new PathFinder(arenaMap));
            arena.PlaceEntityNear(baseMech1, 5, 5);
            arena.PlaceEntityNear(baseMech2, 10, 10);

            return arena;
        }

        public static ArenaState BuildArena(int width, int height, string mapID, int arenaSeed,
            Entity baseMech1, Entity baseMech2)
        {
            if (!seedsToMaps.ContainsKey(mapID))
            {
                var map = Map.Create(new RogueSharp.MapCreation.CaveMapCreationStrategy<Map>(width, height,
                    45, 4, 3, new DotNetRandom(Int32.Parse(mapID))));
                var pathFinder = new PathFinder(map);
                seedsToMaps[mapID] = new Tuple<IMap, PathFinder>(map, pathFinder);
            }

            var mech1 = baseMech1.DeepCopy();
            var mech2 = baseMech2.DeepCopy();
            var mapEntities = new List<Entity>() { mech1, mech2 };
            ArenaState arena = new ArenaState(mapEntities, mapID, seedsToMaps[mapID].Item1, seedsToMaps[mapID].Item2);

            arena.PlaceEntityNear(mech1, width - 15, height - 15);
            arena.PlaceEntityNear(mech2, 15, 15);

            return arena;
        }
    }
}
