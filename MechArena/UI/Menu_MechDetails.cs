using RLNET;
using System;
using System.Linq;

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

        private void DrawWeaponsListing(RLConsole console, int x, int y)
        {
            console.Print(x, y++, "####################", RLColor.White);
            console.Print(x, y++, "# WEAPONS LISTING  #", RLColor.White);
            console.Print(x, y++, "####################", RLColor.White);

            var skeleton = this.mech.GetComponentOfType<Component_MechSkeleton>();
            foreach (var location in EntityBuilder.MechLocations)
            {
                var holsters = skeleton.InspectBodyPart(location)
                    .TryGetSubEntities(SubEntitiesSelector.HOLSTERS);
                if (holsters.Count() > 0)
                {
                    console.Print(x, y, "#                  #", RLColor.White);
                    console.Print(x + 2, y++, location.ToString(), RLColor.White);
                    console.Print(x, y++, "#------------------#", RLColor.White);

                    foreach (var holster in holsters)
                    {
                        var mountedEntity = holster.GetComponentOfType<Component_Holster>().InspectHolsteredEntity();
                        string mountedString;
                        if (mountedEntity != null)
                            mountedString = mountedEntity.ToString();
                        else
                            mountedString = "";

                        console.Print(x, y, "+                  +", RLColor.White);
                        console.Print(x + 2, y++, holster.ToString(), RLColor.White);
                        console.Print(x, y++, "| ^                |", RLColor.White);
                        console.Print(x, y, "+                  +", RLColor.White);
                        console.Print(x + 2, y++, mountedString, RLColor.White);
                        console.Print(x, y++, "|------------------|", RLColor.White);
                    }
                }
            }
        }

        private void DrawMountsListing(RLConsole console, int x, int y)
        {
            console.Print(x, y++, "####################", RLColor.White);
            console.Print(x, y++, "#  MOUNTS LISTING  #", RLColor.White);
            console.Print(x, y++, "####################", RLColor.White);

            var skeleton = this.mech.GetComponentOfType<Component_MechSkeleton>();
            foreach (var location in EntityBuilder.MechLocations)
            {
                var mounts = skeleton.InspectBodyPart(location)
                    .TryGetSubEntities(SubEntitiesSelector.SWAPPABLE_MOUNTS);
                if (mounts.Count() > 0)
                {
                    console.Print(x, y, "#                  #", RLColor.White);
                    console.Print(x + 2, y++, location.ToString(), RLColor.White);
                    console.Print(x, y++, "#------------------#", RLColor.White);

                    foreach (var mount in mounts)
                    {
                        var mountedEntity = mount.GetComponentOfType<Component_Mount>().InspectMountedEntity();
                        string mountedString;
                        if (mountedEntity != null)
                            mountedString = mountedEntity.ToString();
                        else
                            mountedString = "";

                        console.Print(x, y, "+                  +", RLColor.White);
                        console.Print(x + 2, y++, mount.ToString(), RLColor.White);
                        console.Print(x, y++, "| ^                |", RLColor.White);
                        console.Print(x, y, "+                  +", RLColor.White);
                        console.Print(x + 2, y++, mountedString, RLColor.White);
                        console.Print(x, y++, "|------------------|", RLColor.White);
                    }
                }
            }
        }

        public void Blit(RLConsole console)
        {
            Drawer_Mech.DrawMechStatus(this.mech, this.statusConsole);
            RLConsole.Blit(this.statusConsole, 0, 0, Menu_Arena.statusWidth, Menu_Arena.statusHeight, console, 0, 0);

            this.DrawWeaponsListing(console, 1, Menu_Arena.statusHeight + 1);
            this.DrawMountsListing(console, 22, Menu_Arena.statusHeight + 1);
        }
    }
}
