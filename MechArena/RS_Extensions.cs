using RogueSharp;

using System.Collections.Generic;
using System;

namespace MechArena
{
    public static class RS_Extensions
    {
        public static bool IsWalkableAndOpen(this IMap iMap, int x, int y, IList<Entity> mapEntities)
        {
            foreach(var en in mapEntities)
            {
                var position = (GameQuery_Position)en.HandleQuery(new GameQuery_Position());
                if (position != null && position.BlocksMovement && position.X == x && position.Y == y)
                    return false;
            }
            return iMap.IsWalkable(x, y);
        }
    }
}
