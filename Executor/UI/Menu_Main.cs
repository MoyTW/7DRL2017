using Executor.AI;
using RLNET;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace Executor.UI
{
    class Menu_Main : IDisplay
    {
        private readonly bool _playPlayerMatches = false;

        private Menu_Arena arenaMenu;

        public int Width { get; }
        public int Height { get; }

        public Menu_Main(int width, int height)
        {
            this.Width = width;
            this.Height = height;
        }

        // Put each case into own fn, this is just exceptionally unwieldy!
        private IDisplay HandleKeyPressed(RLKeyPress keyPress)
        {
            if (keyPress == null)
                throw new InvalidOperationException("Called HandleKeyPressed with null, don't do this!");

            switch (keyPress.Key)
            {
                case RLKey.L:
                    Log.ToggleDebugLog();
                    return this;
                case RLKey.N:
                    throw new NotImplementedException ();
                case RLKey.R:
                    if (this.arenaMenu != null && !this.arenaMenu.MatchEnded)
                        return this.arenaMenu;
                    else
                    {
                        Log.InfoLine("Cannot re-spectate - no arena!");
                        return this;
                    }
                case RLKey.Escape:
                    Environment.Exit(0);
                    return this;
                default:
                    return this;
            }
        }

        public IDisplay OnRootConsoleUpdate(RLConsole console, RLKeyPress keyPress)
        {
            if (keyPress != null)
                return this.HandleKeyPressed(keyPress);
            else
                return this;
        }

        public void Blit(RLConsole console)
        {
            int baseX = this.Width / 2 - 5;
            int baseY = this.Height / 2 - 8;
            console.SetBackColor(0, 0, this.Width, this.Height, RLColor.Black);
            console.Print(baseX - 4, baseY, "Main Menu", RLColor.White);

            console.Print(baseX - 4, baseY + 2, "Options", RLColor.White);
            console.Print(baseX - 2, baseY + 3, "N) Play Next Match", RLColor.White);
            console.Print(baseX - 2, baseY + 4, "R) Return To Game", RLColor.White);
            console.Print(baseX - 2, baseY + 5, "T) Fast-Forward Tournament", RLColor.White);
            console.Print(baseX - 2, baseY + 6, "H) View Match History", RLColor.White);
            console.Print(baseX - 2, baseY + 8, "Esc) Quit", RLColor.White);

            console.Print(baseX - 4, baseY + 10, "Arena Keys", RLColor.White);
            console.Print(baseX - 2, baseY + 11, "Movement: NumPad, HJKLYUBN, Arrow Keys", RLColor.White);
            console.Print(baseX - 2, baseY + 12, "Fire Weapons: F", RLColor.White);
            console.Print(baseX - 2, baseY + 13, "Delay For One TU: P", RLColor.White);
            console.Print(baseX - 2, baseY + 14, "Delay Until Next Action: Space", RLColor.White);
            console.Print(baseX - 2, baseY + 15, "Delay For Full Cooldown: Enter", RLColor.White);
        }
    }
}
