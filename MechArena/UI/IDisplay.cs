using RLNET;

namespace MechArena.UI
{
    interface IDisplay
    {
        IDisplay NextDisplay { get; }
        void OnRootConsoleUpdate(RLConsole console, RLKeyPress keyPress);
        void Blit(RLConsole console);
    }
}
