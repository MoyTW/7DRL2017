using RogueSharp;
using RogueSharp.Random;

namespace MechArena
{
    public static class ArenaBuilder
    {
        // Push the RNG into the Arena
        public static Arena BuildFixedTestArena(int width, int height, int seed = 100)
        {
            var seededRand = new DotNetRandom(seed);

            // Set up a Arena (move this later)
            Entity player = EntityBuilder.BuildPlayer();
            Entity enemy = EntityBuilder.BuildArmoredAIMech("Heavily Armored Test Enemy");
            IMap arenaMap = Map.Create(
                new RogueSharp.MapCreation.CaveMapCreationStrategy<Map>(width, height, 45, 4, 3, seededRand));
            Arena arena = new Arena(player, enemy, arenaMap, seededRand);

            arena.PlaceEntityNear(player, 25, 25);
            arena.PlaceEntityNear(enemy, 25, 25);

            return arena;
        }

        public static Arena BuildFixedAIVersusAIArena(int width, int height, int seed = 50)
        {
            var seededRand = new DotNetRandom(seed);

            Entity mech1 = EntityBuilder.BuildArmoredAIMech("Sherman Lynx");
            Entity mech2 = EntityBuilder.BuildArmoredAIMech("Abrams Elephant");
            IMap arenaMap = Map.Create(
                new RogueSharp.MapCreation.CaveMapCreationStrategy<Map>(width, height, 45, 4, 3, seededRand));
            Arena arena = new Arena(mech1, mech2, arenaMap, seededRand);

            arena.PlaceEntityNear(mech1, 25, 25);
            arena.PlaceEntityNear(mech2, 25, 25);

            return arena;
        }
    }
}
