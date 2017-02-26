using RogueSharp;
using RogueSharp.Random;

using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using MechArena.Tournament;

namespace MechArena
{
    public static class ArenaBuilder
    {
        // TODO: Concurrent Dictionary
        private static ConcurrentDictionary<string, Tuple<IMap, PathFinder>> seedsToMaps =
            new ConcurrentDictionary<string, Tuple<IMap, PathFinder>>(Config.NumThreads(), Config.NumMaps());

        public static ArenaState TestArena(int arenaSeed, Entity baseMech1, Entity baseMech2)
        {
            var arenaMap = Map.Create(new RogueSharp.MapCreation.BorderOnlyMapCreationStrategy<Map>(15, 15));
            ArenaState arena = new ArenaState(baseMech1, baseMech2, "test", arenaMap, new PathFinder(arenaMap),
                arenaSeed, "test");
            arena.PlaceEntityNear(baseMech1, 5, 5);
            arena.PlaceEntityNear(baseMech2, 10, 10);

            return arena;
        }

        public static ArenaState BuildNewArena(Schedule_Tournament t, Match m)
        {
            return BuildArena(Config.ArenaWidth, Config.ArenaHeight, m.MatchID, t.PickMapID(), t.GenArenaSeed(),
                (CompetitorEntity)m.Competitor1, (CompetitorEntity)m.Competitor2);
        }

        public static ArenaState BuildReplayArena(Schedule_Tournament t, MatchResult m)
        {
            return BuildArena(Config.ArenaWidth, Config.ArenaHeight, m.MatchID, m.MapID, m.ArenaSeed,
                (CompetitorEntity)m.Competitor1, (CompetitorEntity)m.Competitor2);
        }

        // WE ASSUME THE MAP, ONCE GENERATED, NEVER CHANGES!
        // THIS MEANS NO TERRAIN DEFORMATION WITHOUT CACHING EDITS!
        private static ArenaState BuildArena(int width, int height, string matchID, string mapID, int arenaSeed,
            CompetitorEntity entreant1, CompetitorEntity entreant2)
        {
            return BuildArena(width, height, matchID, mapID, arenaSeed, entreant1.Mech, entreant2.Mech);
        }

        public static ArenaState BuildArena(int width, int height, string matchID, string mapID, int arenaSeed,
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
            ArenaState arena = new ArenaState(mech1, mech2, mapID, seedsToMaps[mapID].Item1, seedsToMaps[mapID].Item2, arenaSeed, matchID);

            arena.PlaceEntityNear(mech1, width - 15, height - 15);
            arena.PlaceEntityNear(mech2, 15, 15);

            return arena;
        }
    }
}
