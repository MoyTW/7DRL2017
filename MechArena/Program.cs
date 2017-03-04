using Executor.AI;
using Executor.Tournament;
using Executor.UI;
using RLNET;
using RogueSharp.Random;

namespace Executor
{
    public class Program
    {
        // The screen height and width are in number of tiles
        // Values set for 1280x720
        private static readonly int _screenWidth = 160;
        private static readonly int _screenHeight = 90;
        private static RLRootConsole _rootConsole;

        // Menus
        private static IDisplay _currentDisplay;

        // Tournament
        private static ICompetitor _player;
        private static Schedule_Tournament _tournament;

        public static void Main()
        {
            BlueprintListing.LoadAllBlueprints();
            AIUtils.Initialize();

            var sniperModifier = new Component_AttributeModifier(EntityAttributeType.DAMAGE, ModifierType.FLAT, 100,
                requiredBaseLabel: Blueprints.SNIPER_RILFE);
            var sniperToHit = new Component_AttributeModifier(EntityAttributeType.TO_HIT, ModifierType.FLAT, 100,
                requiredBaseLabel: Blueprints.SNIPER_RILFE);
            var playerPilot = new Entity(label: "You").AddComponent(sniperModifier).AddComponent(sniperToHit);
            var sniperMech = EntityBuilder.BuildSniperMech("Player Mech", true);
            sniperMech.GetComponentOfType<Component_Piloted>().Pilot = playerPilot;

            _player = new CompetitorEntity(playerPilot, sniperMech);
            //EntityBuilder.BuildKnifeMech("Player Knifer", true));
            //EntityBuilder.BuildDoomCannonMech("Doom Cannon Mech", true));
            _tournament = TournamentBuilder.BuildTournament(_player, new DotNetRandom(1), new DotNetRandom(2),
                new TournamentMapPicker(5, new DotNetRandom(3)));

            _currentDisplay = new Menu_Main(_screenWidth, _screenHeight, _player, _tournament);

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
            _currentDisplay = _currentDisplay.OnRootConsoleUpdate(_rootConsole, _rootConsole.Keyboard.GetKeyPress());
        }

        // Event handler for RLNET's Render event
        // TODO: Have a "Should re-render" - in theory, unsure if drawing every time actually hurts perf
        private static void OnRootConsoleRender(object sender, UpdateEventArgs e)
        {
            _rootConsole.Clear();
            _currentDisplay.Blit(_rootConsole);
            _rootConsole.Draw();
        }
    }
}
