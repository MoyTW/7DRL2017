using RLNET;
using RogueSharp;

using MechArena.UI;

using System;
using System.Collections.Generic;

namespace MechArena
{
    public enum GameState
    {
        MAIN_MENU = 0,
        ARENA
    }

    public class Program
    {
        // The screen height and width are in number of tiles
        private static readonly int _screenWidth = 115;
        private static readonly int _screenHeight = 80;
        private static RLRootConsole _rootConsole;

        private static GameState gameState;

        private static Arena _arena;
        private static ArenaDrawer _arenaDrawer;

        public static void Main()
        {
            gameState = GameState.MAIN_MENU;

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

        private static void GotoMainMenu()
        {
            gameState = GameState.MAIN_MENU;
        }

        private static void OnRootConsoleUpdateForArena(object sender, UpdateEventArgs e)
        {
            if (_arena.IsMatchEnded())
            {
                Console.WriteLine("Match has ended!");
                GotoMainMenu();
                return;
            }

            _arenaDrawer.OnRootConsoleUpdate(_rootConsole);

            RLKeyPress keyPress = _rootConsole.Keyboard.GetKeyPress();
            if (keyPress != null)
            {
                switch (keyPress.Key)
                {
                    case RLKey.Escape:
                        GotoMainMenu();
                        break;
                    case RLKey.P:
                        _arena.PlayerDelayAction(DelayDuration.SINGLE_TICK);
                        break;
                    case RLKey.Space:
                        _arena.PlayerDelayAction(DelayDuration.NEXT_ACTION);
                        break;
                    case RLKey.Enter:
                        _arena.PlayerDelayAction(DelayDuration.FULL_INTERVAL);
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
            else
            {
                _arena.TryFindAndExecuteNextCommand();
            }
        }

        private static void GotoNewArena()
        {
            _arena = ArenaBuilder.BuildTestArena(ArenaDrawer.arenaWidth, ArenaDrawer.arenaHeight);
            _arenaDrawer = new ArenaDrawer(_arena);
            gameState = GameState.ARENA;
        }

        private static void GotoCurrentArena()
        {
            if (_arena != null)
                gameState = GameState.ARENA;
            else
                GotoNewArena();
        }

        private static void OnRootConsoleUpdateForMainMeu(object sender, UpdateEventArgs e)
        {
            RLKeyPress keyPress = _rootConsole.Keyboard.GetKeyPress();
            if (keyPress != null)
            {
                switch (keyPress.Key)
                {
                    case RLKey.N:
                        GotoNewArena();
                        break;
                    case RLKey.R:
                        GotoCurrentArena();
                        break;
                    case RLKey.Escape:
                        Environment.Exit(0);
                        break;
                    default:
                        break;
                }
            }
        }

        // Event handler for RLNET's Update event
        private static void OnRootConsoleUpdate(object sender, UpdateEventArgs e)
        {
            switch (gameState)
            {
                case GameState.MAIN_MENU:
                    OnRootConsoleUpdateForMainMeu(sender, e);
                    break;
                case GameState.ARENA:
                    OnRootConsoleUpdateForArena(sender, e);
                    break;
                default:
                    OnRootConsoleUpdateForMainMeu(sender, e);
                    break;
            }
        }

        // Event handler for RLNET's Render event
        // TODO: Have a "Should re-render" - in theory, unsure if drawing every time actually hurts perf
        private static void OnRootConsoleRender(object sender, UpdateEventArgs e)
        {
            _rootConsole.Clear();

            switch (gameState)
            {
                case GameState.MAIN_MENU:
                    _rootConsole.SetBackColor(0, 0, _screenWidth, _screenHeight, RLColor.Black);
                    _rootConsole.Print(_screenWidth / 2 - 4, _screenHeight / 2 - 3, "Main Menu", RLColor.White);
                    _rootConsole.Print(_screenWidth / 2 - 4, _screenHeight / 2 - 1, "Options", RLColor.White);
                    _rootConsole.Print(_screenWidth / 2 - 2, _screenHeight / 2, "N) New Game", RLColor.White);
                    _rootConsole.Print(_screenWidth / 2 - 2, _screenHeight / 2 + 1, "R) Return To Game", RLColor.White);
                    _rootConsole.Print(_screenWidth / 2 - 2, _screenHeight / 2 + 2, "Esc) Quit", RLColor.White);
                    break;
                case GameState.ARENA:
                    _arenaDrawer.Blit(_rootConsole);
                    break;
                default:
                    break;
            }

            _rootConsole.Draw();
        }
    }
}
