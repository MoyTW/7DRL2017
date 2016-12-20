using RLNET;
using RogueSharp;

using MechArena.UI;

using System;
using System.Collections.Generic;

namespace MechArena
{
    public class Program
    {
        // The screen height and width are in number of tiles
        private static readonly int _screenWidth = 205;
        private static readonly int _screenHeight = 80;
        private static RLRootConsole _rootConsole;

        private static Arena _arena;
        private static ArenaDrawer _arenaDrawer;

        public static void Main()
        {
            _arena = ArenaBuilder.BuildTestArena(ArenaDrawer.arenaWidth, ArenaDrawer.arenaHeight);

            // This must be the exact name of the bitmap font file we are using or it will error.
            string fontFileName = "terminal8x8.png";
            // The title will appear at the top of the console window
            string consoleTitle = "Mech Arena (I'm good at naming!) (In-Progress)";

            // Tell RLNet to use the bitmap font that we specified and that each tile is 8 x 8 pixels
            _rootConsole = new RLRootConsole(fontFileName, _screenWidth, _screenHeight, 8, 8, 1f, consoleTitle);
            _arenaDrawer = new ArenaDrawer(_arena);

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
            _arenaDrawer.OnRootConsoleUpdate(_rootConsole);

            RLKeyPress keyPress = _rootConsole.Keyboard.GetKeyPress();
            if (keyPress != null)
            {
                switch(keyPress.Key)
                {
                    case RLKey.Space:
                        _arena.PlayerPassAction();
                        break;
                    case RLKey.F:
                        _arena.TryPlayerAttack();
                        break;
                    case RLKey.Up:
                        _arena.TryPlayerMove(0, -1);
                        break;
                    case RLKey.Down:
                        _arena.TryPlayerMove(0, 1);
                        break;
                    case RLKey.Left:
                        _arena.TryPlayerMove(-1, 0);
                        break;
                    case RLKey.Right:
                        _arena.TryPlayerMove(1, 0);
                        break;
                    case RLKey.Keypad1:
                        _arena.TryPlayerMove(-1, 1);
                        break;
                    case RLKey.Keypad2:
                        _arena.TryPlayerMove(0, 1);
                        break;
                    case RLKey.Keypad3:
                        _arena.TryPlayerMove(1, 1);
                        break;
                    case RLKey.Keypad4:
                        _arena.TryPlayerMove(-1, 0);
                        break;
                    case RLKey.Keypad6:
                        _arena.TryPlayerMove(1, 0);
                        break;
                    case RLKey.Keypad7:
                        _arena.TryPlayerMove(-1, -1);
                        break;
                    case RLKey.Keypad8:
                        _arena.TryPlayerMove(0, -1);
                        break;
                    case RLKey.Keypad9:
                        _arena.TryPlayerMove(1, -1);
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
            _arenaDrawer.Blit(_rootConsole);
            _rootConsole.Draw();
        }
    }
}
