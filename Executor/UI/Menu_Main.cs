using Executor.AI;
using RLNET;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Executor.UI
{
    class Menu_Main : IDisplay
    {
        private Menu_Arena arenaMenu;

        private Random rand;
        public int Width { get; }
        public int Height { get; }

        public Menu_Main(int width, int height)
        {
            // TODO: maybe take entropy from outside
            this.rand = new Random();
            this.Width = width;
            this.Height = height;
        }

        public void SetArena(Menu_Arena arenaMenu)
        {
            this.arenaMenu = arenaMenu;
        }

        private Menu_Arena NewArenaMenu(int seed=1)
        {
            RogueSharp.Random.IRandom iRand = new RogueSharp.Random.DotNetRandom(seed);

            var player = EntityBuilder.BuildPlayerEntity();
            var arena = ArenaBuilder.BuildArena(Config.ArenaWidth, Config.ArenaHeight, iRand.Next(Int16.MaxValue).ToString(),
                player, 0);
            this.arenaMenu = new Menu_Arena(this, arena);
            return this.arenaMenu;
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
                    return this.NewArenaMenu(rand.Next());
                case RLKey.M:
                    return this.NewArenaMenu();
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
            console.Print(baseX - 4, baseY - 1, "A Roguelike Where You Plan Your Moves", RLColor.White);
            console.Print(baseX - 4, baseY + 1, "              By MTW", RLColor.White);

            console.Print(baseX - 4, baseY + 3, "Options", RLColor.White);
            console.Print(baseX - 2, baseY + 4, "N) Play New Arena", RLColor.White);
            console.Print(baseX - 2, baseY + 5, "R) Return To Game", RLColor.White);
            console.Print(baseX - 2, baseY + 6, "Esc) Quit", RLColor.White);
        }
    }
}
