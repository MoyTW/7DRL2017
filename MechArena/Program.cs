using MechArena.Tournament;
using MechArena.UI;
using RLNET;
using RogueSharp.Random;
using System;
using System.Linq;

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

        // Menus
        private static Menu_Main _mainMenu;
        private static Menu_CompetitorListing _competitorListingMenu;
        private static Menu_CompetitorDetails _competitorDetailsMenu;

        // Tournament
        private static ICompetitor _player;
        private static Schedule_Tournament _tournament;
        private static Menu_Arena _arenaDrawer;

        public static void Main()
        {
            _player = new CompetitorEntity(new Entity(label: "Player"),
                EntityBuilder.BuildSniperMech("Player Mech", true));
            //EntityBuilder.BuildKnifeMech("Player Knifer", true));
            //EntityBuilder.BuildDoomCannonMech("Doom Cannon Mech", true));
            _tournament = TournamentBuilder.BuildTournament(_player, new DotNetRandom(1), new DotNetRandom(2),
                new TournamentMapPicker(5, new DotNetRandom(3)));

            _gameState = GameState.MAIN_MENU;
            _mainMenu = new Menu_Main(_screenWidth, _screenHeight, _player, _tournament);

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

        private static void GotoArenaForMatch(MatchResult result)
        {
            var arena = ArenaBuilder.BuildArena(Menu_Arena.arenaWidth, Menu_Arena.arenaHeight, result.MatchID,
                result.MapID, result.ArenaSeed, (CompetitorEntity)result.Competitor1,
                (CompetitorEntity)result.Competitor2);
            _arenaDrawer = new Menu_Arena(_mainMenu, arena, _tournament);
            _gameState = GameState.ARENA;
        }

        // Event handler for RLNET's Update event
        private static void OnRootConsoleUpdate(object sender, UpdateEventArgs e)
        {
            IDisplay nextDisplay = null;

            switch (_gameState)
            {
                case GameState.MAIN_MENU:
                    nextDisplay = _mainMenu.OnRootConsoleUpdate(_rootConsole, _rootConsole.Keyboard.GetKeyPress());
                    if (nextDisplay is Menu_Arena)
                    {
                        _gameState = GameState.ARENA;
                        _arenaDrawer = (Menu_Arena)nextDisplay;
                    }
                    else if (nextDisplay is Menu_CompetitorListing)
                    {
                        _gameState = GameState.COMPETITOR_MENU;
                        _competitorListingMenu = (Menu_CompetitorListing)nextDisplay;
                    }
                    break;
                case GameState.ARENA:
                    nextDisplay = _arenaDrawer.OnRootConsoleUpdate(_rootConsole, _rootConsole.Keyboard.GetKeyPress());
                    if (nextDisplay is Menu_Main)
                        _gameState = GameState.MAIN_MENU;
                    break;
                case GameState.COMPETITOR_MENU:
                    nextDisplay = _competitorListingMenu.OnRootConsoleUpdate(_rootConsole, _rootConsole.Keyboard.GetKeyPress());
                    if (nextDisplay is Menu_Main)
                        _gameState = GameState.MAIN_MENU;
                    else if (nextDisplay is Menu_CompetitorDetails)
                    {
                        _competitorDetailsMenu = (Menu_CompetitorDetails)nextDisplay;
                        _gameState = GameState.COMPETITOR_HISTORY;
                    }
                    break;
                case GameState.COMPETITOR_HISTORY:
                    nextDisplay = _competitorDetailsMenu.OnRootConsoleUpdate(_rootConsole, _rootConsole.Keyboard.GetKeyPress());
                    if (nextDisplay is Menu_CompetitorListing)
                    {
                        // TODO: Write transition fn
                        _competitorListingMenu = (Menu_CompetitorListing)nextDisplay;
                        _gameState = GameState.COMPETITOR_MENU;
                    }
                    else if (_competitorDetailsMenu.SelectedMatch != null)
                    {
                        if (_competitorDetailsMenu.SelectedMatch.HasCompetitor(_player.CompetitorID))
                        {
                            Log.InfoLine("Can't replay player matches!");
                            _competitorDetailsMenu.ResetCompetitorHistory();
                        }
                        else
                        {
                            GotoArenaForMatch(_competitorDetailsMenu.SelectedMatch);
                        }
                    }
                    break;
                default:
                    nextDisplay = _mainMenu.OnRootConsoleUpdate(_rootConsole, _rootConsole.Keyboard.GetKeyPress());
                    if (nextDisplay is Menu_Arena)
                    {
                        _gameState = GameState.ARENA;
                        _arenaDrawer = (Menu_Arena)nextDisplay;
                    }
                    else if (nextDisplay is Menu_CompetitorListing)
                    {
                        _gameState = GameState.COMPETITOR_MENU;
                        _competitorListingMenu = (Menu_CompetitorListing)nextDisplay;
                    }
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
                    _mainMenu.Blit(_rootConsole);
                    break;
                case GameState.ARENA:
                    _arenaDrawer.Blit(_rootConsole);
                    break;
                case GameState.COMPETITOR_MENU:
                    _competitorListingMenu.Blit(_rootConsole, _tournament);
                    break;
                case GameState.COMPETITOR_HISTORY:
                    _competitorDetailsMenu.Blit(_rootConsole);
                    break;
                default:
                    break;
            }

            _rootConsole.Draw();
        }
    }
}
