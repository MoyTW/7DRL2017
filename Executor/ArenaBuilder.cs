using RogueSharp;
using RogueSharp.Random;

using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;

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

        public static ArenaState BuildArena(int width, int height, string mapID, IEnumerable<Entity> entities)
        {
            if (!seedsToMaps.ContainsKey(mapID))
            {
                var map = Map.Create(new RogueSharp.MapCreation.RandomRoomsMapCreationStrategy<Map>(width, height,
                    2000, 9, 5, new DotNetRandom(Int32.Parse(mapID))));
                var pathFinder = new PathFinder(map);
                seedsToMaps[mapID] = new Tuple<IMap, PathFinder>(map, pathFinder);
            }

            var mapEntities = new List<Entity>();
            foreach (var e in entities)
            {
                mapEntities.Add(e.DeepCopy());
            }
            ArenaState arena = new ArenaState(mapEntities, mapID, seedsToMaps[mapID].Item1, seedsToMaps[mapID].Item2);

            var openCells = arena.ArenaMap.GetAllCells().Where(c => c.IsWalkable).ToList();
            var placementRand = new DotNetRandom(Int32.Parse(mapID));
            foreach (var e in mapEntities)
            {
                while (!e.HasComponentOfType<Component_Position>())
                {
                    var cell = openCells[placementRand.Next(openCells.Count - 1)];
                    arena.PlaceEntityNear(e, cell.X, cell.Y);
                }
            }

            return arena;
        }

        public static ArenaState BuildArena(int width, int height, string mapID, int arenaSeed,
            Entity baseMech1, Entity baseMech2)
        {
            return ArenaBuilder.BuildArena(width, height, mapID, new List<Entity>() { baseMech1, baseMech2 });
        }
    }
}
