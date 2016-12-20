using RogueSharp;

namespace MechArena
{
    public static class ArenaBuilder
    {
        public static Arena BuildTestArena(int width, int height)
        {
            // Set up a Arena (move this later)
            Entity player = EntityBuilder.BuildPlayer();
            Entity enemy = EntityBuilder.BuildArmoredMech("Heavily Armored Test Enemy");
            IMap arenaMap = Map.Create(
                new RogueSharp.MapCreation.CaveMapCreationStrategy<Map>(width, height, 45, 4, 3));
            Arena arena = new Arena(player, enemy, arenaMap);

            arena.PlaceEntityNear(player, 25, 25);
            arena.PlaceEntityNear(enemy, 25, 25);

            return arena;
        }
    }
}
