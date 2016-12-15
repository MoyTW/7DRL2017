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

        private static RLRootConsole _rootConsole;

        private static Arena arena;

        public static void Main()
        {
            // Set up a Arena (move this later)
            Entity player = EntityBuilder.BuildPlayer();
            Entity enemy = EntityBuilder.BuildMech("Test Enemy");
            IMap arenaMap = Map.Create(
                new RogueSharp.MapCreation.CaveMapCreationStrategy<Map>(_screenWidth, _screenHeight, 45, 4, 3));
            arena = new Arena(player, enemy, arenaMap);

            arena.PlaceEntityNear(player, 25, 25);
            arena.PlaceEntityNear(enemy, 25, 25);

            // This must be the exact name of the bitmap font file we are using or it will error.
            string fontFileName = "terminal8x8.png";
            // The title will appear at the top of the console window
            string consoleTitle = "Mech Arena (I'm good at naming!) (In-Progress)";
            // Tell RLNet to use the bitmap font that we specified and that each tile is 8 x 8 pixels
            _rootConsole = new RLRootConsole(fontFileName, _screenWidth, _screenHeight, 8, 8, 1f, consoleTitle);
            // Set up a handler for RLNET's Update event
            _rootConsole.Update += OnRootConsoleUpdate;
            // Set up a handler for RLNET's Render event
            _rootConsole.Render += OnRootConsoleRender;
            // Begin RLNET's game loop
            _rootConsole.Run();
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
                        arena.TryPlayerAttack();
                        break;
                    case RLKey.Up:
                        arena.TryPlayerMove(0, -1);
                        break;
                    case RLKey.Down:
                        arena.TryPlayerMove(0, 1);
                        break;
                    case RLKey.Left:
                        arena.TryPlayerMove(-1, 0);
                        break;
                    case RLKey.Right:
                        arena.TryPlayerMove(1, 0);
                        break;
                    case RLKey.Keypad1:
                        arena.TryPlayerMove(-1, 1);
                        break;
                    case RLKey.Keypad2:
                        arena.TryPlayerMove(0, 1);
                        break;
                    case RLKey.Keypad3:
                        arena.TryPlayerMove(1, 1);
                        break;
                    case RLKey.Keypad4:
                        arena.TryPlayerMove(-1, 0);
                        break;
                    case RLKey.Keypad6:
                        arena.TryPlayerMove(1, 0);
                        break;
                    case RLKey.Keypad7:
                        arena.TryPlayerMove(-1, -1);
                        break;
                    case RLKey.Keypad8:
                        arena.TryPlayerMove(0, -1);
                        break;
                    case RLKey.Keypad9:
                        arena.TryPlayerMove(1, -1);
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
            arena.Blit(_rootConsole);
            _rootConsole.Draw();
        }
    }
}
