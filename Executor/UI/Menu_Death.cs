using RLNET;

namespace Executor.UI
{
    class Menu_Death : IDisplay
    {
        private readonly IDisplay mainMenu;
        private int level;

        public Menu_Death(IDisplay mainMenu, int level)
        {
            this.mainMenu = mainMenu;
            this.level = level;
        }

        public IDisplay OnRootConsoleUpdate(RLConsole console, RLKeyPress keyPress)
        {
            if (keyPress != null)
                return this.mainMenu;
            else
                return this;
        }

        public void Blit(RLConsole console)
        {
            console.SetBackColor(0, 0, console.Width, console.Height, RLColor.Black);
            console.Print(console.Width / 2 - 4, console.Height / 2 - 1, "YOU DIED", RLColor.White);
            console.Print(console.Width / 2 - 11, console.Height / 2 + 1, "You made it to level " + this.level, RLColor.White);
        }
    }
}
