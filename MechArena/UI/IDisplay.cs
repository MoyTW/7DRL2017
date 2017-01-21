using RLNET;

namespace MechArena.UI
{
    interface IDisplay
    {
        IDisplay OnRootConsoleUpdate(RLConsole console, RLKeyPress keyPress);
        void Blit(RLConsole console);
    }
}
