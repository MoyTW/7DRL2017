using RLNET;
using System;
using System.Collections.Generic;
using System.Linq;

namespace MechArena.UI
{
    class Menu_MechDetails : IDisplay
    {
        private const string letters = "abcdefghijklmnopqrstuvwxyz";

        private RLConsole statusConsole;
        private IDisplay parentDisplay;
        private Entity mech;
        private Dictionary<BodyPartLocation, List<Tuple<char, Entity>>> holstersDict;
        private Dictionary<BodyPartLocation, List<Tuple<char, Entity>>> mountsDict;

        public Menu_MechDetails(IDisplay parentDisplay, Entity mech)
        {
            if (!mech.HasComponentOfType<Component_MechSkeleton>())
                throw new ArgumentException("Entity " + mech + " passed into Menu_ArenaEquipment is not a mech!");

            this.statusConsole = new RLConsole(Menu_Arena.statusWidth, Menu_Arena.statusHeight);
            this.statusConsole.SetBackColor(0, 0, Menu_Arena.statusWidth, Menu_Arena.statusHeight,
                RLColor.LightBlue);

            this.parentDisplay = parentDisplay;
            this.mech = mech;

            this.holstersDict = new Dictionary<BodyPartLocation, List<Tuple<char, Entity>>>();
            this.mountsDict = new Dictionary<BodyPartLocation, List<Tuple<char, Entity>>>();
            var skeleton = this.mech.GetComponentOfType<Component_MechSkeleton>();
            int holsterIdx = 0, mountIdx = 0;
            foreach (var location in EntityBuilder.MechLocations)
            {
                var holsters = skeleton.InspectBodyPart(location).TryGetSubEntities(SubEntitiesSelector.HOLSTERS);
                foreach (var holster in holsters)
                {
                    if (!this.holstersDict.ContainsKey(location))
                        this.holstersDict.Add(location, new List<Tuple<char, Entity>>());
                    this.holstersDict[location].Add(new Tuple<char, Entity>(letters[holsterIdx], holster));
                    holsterIdx++;
                }
                var mounts = skeleton.InspectBodyPart(location)
                    .TryGetSubEntities(SubEntitiesSelector.SWAPPABLE_MOUNTS);
                foreach (var mount in mounts)
                {
                    if (!this.mountsDict.ContainsKey(location))
                        this.mountsDict.Add(location, new List<Tuple<char, Entity>>());
                    this.mountsDict[location].Add(new Tuple<char, MechArena.Entity>(letters[mountIdx], mount));
                    mountIdx++;
                }
            }
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
            foreach (var entry in this.holstersDict)
            {
                console.Print(x, y, "#                  #", RLColor.White);
                console.Print(x + 2, y++, entry.Key.ToString(), RLColor.White);
                console.Print(x, y++, "#------------------#", RLColor.White);

                foreach (var holster in entry.Value)
                {
                    var mountedEntity = holster.Item2.GetComponentOfType<Component_Holster>().InspectHolsteredEntity();
                    string mountedString;
                    if (mountedEntity != null)
                        mountedString = mountedEntity.ToString();
                    else
                        mountedString = "";

                    console.Print(x, y, "+                  +", RLColor.White);
                    console.Print(x + 5, y++, holster.Item2.ToString(), RLColor.White);
                    console.Print(x, y++, "| " + holster.Item1 + ") ^             |", RLColor.White);
                    console.Print(x, y, "+                  +", RLColor.White);
                    console.Print(x + 5, y++, mountedString, RLColor.White);
                    console.Print(x, y++, "|------------------|", RLColor.White);
                }
            }
        }

        private void DrawMountsListing(RLConsole console, int x, int y)
        {
            console.Print(x, y++, "####################", RLColor.White);
            console.Print(x, y++, "#  MOUNTS LISTING  #", RLColor.White);
            console.Print(x, y++, "####################", RLColor.White);

            var skeleton = this.mech.GetComponentOfType<Component_MechSkeleton>();
            foreach (var entry in this.mountsDict)
            {
                console.Print(x, y, "#                  #", RLColor.White);
                console.Print(x + 2, y++, entry.Key.ToString(), RLColor.White);
                console.Print(x, y++, "#------------------#", RLColor.White);

                foreach (var mount in entry.Value)
                {
                    var mountedEntity = mount.Item2.GetComponentOfType<Component_Mount>().InspectMountedEntity();
                    string mountedString;
                    if (mountedEntity != null)
                        mountedString = mountedEntity.ToString();
                    else
                        mountedString = "";

                    console.Print(x, y, "+                  +", RLColor.White);
                    console.Print(x + 5, y++, mount.Item2.ToString(), RLColor.White);
                    console.Print(x, y++, "| " + mount.Item1 + ") ^             |", RLColor.White);
                    console.Print(x, y, "+                  +", RLColor.White);
                    console.Print(x + 5, y++, mountedString, RLColor.White);
                    console.Print(x, y++, "|------------------|", RLColor.White);
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
