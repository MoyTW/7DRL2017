using RLNET;
using RogueSharp;

using System;
using System.Collections.Generic;

namespace MechArena
{
    public class Arena
    {
        // TODO: Don't literally have Player/Enemy, as two AIs can fight each other!
        private Entity player;
        private Entity enemy;
        private List<Entity> mapEntities;
        private IMap arenaMap;

        // TODO: Create a "Mech/Map Blueprint" so you don't pass a literal Entity/IMap instance in!
        public Arena(Entity player, Entity enemy, IMap arenaMap)
        {
            this.player = player;
            this.enemy = enemy;
            this.mapEntities = new List<Entity>();
            this.arenaMap = arenaMap;
        }

        public bool PlaceEntityNear(Entity en, int x, int y)
        {
            for (int nx = x - 1; nx <= x + 1; nx++)
            {
                for (int ny = y - 1; ny <= y + 1; ny++)
                {
                    if (this.arenaMap.IsWalkableAndOpen(nx, ny, this.mapEntities))
                    {
                        if (!this.mapEntities.Contains(en))
                            this.mapEntities.Add(en);
                        en.AddComponent(new Component_Position(nx, ny, true));
                        return true;
                    }
                }
            }
            return false;
        }

        // TODO: Testing! Don't directly call!
        public void TryPlayerAttack()
        {
            Console.WriteLine("########## ATTACK INFO ##########");
            var guns = this.player.HandleQuery(new GameQuery_SubEntities(SubEntitiesSelector.WEAPON)).SubEntities;
            foreach (var gun in guns)
            {
                this.player.HandleEvent(new GameEvent_Attack(player, enemy, gun, this.arenaMap));
            }
        }

        // TODO: Testing! Don't directly call!
        public void TryPlayerMove(int dx, int dy)
        {
            var position = this.player.HandleQuery(new GameQuery_Position());
            if (this.arenaMap.IsWalkableAndOpen(position.X + dx, position.Y + dy, mapEntities))
            {
                this.player.HandleEvent(new GameEvent_MoveSingle((XDirection)dx, (YDirection)dy));
            }
        }

        private void DrawBodyPartStatus(Entity bodyPart, int x, int y, bool mechDestroyed, RLConsole console)
        {
            var bodyPartDestroyed = bodyPart.TryGetDestroyed().Destroyed;
            var bodyPartStructure = bodyPart.TryGetAttribute(EntityAttributeType.STRUCTURE).Value;

            if (mechDestroyed || bodyPartDestroyed)
                console.Print(y, x, "  - " + bodyPart.ToString() + ":" + bodyPartStructure + " ", RLColor.Red);
            else
                console.Print(y, x, "  - " + bodyPart.ToString()+ ":" + bodyPartStructure + " ", RLColor.Black);
            x += 2;

            var mountedParts = bodyPart.HandleQuery(new GameQuery_SubEntities(SubEntitiesSelector.ALL)).SubEntities;
            foreach (var mountedPart in mountedParts)
            {
                var mountedPartDestroyed = mountedPart.TryGetDestroyed().Destroyed;
                var structure = mountedPart.TryGetAttribute(EntityAttributeType.STRUCTURE).Value;
                if (mechDestroyed || bodyPartDestroyed || mountedPartDestroyed)
                    console.Print(y, x, "    + " + mountedPart.ToString() + ":" + structure + " ", RLColor.Red);
                else
                    console.Print(y, x, "    + " + mountedPart.ToString() + ":" + structure + " ", RLColor.Black);
                x += 2;
            }
        }

        private void DrawMechStatus(Entity mech, RLConsole console)
        {
            int line = 1;

            var mechDestroyed = mech.TryGetDestroyed().Destroyed;
            if (mechDestroyed)
                console.Print(1, line, mech.ToString(), RLColor.Red);
            else
                console.Print(1, line, mech.ToString(), RLColor.Black);
            line++;
            line++;

            var bodyParts = mech.HandleQuery(new GameQuery_SubEntities(SubEntitiesSelector.BODY_PART)).SubEntities;
            int y = 1;
            foreach (var bodyPart in bodyParts)
            {
                this.DrawBodyPartStatus(bodyPart, line, y, mechDestroyed, console);
                y += 25;
            }
        }

        public void DrawMech1Status(RLConsole console)
        {
            this.DrawMechStatus(this.player, console);
        }

        public void DrawMech2Status(RLConsole console)
        {
            this.DrawMechStatus(this.enemy, console);
        }

        public void DrawArena(RLConsole console)
        {
            var enemyPosition = (GameQuery_Position)enemy.HandleQuery(new GameQuery_Position());

            // Use RogueSharp to calculate the current field-of-view for the player
            var position = (GameQuery_Position)player.HandleQuery(new GameQuery_Position());
            this.arenaMap.ComputeFov(position.X, position.Y, 50, true);

            foreach (var cell in this.arenaMap.GetAllCells())
            {
                // When a Cell is in the field-of-view set it to a brighter color
                if (cell.IsInFov)
                {
                    this.arenaMap.SetCellProperties(cell.X, cell.Y, cell.IsTransparent, cell.IsWalkable, true);
                    if (cell.IsWalkable)
                    {
                        console.Set(cell.X, cell.Y, RLColor.Gray, null, '.');
                    }
                    else
                    {
                        console.Set(cell.X, cell.Y, RLColor.LightGray, null, '#');
                    }
                }
                // If the Cell is not in the field-of-view but has been explored set it darker
                else if (cell.IsExplored)
                {
                    if (cell.IsWalkable)
                    {
                        console.Set(cell.X, cell.Y, new RLColor(30, 30, 30), null, '.');
                    }
                    else
                    {
                        console.Set(cell.X, cell.Y, RLColor.Gray, null, '#');
                    }
                }
            }

            // Set the player's symbol after the map symbol to make sure it is draw
            console.Set(position.X, position.Y, RLColor.LightGreen, null, '@');

            if (enemy.TryGetDestroyed().Destroyed)
                console.Set(enemyPosition.X, enemyPosition.Y, RLColor.LightGreen, null, 'D');
            else
                console.Set(enemyPosition.X, enemyPosition.Y, RLColor.LightGreen, null, 'E');
        }
    }
}
