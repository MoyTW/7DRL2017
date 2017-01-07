using RLNET;
using RogueSharp;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MechArena.UI
{
    class ArenaDrawer
    {
        private readonly ArenaState arena;

        public const int arenaWidth = 50;
        public const int arenaHeight = 50;
        private RLConsole arenaConsole;

        private readonly int hudWidth = 30;
        private readonly int hudHeight = 90;
        private RLConsole hudConsole;

        public const int statusWidth = 60;
        public const int statusHeight = 45;
        private RLConsole status1Console;
        private RLConsole status2Console;

        public ArenaDrawer(ArenaState arena)
        {
            this.arena = arena;
            arenaConsole = new RLConsole(ArenaDrawer.arenaWidth, ArenaDrawer.arenaHeight);
            hudConsole = new RLConsole(this.hudWidth, this.hudHeight);
            status1Console = new RLConsole(ArenaDrawer.statusWidth, ArenaDrawer.statusHeight);
            status2Console = new RLConsole(ArenaDrawer.statusWidth, ArenaDrawer.statusHeight);
        }

        #region External

        public void OnRootConsoleUpdate(RLConsole console)
        {
            this.arenaConsole.SetBackColor(0, 0, ArenaDrawer.arenaWidth, ArenaDrawer.arenaHeight, RLColor.Black);
            this.arenaConsole.Print(1, 1, "Arena", RLColor.White);

            this.hudConsole.SetBackColor(0, 0, this.hudWidth, this.hudHeight, RLColor.LightGray);

            this.status1Console.SetBackColor(0, 0, ArenaDrawer.statusWidth, ArenaDrawer.statusHeight, RLColor.LightBlue);
            this.status2Console.SetBackColor(0, 0, ArenaDrawer.statusWidth, ArenaDrawer.statusHeight, RLColor.LightCyan);
        }

        public void Blit(RLConsole console)
        {
            this.arenaConsole.SetBackColor(0, 0, ArenaDrawer.arenaWidth, ArenaDrawer.arenaHeight, RLColor.Black);
            this.arenaConsole.Print(1, 1, "Arena", RLColor.White);

            this.hudConsole.SetBackColor(0, 0, this.hudWidth, this.hudHeight, RLColor.LightGray);

            this.status1Console.SetBackColor(0, 0, ArenaDrawer.statusWidth, ArenaDrawer.statusHeight, RLColor.LightBlue);
            this.status2Console.SetBackColor(0, 0, ArenaDrawer.statusWidth, ArenaDrawer.statusHeight, RLColor.LightCyan);

            this.DrawArena(this.arenaConsole);
            RLConsole.Blit(this.arenaConsole, 0, 0, ArenaDrawer.arenaWidth, ArenaDrawer.arenaHeight, console, 0, 0);

            this.DrawHUD(this.hudConsole);
            RLConsole.Blit(this.hudConsole, 0, 0, this.hudWidth, this.hudHeight, console,
                ArenaDrawer.arenaWidth + ArenaDrawer.statusWidth, 0);

            ArenaDrawer.DrawMechStatus(this.arena.Mech1, this.status1Console);
            RLConsole.Blit(this.status1Console, 0, 0, ArenaDrawer.statusWidth, ArenaDrawer.statusHeight, console,
                ArenaDrawer.arenaWidth, 0);

            ArenaDrawer.DrawMechStatus(this.arena.Mech2, this.status2Console);
            RLConsole.Blit(this.status2Console, 0, 0, ArenaDrawer.statusWidth, ArenaDrawer.statusHeight, console,
                ArenaDrawer.arenaWidth, ArenaDrawer.statusHeight);
        }

        #endregion

        #region Drawing

        private static int DrawBodyPartStatus(Entity bodyPart, int x, int y, bool mechDestroyed, RLConsole console)
        {
            var bodyPartDestroyed = bodyPart.TryGetDestroyed();
            var bodyPartStructure = bodyPart.TryGetAttribute(EntityAttributeType.STRUCTURE).Value;

            if (mechDestroyed || bodyPartDestroyed)
                console.Print(x, y, "    " + bodyPart.ToString() + ":" + bodyPartStructure + " ", RLColor.Red);
            else
                console.Print(x, y, "    " + bodyPart.ToString() + ":" + bodyPartStructure + " ", RLColor.Black);
            console.Print(x, ++y, " |----------------| ", RLColor.Black);
            y++;

            var mountedParts = bodyPart.HandleQuery(new GameQuery_SubEntities(SubEntitiesSelector.ALL)).SubEntities;
            foreach (var mountedPart in mountedParts)
            {
                var mountedPartDestroyed = mountedPart.TryGetDestroyed();
                var structure = mountedPart.TryGetAttribute(EntityAttributeType.STRUCTURE).Value;
                if (mechDestroyed || bodyPartDestroyed || mountedPartDestroyed)
                    console.Print(x + 3, y, mountedPart.Label + ":" + structure + " ", RLColor.Red);
                else
                    console.Print(x + 3, y, mountedPart.Label + ":" + structure + " ", RLColor.Black);

                console.Print(x+1, y, "+", RLColor.Black);
                console.Print(x+18, y, "+", RLColor.Black);

                if (mountedPart.HasComponentOfType<Component_Mount>() &&
                    mountedPart.GetComponentOfType<Component_Mount>().HasMountedEntity)
                {
                    console.Print(x, ++y, " | ^              | ", RLColor.Black);
                }
                else
                {
                    console.Print(x, ++y, " |----------------| ", RLColor.Black);
                }
                y++;
            }

            return y - 3;
        }

        public static void DrawMechStatus(Entity mech, RLConsole console)
        {
            int line = 1;

            var mechDestroyed = mech.TryGetDestroyed();
            if (mechDestroyed)
                console.Print(1, line, mech.ToString(), RLColor.Red);
            else
                console.Print(1, line, mech.ToString(), RLColor.Black);
            line++;
            line++;

            var skeleton = mech.GetComponentOfType<Component_MechSkeleton>();
            ArenaDrawer.DrawBodyPartStatus(skeleton.InspectBodyPart(BodyPartLocation.LEFT_ARM), 0, line + 2, mechDestroyed, console);
            ArenaDrawer.DrawBodyPartStatus(skeleton.InspectBodyPart(BodyPartLocation.LEFT_LEG), 0, line + 20, mechDestroyed, console);

            ArenaDrawer.DrawBodyPartStatus(skeleton.InspectBodyPart(BodyPartLocation.HEAD), 20, line, mechDestroyed, console);
            ArenaDrawer.DrawBodyPartStatus(skeleton.InspectBodyPart(BodyPartLocation.TORSO), 20, line + 12, mechDestroyed, console);

            ArenaDrawer.DrawBodyPartStatus(skeleton.InspectBodyPart(BodyPartLocation.RIGHT_ARM), 40, line + 2, mechDestroyed, console);
            ArenaDrawer.DrawBodyPartStatus(skeleton.InspectBodyPart(BodyPartLocation.RIGHT_LEG), 40, line + 20, mechDestroyed, console);
        }

        private IEnumerable<Tuple<Entity,int>> ArenaTimeTrackers()
        {
            var trackers = new List<Entity>();

            trackers.Add(this.arena.Mech1);
            trackers.AddRange(this.arena.Mech1.TryGetSubEntities(SubEntitiesSelector.TRACKS_TIME));
            trackers.Add(this.arena.Mech2);
            trackers.AddRange(this.arena.Mech2.TryGetSubEntities(SubEntitiesSelector.TRACKS_TIME));

            return trackers.Select(e => new Tuple<Entity,int>(e, e.TryGetTicksToLive(this.arena.CurrentTick)))
                .OrderBy(t => t.Item2);
        }

        public void DrawHUD(RLConsole console)
        {
            int line = 0;

            // HUD line
            console.Print(0, line,      "##############################", RLColor.Black);
            console.Print(0, ++line, "#                            #", RLColor.Black);
            console.Print(0, ++line, "#       ACTION QUEUE         #", RLColor.Black);
            console.Print(0, ++line, "#                            #", RLColor.Black);
            console.Print(0, ++line, "#     Current Tick: " + arena.CurrentTick + "           ", RLColor.Black);
            console.Print(29, line, "#", RLColor.Black);
            console.Print(0, ++line, "#                            #", RLColor.Black);
            console.Print(0, ++line, "##############################", RLColor.Black);
            console.Print(0, ++line, "#                #     #     #", RLColor.Black);
            console.Print(0, ++line, "#    ENTITY      # CD  # TTL #", RLColor.Black);
            console.Print(0, ++line, "#                #     #     #", RLColor.Black);
            console.Print(0, ++line, "##############################", RLColor.Black);
            console.Print(0, ++line, "------------------------------", RLColor.Black);
            line++;

            var trackers = this.ArenaTimeTrackers();
            foreach (var tracker in trackers)
            {
                var cd = tracker.Item1.HandleQuery(new GameQuery_TicksCooldown()).Value;
                console.Print(2, line, tracker.Item1.Label + "                  ", RLColor.Black);
                console.Print(19, line, cd + "        ", RLColor.Black);
                console.Print(25, line, tracker.Item2 + "                  ", RLColor.Black);
                console.Print(0, line, "|", RLColor.Black);
                console.Print(17, line, "|", RLColor.Black);
                console.Print(23, line, "|", RLColor.Black);
                console.Print(29, line, "|", RLColor.Black);
                line++;
                console.Print(0, line, "------------------------------", RLColor.Black);
                line++;
            }
        }

        public void DrawArena(RLConsole console)
        {
            var mech2Position = arena.Mech2.TryGetPosition();

            // Use RogueSharp to calculate the current field-of-view for the player
            var position = arena.Mech1.TryGetPosition();
            arena.ArenaMap.ComputeFov(position.X, position.Y, 50, true);

            foreach (var cell in arena.ArenaMap.GetAllCells())
            {
                // When a Cell is in the field-of-view set it to a brighter color
                if (cell.IsInFov)
                {
                    arena.ArenaMap.SetCellProperties(cell.X, cell.Y, cell.IsTransparent, cell.IsWalkable, true);
                    if (cell.IsWalkable)
                    {
                        console.Set(cell.X, cell.Y, RLColor.Gray, null, '.');
                    }
                    else
                    {
                        console.Set(cell.X, cell.Y, RLColor.LightGray, null, '#');
                    }
                }
                // If the Cell is not in the field-of-view but has been explored set it darker
                else if (cell.IsExplored)
                {
                    if (cell.IsWalkable)
                    {
                        console.Set(cell.X, cell.Y, new RLColor(30, 30, 30), null, '.');
                    }
                    else
                    {
                        console.Set(cell.X, cell.Y, RLColor.Gray, null, '#');
                    }
                }
            }

            // Set the player's symbol after the map symbol to make sure it is draw
            console.Set(position.X, position.Y, RLColor.LightGreen, null, '@');

            if (arena.Mech2.TryGetDestroyed())
                console.Set(mech2Position.X, mech2Position.Y, RLColor.LightGreen, null, 'D');
            else
                console.Set(mech2Position.X, mech2Position.Y, RLColor.LightGreen, null, 'E');
        }
    }

    #endregion
}
