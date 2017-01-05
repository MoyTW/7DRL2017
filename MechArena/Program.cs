using MechArena.Tournament;
using MechArena.UI;
using RLNET;
using RogueSharp.Random;
using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace MechArena
{
    public enum GameState
    {
        MAIN_MENU = 0,
        ARENA,
        COMPETITOR_MENU,
        COMPETITOR_HISTORY
    }

    public class Program
    {
        // The screen height and width are in number of tiles
        // Values set for 1280x720
        private static readonly int _screenWidth = 160;
        private static readonly int _screenHeight = 90;
        private static RLRootConsole _rootConsole;

        private static GameState _gameState;

        // History
        private static CompetitorMenu _competitorMenu;
        private static CompetitorHistory _competitorHistory;

        // Tournament
        private static ICompetitor _player;
        private static bool _playPlayerMatches = false;
        private static IRandom _tournamentRandom;
        private static TournamentMapPicker _tournamentPicker;
        private static Schedule_Tournament _tournament;
        private static ArenaState _arena;
        private static ArenaDrawer _arenaDrawer;

        public static void Main()
        {
            _player = new CompetitorEntity(new Entity(label: "Player"),
                EntityBuilder.BuildAlphaStrikerMech("Player Mech", true));
            //EntityBuilder.BuildKnifeMech("Player Knifer", true));
            //EntityBuilder.BuildDoomCannonMech("Doom Cannon Mech", true));
            _tournamentRandom = new DotNetRandom(1);
            _tournamentPicker = new TournamentMapPicker(5, _tournamentRandom);
            _tournament = TournamentBuilder.BuildTournament(_player, _tournamentRandom, _tournamentPicker);

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

        private static int GenArenaSeed()
        {
            return _tournamentRandom.Next(Int16.MaxValue);
        }

        private static void GotoMainMenu()
        {
            _gameState = GameState.MAIN_MENU;
        }

        // TODO: Will attempt to register replays!
        private static void HandleArenaEnded()
        {
            Log.Debug("Reporting match " + _arena.MatchID + " to Tournament!");
            _tournament.ReportResult(_arena.MatchID, _arena.WinnerID(), _arena.MapID, _arena.ArenaSeed);

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
                    case RLKey.Keypad1:
                    case RLKey.B:
                        _arena.TryPlayerMove(-1, 1);
                        break;
                    case RLKey.Keypad2:
                    case RLKey.Down:
                    case RLKey.J:
                        _arena.TryPlayerMove(0, 1);
                        break;
                    case RLKey.Keypad3:
                    case RLKey.N:
                        _arena.TryPlayerMove(1, 1);
                        break;
                    case RLKey.Keypad4:
                    case RLKey.H:
                    case RLKey.Left:
                        _arena.TryPlayerMove(-1, 0);
                        break;
                    case RLKey.Keypad6:
                    case RLKey.Right:
                    case RLKey.L:
                        _arena.TryPlayerMove(1, 0);
                        break;
                    case RLKey.Keypad7:
                    case RLKey.Y:
                        _arena.TryPlayerMove(-1, -1);
                        break;
                    case RLKey.Keypad8:
                    case RLKey.Up:
                    case RLKey.K:
                        _arena.TryPlayerMove(0, -1);
                        break;
                    case RLKey.Keypad9:
                    case RLKey.U:
                        _arena.TryPlayerMove(1, -1);
                        break;
                    default:
                        break;
                }
            }
            else
            {
                _arena.TryFindAndExecuteNextCommand();
                Thread.Sleep(50); // inelegant way of forcing games to display slow enough to spectate
            }
        }

        private static void GotoMatchArena(Match m)
        {
            // TODO: The casting here is silly!
            _arena = ArenaBuilder.BuildArena(ArenaDrawer.arenaWidth, ArenaDrawer.arenaHeight, m.MatchID,
                _tournamentPicker.PickMapID(), GenArenaSeed(), (CompetitorEntity)m.Competitor1,
                (CompetitorEntity)m.Competitor2);
            _arenaDrawer = new ArenaDrawer(_arena);
            _gameState = GameState.ARENA;
        }

        private static void GotoCurrentArena()
        {
            if (_arena != null)
                _gameState = GameState.ARENA;
            else
                Log.InfoLine("Cannot re-spectate - no arena!");
        }

        private static void GotoArenaForMatch(MatchResult result)
        {
            _arena = ArenaBuilder.BuildArena(ArenaDrawer.arenaWidth, ArenaDrawer.arenaHeight, result.MatchID,
                result.MapID, result.ArenaSeed, (CompetitorEntity)result.Competitor1,
                (CompetitorEntity)result.Competitor2);
            _arenaDrawer = new ArenaDrawer(_arena);
            _gameState = GameState.ARENA;
        }

        private static Tuple<Match, ArenaState> BuildArena(Match m)
        {
            var arena = ArenaBuilder.BuildArena(ArenaDrawer.arenaWidth, ArenaDrawer.arenaHeight, m.MatchID,
                _tournamentPicker.PickMapID(), GenArenaSeed(), (CompetitorEntity)m.Competitor1,
                (CompetitorEntity)m.Competitor2);
            return new Tuple<Match, ArenaState>(m, arena);
        }

        private static MatchResult RunArena(Tuple<Match, ArenaState> matchAndArena)
        {
            var match = matchAndArena.Item1;
            var matchArena = matchAndArena.Item2;

            while (!matchArena.IsMatchEnded())
            {
                matchArena.TryFindAndExecuteNextCommand();
            }
            return match.BuildResult(matchArena.WinnerID(), matchArena.MapID, matchArena.ArenaSeed);
        }

        private static void OnRootConsoleUpdateForMainMenu(object sender, UpdateEventArgs e)
        {
            RLKeyPress keyPress = _rootConsole.Keyboard.GetKeyPress();
            if (keyPress != null)
            {
                switch (keyPress.Key)
                {
                    case RLKey.L:
                        Log.ToggleDebugLog();
                        break;
                    // TODO: Don't just dump the info onto the console, actually display it
                    // argh UI work is the *worst*!
                    case RLKey.H:
                        _gameState = GameState.COMPETITOR_MENU;
                        _competitorMenu = new CompetitorMenu();
                        break;
                    case RLKey.M:
                        Log.InfoLine("########## UPCOMING PLAYER MATCHES ##########");
                        foreach (var m in _tournament.ScheduledMatches(_player.CompetitorID))
                        {
                            Log.InfoLine(m);
                        }
                        break;
                    case RLKey.T:
                        Log.DebugLine("T Pressed!");
                        Log.DebugLine("Round: " + _tournament.RoundNum());

                        var results = _tournament.ScheduledMatches()
                            .TakeWhile(m => !m.HasCompetitor(_player.CompetitorID))
                            // BuildArena sequential because of the RNG draws
                            .Select(m => BuildArena(m))
                            // Setting degree of parallelism not required - default is fn of # processors already
                            .AsParallel().WithDegreeOfParallelism(Config.NumThreads())
                            .Select(ma => Task.Run(() => RunArena(ma)));

                        // Reporting happens in order
                        foreach (var task in results)
                        {
                            Log.DebugLine("Winner of " + task.Result.OriginalMatch + " is " + task.Result.Winner);
                            _tournament.ReportResult(task.Result);
                        }

                        // TODO: Get rid of _match!
                        var match = _tournament.NextMatch();
                        // If it's a player match, resolve it or stop
                        if (match != null && match.HasCompetitor(_player.CompetitorID))
                        {
                            if (_playPlayerMatches)
                            {
                                Log.InfoLine("Next match is player!");
                            }
                            else
                            {
                                var playerResult = match.BuildResult(_player.CompetitorID, "0", 0);
                                Log.InfoLine("Player wins match!");
                                _tournament.ReportResult(playerResult);
                            }
                        }

                        if (_tournament.NextMatch() == null)
                        {
                            Log.InfoLine("===== WINNER IS =====");
                            Log.InfoLine("Winner is " + _tournament.Winners()[0]);
                        }
                        break;
                    case RLKey.N:
                        if (_tournament.NextMatch() != null)
                            GotoMatchArena(_tournament.NextMatch());
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

        private static void GotoCompetitorHistory(ICompetitor selectedCompetitor)
        {
            _competitorHistory = new CompetitorHistory(selectedCompetitor);
            _gameState = GameState.COMPETITOR_HISTORY;
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
                    else if (_competitorMenu.Transition != null)
                        GotoCompetitorHistory(_competitorMenu.Transition.SelectedCompetitor);
                    break;
                case GameState.COMPETITOR_HISTORY:
                    _competitorHistory.OnRootConsoleUpdate(_rootConsole, _tournament);
                    if (_competitorHistory.GotoCompetitorMenu)
                    {
                        // TODO: Write transition fn
                        _competitorMenu = new CompetitorMenu();
                        _gameState = GameState.COMPETITOR_MENU;
                    }
                    else if (_competitorHistory.SelectedMatch != null)
                    {
                        if (_competitorHistory.SelectedMatch.HasCompetitor(_player.CompetitorID))
                        {
                            Log.InfoLine("Can't replay player matches!");
                            _competitorHistory.ResetCompetitorHistory();
                        }
                        else
                        {
                            GotoArenaForMatch(_competitorHistory.SelectedMatch);
                        }
                    }
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
                    int baseX = _screenWidth / 2 - 5;
                    int baseY = _screenHeight / 2 - 8;
                    _rootConsole.SetBackColor(0, 0, _screenWidth, _screenHeight, RLColor.Black);
                    _rootConsole.Print(baseX - 4, baseY, "Main Menu", RLColor.White);

                    _rootConsole.Print(baseX - 4, baseY + 2, "Options", RLColor.White);
                    _rootConsole.Print(baseX - 2, baseY + 3, "N) Play Next Match", RLColor.White);
                    _rootConsole.Print(baseX - 2, baseY + 4, "R) Return To Game", RLColor.White);
                    _rootConsole.Print(baseX - 2, baseY + 5, "T) Fast-Forward Tournament", RLColor.White);
                    _rootConsole.Print(baseX - 2, baseY + 6, "H) View Match History", RLColor.White);
                    _rootConsole.Print(baseX - 2, baseY + 7, "M) View Upcoming Matches", RLColor.White);
                    _rootConsole.Print(baseX - 2, baseY + 8, "Esc) Quit", RLColor.White);

                    _rootConsole.Print(baseX - 4, baseY + 10, "Arena Keys", RLColor.White);
                    _rootConsole.Print(baseX - 2, baseY + 11, "Movement: NumPad, HJKLYUBN, Arrow Keys", RLColor.White);
                    _rootConsole.Print(baseX - 2, baseY + 12, "Fire Weapons: F", RLColor.White);
                    _rootConsole.Print(baseX - 2, baseY + 13, "Delay For One TU: P", RLColor.White);
                    _rootConsole.Print(baseX - 2, baseY + 14, "Delay Until Next Action: Space", RLColor.White);
                    _rootConsole.Print(baseX - 2, baseY + 15, "Delay For Full Cooldown: Enter", RLColor.White);
                    break;
                case GameState.ARENA:
                    _arenaDrawer.Blit(_rootConsole);
                    break;
                case GameState.COMPETITOR_MENU:
                    _competitorMenu.Blit(_rootConsole, _tournament);
                    break;
                case GameState.COMPETITOR_HISTORY:
                    _competitorHistory.Blit(_rootConsole, _tournament);
                    break;
                default:
                    break;
            }

            _rootConsole.Draw();
        }
    }
}
