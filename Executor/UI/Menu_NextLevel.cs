using RLNET;
using System;

namespace Executor.UI
{
    class Menu_NextLevel : IDisplay
    {
        private Menu_Main mainMenu;
        private Entity player;
        private int nextLevel;

        public Menu_NextLevel(Menu_Main mainMenu, Entity player, int nextLevel)
        {
            this.mainMenu = mainMenu;
            this.player = player;
            this.nextLevel = nextLevel;
        }


        public IDisplay OnRootConsoleUpdate(RLConsole console, RLKeyPress keyPress)
        {
            if (keyPress != null && this.nextLevel < 10)
            {
                RogueSharp.Random.IRandom iRand = new RogueSharp.Random.DotNetRandom();

                // TODO: ADD A BONUS HERE THAT CARRIES OVER
                var arena = ArenaBuilder.BuildArena(Config.ArenaWidth, Config.ArenaHeight, iRand.Next(Int16.MaxValue).ToString(),
                    EntityBuilder.BuildPlayerEntity(), this.nextLevel);
                var arenaMenu = new Menu_Arena(this.mainMenu, arena);
                this.mainMenu.SetArena(arenaMenu);
                return arenaMenu;
            }
            else if (keyPress != null)
                return this.mainMenu;
            else
                return this;
        }

        public void Blit(RLConsole console)
        {
            if (this.nextLevel < 10)
            {
                console.SetBackColor(0, 0, console.Width, console.Height, RLColor.Black);
                console.Print(console.Width / 2 - 19, console.Height / 2 - 1, "You have progressed to the next arena!", RLColor.White);
                console.Print(console.Width / 2 - 10, console.Height / 2 + 1, "The next level is " + this.nextLevel, RLColor.White);
            }
            else
            {
                console.SetBackColor(0, 0, console.Width, console.Height, RLColor.Black);
                console.Print(console.Width / 2 - 15, console.Height / 2 - 1, "You have won! Congratulations!", RLColor.White);
            }
        }
    }
}
