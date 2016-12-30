using RLNET;
using RogueSharp.Random;

using MechArena.Tournament;
using MechArena.UI;

using System;
using System.Threading;

namespace MechArena
{
    public enum GameState
    {
        MAIN_MENU = 0,
        ARENA,
        COMPETITOR_MENU
    }

    public class Program
    {
        // The screen height and width are in number of tiles
        private static readonly int _screenWidth = 115;
        private static readonly int _screenHeight = 80;
        private static RLRootConsole _rootConsole;

        private static GameState _gameState;

        // History
        private static CompetitorMenu _competitorMenu;

        // Tournament
        private static Competitor _player;
        private static bool _playPlayerMatches = false;
        private static IRandom _tournamentRandom;
        private static Schedule_Tournament _tournament;
        private static Match _match;
        private static ArenaState _arena;
        private static ArenaDrawer _arenaDrawer;

        public static void Main()
        {
            _player = new CompetitorEntity(new Entity(label: "Player"), EntityBuilder.BuildPlayer());
            _tournamentRandom = new DotNetRandom(1);
            _tournament = TournamentBuilder.BuildTournament(_player, _tournamentRandom);
            _match = _tournament.NextMatch();

            _gameState = GameState.MAIN_MENU;

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
            _gameState = GameState.MAIN_MENU;
        }

        private static void HandleArenaEnded()
        {
            Log.DebugLine("Match has ended!");

            if (_match != null)
            {
                Log.DebugLine("Attempting to register match results with the Tournament!");
                var winner = _match.CompetitorByID(_arena.WinnerID());
                if (winner != null)
                {
                    _tournament.ReportResult(_match.BuildResult(winner, _arena.Seed));
                    _match = null;
                    Log.DebugLine("Reported winner of match!");
                }
                else
                {
                    Log.DebugLine("Something's gone wrong! Can't get winner from ended match!");
                }
            }
            else
            {
                Log.DebugLine("Match was not an official tournament match or was a replay, not registering!");
            }

            GotoMainMenu();
        }

        private static void OnRootConsoleUpdateForArena(object sender, UpdateEventArgs e)
        {
            if (_arena.IsMatchEnded())
            {
                HandleArenaEnded();
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
                Thread.Sleep(100); // inelegant way of forcing games to display slow enough to spectate
            }
        }

        private static void GotoNextMatchArena()
        {
            _match = _tournament.NextMatch();
            // TODO: The casting here is silly!
            _arena = ArenaBuilder.BuildArena(ArenaDrawer.arenaWidth, ArenaDrawer.arenaHeight, 100,
                (CompetitorEntity)_match.Competitor1, (CompetitorEntity)_match.Competitor2);
            _arenaDrawer = new ArenaDrawer(_arena);
            _gameState = GameState.ARENA;
        }

        private static void GotoCurrentArena()
        {
            if (_arena != null)
                _gameState = GameState.ARENA;
            else
                GotoNextMatchArena();
        }

        private static void OnRootConsoleUpdateForMainMenu(object sender, UpdateEventArgs e)
        {
            RLKeyPress keyPress = _rootConsole.Keyboard.GetKeyPress();
            if (keyPress != null)
            {
                switch (keyPress.Key)
                {
                    case RLKey.L:
                        Log.EnableDebugLog();
                        break;
                    // TODO: Don't just dump the info onto the console, actually display it
                    // argh UI work is the *worst*!
                    case RLKey.H:
                        _gameState = GameState.COMPETITOR_MENU;
                        _competitorMenu = new CompetitorMenu();
                        break;
                    case RLKey.M:
                        Console.WriteLine("########## UPCOMING PLAYER MATCHES ##########");
                        foreach(var m in _tournament.ScheduledMatches(_player.CompetitorID))
                        {
                            Console.WriteLine(m);
                        }
                        break;
                    case RLKey.T:
                        Log.DebugLine("T Pressed!");
                        Log.DebugLine("Round: " + _tournament.RoundNum());
                        _match = _tournament.NextMatch();
                        while(_match != null && !_match.HasCompetitor(_player.CompetitorID))
                        {
                            int seed = 100;
                            // int seed = _tournamentRandom.Next(Int16.MaxValue);
                            // TODO: Silly cast, use interface v. actual class!
                            var matchArena = ArenaBuilder.BuildArena(ArenaDrawer.arenaWidth, ArenaDrawer.arenaHeight,
                                seed, (CompetitorEntity)_match.Competitor1, (CompetitorEntity)_match.Competitor2);
                            while (!matchArena.IsMatchEnded())
                            {
                                matchArena.TryFindAndExecuteNextCommand();
                            }
                            var result =  _match.BuildResult(matchArena.WinnerID(), seed);

                            Console.WriteLine("Winner of " + _match + " is " + result.Winner);
                            _tournament.ReportResult(result);
                            _match = _tournament.NextMatch();
                        }
                        if (_match != null)
                        {
                            if (_playPlayerMatches)
                            {
                                Console.WriteLine("Next match is player!");
                            }
                            else
                            {
                                var result = _match.BuildResult(_player.CompetitorID, 0);
                                Console.WriteLine("Player wins match!");
                                _tournament.ReportResult(result);
                                _match = _tournament.NextMatch();
                            }
                        }
                        else
                        {
                            Console.WriteLine("===== WINNER IS =====");
                            Console.WriteLine("Winner is " + _tournament.Winners()[0]);
                        }
                        break;
                    case RLKey.N:
                        GotoNextMatchArena();
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
            switch (_gameState)
            {
                case GameState.MAIN_MENU:
                    OnRootConsoleUpdateForMainMenu(sender, e);
                    break;
                case GameState.ARENA:
                    OnRootConsoleUpdateForArena(sender, e);
                    break;
                case GameState.COMPETITOR_MENU:
                    _competitorMenu.OnRootConsoleUpdate(_rootConsole, _tournament);
                    if (_competitorMenu.GotoMainMenu)
                        _gameState = GameState.MAIN_MENU;
                    break;
                default:
                    OnRootConsoleUpdateForMainMenu(sender, e);
                    break;
            }
        }

        // Event handler for RLNET's Render event
        // TODO: Have a "Should re-render" - in theory, unsure if drawing every time actually hurts perf
        private static void OnRootConsoleRender(object sender, UpdateEventArgs e)
        {
            _rootConsole.Clear();

            switch (_gameState)
            {
                case GameState.MAIN_MENU:
                    _rootConsole.SetBackColor(0, 0, _screenWidth, _screenHeight, RLColor.Black);
                    _rootConsole.Print(_screenWidth / 2 - 4, _screenHeight / 2 - 3, "Main Menu", RLColor.White);
                    _rootConsole.Print(_screenWidth / 2 - 4, _screenHeight / 2 - 1, "Options", RLColor.White);
                    _rootConsole.Print(_screenWidth / 2 - 2, _screenHeight / 2 + 0, "N) Play Next Match", RLColor.White);
                    _rootConsole.Print(_screenWidth / 2 - 2, _screenHeight / 2 + 1, "R) Return To Game", RLColor.White);
                    _rootConsole.Print(_screenWidth / 2 - 2, _screenHeight / 2 + 2, "T) Fast-Forward Tournament", RLColor.White);
                    _rootConsole.Print(_screenWidth / 2 - 2, _screenHeight / 2 + 3, "H) View Match History", RLColor.White);
                    _rootConsole.Print(_screenWidth / 2 - 2, _screenHeight / 2 + 4, "M) View Upcoming Matches", RLColor.White);
                    _rootConsole.Print(_screenWidth / 2 - 2, _screenHeight / 2 + 5, "Esc) Quit", RLColor.White);
                    break;
                case GameState.ARENA:
                    _arenaDrawer.Blit(_rootConsole);
                    break;
                case GameState.COMPETITOR_MENU:
                    _competitorMenu.Blit(_rootConsole, _tournament);
                    break;
                default:
                    break;
            }

            _rootConsole.Draw();
        }
    }
}
