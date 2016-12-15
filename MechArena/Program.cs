using RLNET;
using RogueSharp;

using System;
using System.Collections.Generic;

namespace MechArena
{
    public class Program
    {
        // The screen height and width are in number of tiles
        private static readonly int _screenWidth = 80;
        private static readonly int _screenHeight = 50;
        private static RLRootConsole _rootConsole;

        private static readonly int _arenaWidth = 50;
        private static readonly int _arenaHeight = 50;
        private static RLConsole _arenaConsole;

        private static readonly int _statusWidth = 30;
        private static readonly int _statusHeight = 25;
        private static RLConsole _status1Console;
        private static RLConsole _status2Console;

        private static Arena arena;

        public static void Main()
        {
            // Set up a Arena (move this later)
            Entity player = EntityBuilder.BuildPlayer();
            Entity enemy = EntityBuilder.BuildMech("Test Enemy");
            IMap arenaMap = Map.Create(
                new RogueSharp.MapCreation.CaveMapCreationStrategy<Map>(_arenaWidth, _arenaHeight, 45, 4, 3));
            arena = new Arena(player, enemy, arenaMap);

            arena.PlaceEntityNear(player, 25, 25);
            arena.PlaceEntityNear(enemy, 25, 25);

            // This must be the exact name of the bitmap font file we are using or it will error.
            string fontFileName = "terminal8x8.png";
            // The title will appear at the top of the console window
            string consoleTitle = "Mech Arena (I'm good at naming!) (In-Progress)";

            // Tell RLNet to use the bitmap font that we specified and that each tile is 8 x 8 pixels
            _rootConsole = new RLRootConsole(fontFileName, _screenWidth, _screenHeight, 8, 8, 1f, consoleTitle);
            _arenaConsole = new RLConsole(_arenaWidth, _arenaHeight);
            _status1Console = new RLConsole(_statusWidth, _statusHeight);
            _status2Console = new RLConsole(_statusWidth, _statusHeight);

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
            _arenaConsole.SetBackColor(0, 0, _arenaWidth, _arenaHeight, RLColor.Black);
            _arenaConsole.Print(1, 1, "Arena", RLColor.White);

            _status1Console.SetBackColor(0, 0, _statusWidth, _statusHeight, RLColor.LightBlue);
            _status2Console.SetBackColor(0, 0, _statusWidth, _statusHeight, RLColor.LightCyan);

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

            arena.DrawArena(_arenaConsole);
            RLConsole.Blit(_arenaConsole, 0, 0, _arenaWidth, _arenaHeight, _rootConsole, 0, 0);

            arena.DrawMech1Status(_status1Console);
            RLConsole.Blit(_status1Console, 0, 0, _statusWidth, _statusHeight, _rootConsole, _arenaWidth, 0);

            arena.DrawMech2Status(_status2Console);
            RLConsole.Blit(_status2Console, 0, 0, _statusWidth, _statusHeight, _rootConsole, _arenaWidth, _statusHeight);

            _rootConsole.Draw();
        }
    }
}
