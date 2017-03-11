using Executor.AI;
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

        public static void Main()
        {
            BlueprintListing.LoadAllBlueprints();

            _currentDisplay = new Menu_Main(_screenWidth, _screenHeight);

            // This must be the exact name of the bitmap font file we are using or it will error.
            string fontFileName = "terminal8x8.png";
            // The title will appear at the top of the console window
            string consoleTitle = "A Rougelike Where You Plan Your Moves";

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
