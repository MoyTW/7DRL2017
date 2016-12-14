using RLNET;
using RogueSharp;

using System;
using System.Collections.Generic;

namespace MechArena
{
    public class Program
    {
        // The screen height and width are in number of tiles
        private static readonly int _screenWidth = 50;
        private static readonly int _screenHeight = 50;
        // The starting position for the player
        private static Entity player;
        private static Entity enemy;
        // Will be tricky to track the state here (take or drop) - instead track ALL and iterate through?
        private static List<Entity> mapEntities;
 
        private static RLRootConsole _rootConsole;
        private static IMap _map;

        private static Entity PlaceEntityNear(Entity en, IMap m, int x, int y)
        {
            for(int nx = x - 1; nx <= x + 1; nx++)
            {
                for (int ny = y - 1; ny <= y + 1; ny++)
                {
                    if (m.IsWalkableAndOpen(nx, ny, mapEntities))
                    {
                        if (!mapEntities.Contains(en))
                            mapEntities.Add(en);
                        return en.AddComponent(new Component_Position(nx, ny, true));
                    }
                }
            }
            return null;
        }

        public static void Main()
        {
            mapEntities = new List<Entity>();
            player = EntityBuilder.BuildPlayer();
            enemy = EntityBuilder.BuildMech("Test Enemy");

            // Use RogueSharp to create a new cave map the same size as the screen.
            _map = Map.Create(new RogueSharp.MapCreation.CaveMapCreationStrategy<Map>(_screenWidth, _screenHeight, 45, 4, 3));

            PlaceEntityNear(player, _map, 25, 25);
            PlaceEntityNear(enemy, _map, 25, 25);

            // This must be the exact name of the bitmap font file we are using or it will error.
            string fontFileName = "terminal8x8.png";
            // The title will appear at the top of the console window
            string consoleTitle = "RougeSharp RLNet Tutorial";
            // Tell RLNet to use the bitmap font that we specified and that each tile is 8 x 8 pixels
            _rootConsole = new RLRootConsole(fontFileName, _screenWidth, _screenHeight, 8, 8, 1f, consoleTitle);
            // Set up a handler for RLNET's Update event
            _rootConsole.Update += OnRootConsoleUpdate;
            // Set up a handler for RLNET's Render event
            _rootConsole.Render += OnRootConsoleRender;
            // Begin RLNET's game loop
            _rootConsole.Run();
        }

        // TODO: Testing!
        private static void TryPlayerAttack()
        {
            Console.WriteLine("########## ATTACK INFO ##########");
            var guns = player.HandleQuery(new GameQuery_SubEntities(SubEntitiesSelector.WEAPON)).SubEntities;
            foreach(var gun in guns)
            {
                player.HandleEvent(new GameEvent_Attack(player, enemy, gun, _map));
            }
        }

        private static void TryMove(int dx, int dy)
        {
            var position = (GameQuery_Position)player.HandleQuery(new GameQuery_Position());
            if (_map.IsWalkableAndOpen(position.X + dx, position.Y + dy, mapEntities))
            {
                player.HandleEvent(new GameEvent_MoveSingle((XDirection)dx, (YDirection)dy));
            }
        }

        // Event handler for RLNET's Update event
        private static void OnRootConsoleUpdate(object sender, UpdateEventArgs e)
        {
            RLKeyPress keyPress = _rootConsole.Keyboard.GetKeyPress();
            if (keyPress != null)
            {
                switch(keyPress.Key)
                {
                    case RLKey.F:
                        TryPlayerAttack();
                        break;
                    case RLKey.Up:
                        TryMove(0, -1);
                        break;
                    case RLKey.Down:
                        TryMove(0, 1);
                        break;
                    case RLKey.Left:
                        TryMove(-1, 0);
                        break;
                    case RLKey.Right:
                        TryMove(1, 0);
                        break;
                    case RLKey.Keypad1:
                        TryMove(-1, 1);
                        break;
                    case RLKey.Keypad2:
                        TryMove(0, 1);
                        break;
                    case RLKey.Keypad3:
                        TryMove(1, 1);
                        break;
                    case RLKey.Keypad4:
                        TryMove(-1, 0);
                        break;
                    case RLKey.Keypad6:
                        TryMove(1, 0);
                        break;
                    case RLKey.Keypad7:
                        TryMove(-1, -1);
                        break;
                    case RLKey.Keypad8:
                        TryMove(0, -1);
                        break;
                    case RLKey.Keypad9:
                        TryMove(1, -1);
                        break;
                    default:
                        break;
                }
            }
        }

        // Event handler for RLNET's Render event
        private static void OnRootConsoleRender(object sender, UpdateEventArgs e)
        {
            _rootConsole.Clear();

            var enemyPosition = (GameQuery_Position)enemy.HandleQuery(new GameQuery_Position());

            // Use RogueSharp to calculate the current field-of-view for the player
            var position = (GameQuery_Position)player.HandleQuery(new GameQuery_Position());
            _map.ComputeFov(position.X, position.Y, 50, true);

            foreach (var cell in _map.GetAllCells())
            {
                // When a Cell is in the field-of-view set it to a brighter color
                if (cell.IsInFov)
                {
                    _map.SetCellProperties(cell.X, cell.Y, cell.IsTransparent, cell.IsWalkable, true);
                    if (cell.IsWalkable)
                    {
                        _rootConsole.Set(cell.X, cell.Y, RLColor.Gray, null, '.');
                    }
                    else
                    {
                        _rootConsole.Set(cell.X, cell.Y, RLColor.LightGray, null, '#');
                    }
                }
                // If the Cell is not in the field-of-view but has been explored set it darker
                else if (cell.IsExplored)
                {
                    if (cell.IsWalkable)
                    {
                        _rootConsole.Set(cell.X, cell.Y, new RLColor(30, 30, 30), null, '.');
                    }
                    else
                    {
                        _rootConsole.Set(cell.X, cell.Y, RLColor.Gray, null, '#');
                    }
                }
            }

            // Set the player's symbol after the map symbol to make sure it is draw
            _rootConsole.Set(position.X, position.Y, RLColor.LightGreen, null, '@');
            _rootConsole.Set(enemyPosition.X, enemyPosition.Y, RLColor.LightGreen, null, 'E');

            // Tell RLNET to draw the console that we set
            _rootConsole.Draw();
        }
    }
}
