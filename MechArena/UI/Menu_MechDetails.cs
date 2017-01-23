using RLNET;
using System;

namespace MechArena.UI
{
    class Menu_MechDetails : IDisplay
    {
        private RLConsole statusConsole;
        private IDisplay parentDisplay;
        private Entity mech;

        public Menu_MechDetails(IDisplay parentDisplay, Entity mech)
        {
            if (!mech.HasComponentOfType<Component_MechSkeleton>())
                throw new ArgumentException("Entity " + mech + " passed into Menu_ArenaEquipment is not a mech!");

            this.statusConsole = new RLConsole(Menu_Arena.statusWidth, Menu_Arena.statusHeight);
            this.statusConsole.SetBackColor(0, 0, Menu_Arena.statusWidth, Menu_Arena.statusHeight,
                RLColor.LightBlue);

            this.parentDisplay = parentDisplay;
            this.mech = mech;
        }

        public IDisplay OnRootConsoleUpdate(RLConsole console, RLKeyPress keyPress)
        {
            if (keyPress != null)
            {
                switch (keyPress.Key)
                {
                    case RLKey.Escape:
                        return this.parentDisplay;
                    default:
                        return this;
                }
            }
            else
            {
                return this;
            }
        }

        public void Blit(RLConsole console)
        {
            Drawer_Mech.DrawMechStatus(this.mech, this.statusConsole);
            RLConsole.Blit(this.statusConsole, 0, 0, Menu_Arena.statusWidth, Menu_Arena.statusHeight, console, 0, 0);

            // TODO: Weapons Column
            int line = Menu_Arena.statusHeight + 1;
            console.Print(1, line++, "####################", RLColor.White);
            console.Print(1, line++, "# WEAPONS LISTING  #", RLColor.White);
            console.Print(1, line++, "####################", RLColor.White);

            // TODO: Mounts Column
            line = Menu_Arena.statusHeight + 1;
            console.Print(22, line++, "####################", RLColor.White);
            console.Print(22, line++, "#  MOUNTS LISTING  #", RLColor.White);
            console.Print(22, line++, "####################", RLColor.White);
        }
    }
}
