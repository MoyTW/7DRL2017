using RLNET;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Executor.UI
{
    class Menu_MechDetails : IDisplay
    {
        private const string letters = "abcdefghijklmnopqrstuvwxyz";

        private RLConsole statusConsole;
        private char? weaponSelection;

        private IDisplay parentDisplay;
        private Entity mech;
        private Dictionary<BodyPartLocation, List<Tuple<char, Entity>>> holstersDict;
        private Dictionary<BodyPartLocation, List<Tuple<char, Entity>>> mountsDict;

        public Menu_MechDetails(IDisplay parentDisplay, Entity mech)
        {
            if (!mech.HasComponentOfType<Component_Skeleton>())
                throw new ArgumentException("Entity " + mech + " passed into Menu_ArenaEquipment is not a mech!");

            this.statusConsole = new RLConsole(Menu_Arena.statusWidth, Menu_Arena.statusHeight);
            this.statusConsole.SetBackColor(0, 0, Menu_Arena.statusWidth, Menu_Arena.statusHeight,
                RLColor.LightBlue);
            this.weaponSelection = null;

            this.parentDisplay = parentDisplay;
            this.mech = mech;

            this.holstersDict = new Dictionary<BodyPartLocation, List<Tuple<char, Entity>>>();
            this.mountsDict = new Dictionary<BodyPartLocation, List<Tuple<char, Entity>>>();
            var skeleton = this.mech.GetComponentOfType<Component_Skeleton>();
            int holsterIdx = 0, mountIdx = 0;
            foreach (var location in EntityBuilder.MechLocations)
            {
                var mounts = skeleton.InspectBodyPart(location)
                    .TryGetSubEntities(SubEntitiesSelector.SWAPPABLE_ATTACH_POINTS);
                foreach (var mount in mounts)
                {
                    if (mount.GetComponentOfType<Component_AttachPoint>().Active)
                    {
                        if (!this.mountsDict.ContainsKey(location))
                            this.mountsDict.Add(location, new List<Tuple<char, Entity>>());
                        this.mountsDict[location].Add(new Tuple<char, Executor.Entity>(letters[mountIdx], mount));
                        mountIdx++;
                    }
                    else
                    {
                        if (!this.holstersDict.ContainsKey(location))
                            this.holstersDict.Add(location, new List<Tuple<char, Entity>>());
                        this.holstersDict[location].Add(new Tuple<char, Entity>(letters[holsterIdx], mount));
                        holsterIdx++;
                    }
                }
            }
        }

        // This is kind of silly, but not silly enough for me to refactor right now.
        // It's not like this silliness will leak...WILL IT!? (it won't)
        private Entity GetEntityByChar(Dictionary<BodyPartLocation, List<Tuple<char,Entity>>> d, char c)
        {
            var selected = d.Values
                .SelectMany(i => i)
                .Where(i => i.Item1 == c)
                .FirstOrDefault();

            if (selected != null)
                return selected.Item2;
            else
                return null;
        }

        public void TryStoreWeaponSelection(char weaponSelection)
        {
            if (this.GetEntityByChar(this.holstersDict, weaponSelection) != null)
                this.weaponSelection = weaponSelection;
        }

        // TODO: Pull logic of swapping out of menu code
        public static void TrySwapWeaponToMount(Entity selectedHolster, Entity selectedMount)
        {
            if (selectedMount == null || selectedHolster == null)
                return;

            var holsterMount = selectedHolster.GetComponentOfType<Component_AttachPoint>();
            var weaponFromHolster = holsterMount.InspectAttachedEntity();

            var mountMount = selectedMount.GetComponentOfType<Component_AttachPoint>();
            var weaponFromMount = mountMount.InspectAttachedEntity();

            // Whew, that's silly ugly!
            if (holsterMount.Swappable &&
                holsterMount.MaxSize >= weaponFromMount.GetComponentOfType<Component_Attachable>().SizeRequired &&
                mountMount.Swappable &&
                mountMount.MaxSize >= weaponFromHolster.GetComponentOfType<Component_Attachable>().SizeRequired)
            {
                // TODO: There's something wrong when you pass null into a constructor like this!
                selectedMount.HandleEvent(new GameEvent_Unslot(null, selectedMount, weaponFromMount));
                selectedHolster.HandleEvent(new GameEvent_Unslot(null, selectedHolster, weaponFromHolster));

                selectedMount.HandleEvent(new GameEvent_Slot(null, selectedMount, weaponFromHolster));
                selectedHolster.HandleEvent(new GameEvent_Slot(null, selectedHolster, weaponFromMount));
            }
            else
            {
                // TODO: See message
                Log.ErrorLine("Can't swap, weapons not same size! Msg should be printed on screen & removed from ERR");
            }
        }

        public IDisplay OnRootConsoleUpdate(RLConsole console, RLKeyPress keyPress)
        {
            // Explicit exit conditions
            if (keyPress == null)
                return this;
            else if (keyPress.Key == RLKey.Escape)
                return this.parentDisplay;

            // Key logic
            if (keyPress.Key == RLKey.BackSpace)
                this.weaponSelection = null;
            else if (keyPress.Char != null && char.IsLetter((char)keyPress.Char))
            {
                if (this.weaponSelection == null)
                    this.TryStoreWeaponSelection((char)keyPress.Char);
                else
                {
                    TrySwapWeaponToMount(this.GetEntityByChar(this.holstersDict, (char)this.weaponSelection),
                        this.GetEntityByChar(this.mountsDict, (char)keyPress.Char));
                    this.weaponSelection = null;
                }
            }

            return this;
        }

        private void DrawWeaponsListing(RLConsole console, int x, int y)
        {
            console.Print(x, y++, "####################", RLColor.White);
            console.Print(x, y++, "# WEAPONS LISTING  #", RLColor.White);
            console.Print(x, y++, "####################", RLColor.White);

            foreach (var entry in this.holstersDict)
            {
                console.Print(x, y, "#                  #", RLColor.White);
                console.Print(x + 2, y++, entry.Key.ToString(), RLColor.White);
                console.Print(x, y++, "#------------------#", RLColor.White);

                foreach (var holster in entry.Value)
                {
                    var mountedEntity = holster.Item2.GetComponentOfType<Component_AttachPoint>().InspectAttachedEntity();
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

            foreach (var entry in this.mountsDict)
            {
                console.Print(x, y, "#                  #", RLColor.White);
                console.Print(x + 2, y++, entry.Key.ToString(), RLColor.White);
                console.Print(x, y++, "#------------------#", RLColor.White);

                foreach (var mount in entry.Value)
                {
                    var mountedEntity = mount.Item2.GetComponentOfType<Component_AttachPoint>().InspectAttachedEntity();
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

        private void DrawSelection(RLConsole console, int x, int y)
        {
            console.Print(x, y++, "####################", RLColor.White);
            console.Print(x, y++, "# SELECTED WEAPON  #", RLColor.White);
            console.Print(x, y++, "####################", RLColor.White);
            console.Print(x, y++, "#                  #", RLColor.White);
            console.Print(x, y, "#                  #", RLColor.White);
            console.Print(x + 9, y++, this.weaponSelection.ToString(), RLColor.White);
            console.Print(x, y++, "#                  #", RLColor.White);
            console.Print(x, y++, "####################", RLColor.White);
        }

        public void Blit(RLConsole console)
        {
            Drawer_Mech.DrawMechStatus(this.mech, this.statusConsole);
            RLConsole.Blit(this.statusConsole, 0, 0, Menu_Arena.statusWidth, Menu_Arena.statusHeight, console, 0, 0);

            this.DrawSelection(console, 12, Menu_Arena.statusHeight + 1);
            this.DrawWeaponsListing(console, 1, Menu_Arena.statusHeight + 9);
            this.DrawMountsListing(console, 22, Menu_Arena.statusHeight + 9);
        }
    }
}
