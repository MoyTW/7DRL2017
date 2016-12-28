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
        private readonly Arena arena;

        public const int arenaWidth = 50;
        public const int arenaHeight = 50;
        private RLConsole arenaConsole;

        private readonly int hudWidth = 50;
        private readonly int hudHeight = 30;
        private RLConsole hudConsole;

        private readonly int statusWidth = 125;
        private readonly int statusHeight = 40;
        private RLConsole status1Console;
        private RLConsole status2Console;

        public ArenaDrawer(Arena arena)
        {
            this.arena = arena;
            arenaConsole = new RLConsole(ArenaDrawer.arenaWidth, ArenaDrawer.arenaHeight);
            hudConsole = new RLConsole(this.hudWidth, this.hudHeight);
            status1Console = new RLConsole(this.statusWidth, this.statusHeight);
            status2Console = new RLConsole(this.statusWidth, this.statusHeight);
        }

        #region External

        public void OnRootConsoleUpdate(RLConsole console)
        {
            this.arenaConsole.SetBackColor(0, 0, ArenaDrawer.arenaWidth, ArenaDrawer.arenaHeight, RLColor.Black);
            this.arenaConsole.Print(1, 1, "Arena", RLColor.White);

            this.hudConsole.SetBackColor(0, 0, this.hudWidth, this.hudHeight, RLColor.LightGray);

            this.status1Console.SetBackColor(0, 0, this.statusWidth, this.statusHeight, RLColor.LightBlue);
            this.status2Console.SetBackColor(0, 0, this.statusWidth, this.statusHeight, RLColor.LightCyan);
        }

        public void Blit(RLConsole console)
        {
            this.arenaConsole.SetBackColor(0, 0, ArenaDrawer.arenaWidth, ArenaDrawer.arenaHeight, RLColor.Black);
            this.arenaConsole.Print(1, 1, "Arena", RLColor.White);

            this.hudConsole.SetBackColor(0, 0, this.hudWidth, this.hudHeight, RLColor.LightGray);

            this.status1Console.SetBackColor(0, 0, this.statusWidth, this.statusHeight, RLColor.LightBlue);
            this.status2Console.SetBackColor(0, 0, this.statusWidth, this.statusHeight, RLColor.LightCyan);

            this.DrawArena(this.arenaConsole);
            RLConsole.Blit(this.arenaConsole, 0, 0, ArenaDrawer.arenaWidth, ArenaDrawer.arenaHeight, console, 0, 0);

            this.DrawHUD(this.hudConsole);
            RLConsole.Blit(this.hudConsole, 0, 0, this.hudWidth, this.hudHeight, console, 0, arenaHeight);

            this.DrawMech1Status(this.status1Console);
            RLConsole.Blit(this.status1Console, 0, 0, this.statusWidth, this.statusHeight, console, ArenaDrawer.arenaWidth, 0);

            this.DrawMech2Status(this.status2Console);
            RLConsole.Blit(this.status2Console, 0, 0, this.statusWidth, this.statusHeight, console, ArenaDrawer.arenaWidth, this.statusHeight);
        }

        #endregion

        #region Drawing

        private int DrawBodyPartStatus(Entity bodyPart, int x, int y, bool mechDestroyed, RLConsole console)
        {
            var bodyPartDestroyed = bodyPart.TryGetDestroyed();
            var bodyPartStructure = bodyPart.TryGetAttribute(EntityAttributeType.STRUCTURE).Value;

            if (mechDestroyed || bodyPartDestroyed)
                console.Print(x, y, "- " + bodyPart.ToString() + ":" + bodyPartStructure + " ", RLColor.Red);
            else
                console.Print(x, y, "- " + bodyPart.ToString() + ":" + bodyPartStructure + " ", RLColor.Black);
            y += 2;

            var mountedParts = bodyPart.HandleQuery(new GameQuery_SubEntities(SubEntitiesSelector.ALL)).SubEntities;
            foreach (var mountedPart in mountedParts)
            {
                var mountedPartDestroyed = mountedPart.TryGetDestroyed();
                var structure = mountedPart.TryGetAttribute(EntityAttributeType.STRUCTURE).Value;
                if (mechDestroyed || bodyPartDestroyed || mountedPartDestroyed)
                    console.Print(x, y, "  + " + mountedPart.ToString() + ":" + structure + " ", RLColor.Red);
                else
                    console.Print(x, y, "  + " + mountedPart.ToString() + ":" + structure + " ", RLColor.Black);
                y += 2;
            }

            return y - 3;
        }

        private void DrawMechStatus(Entity mech, RLConsole console)
        {
            int line = 1;

            var mechDestroyed = mech.TryGetDestroyed();
            if (mechDestroyed)
                console.Print(1, line, mech.ToString(), RLColor.Red);
            else
                console.Print(1, line, mech.ToString(), RLColor.Black);
            line++;
            line++;

            var bodyParts = mech.HandleQuery(new GameQuery_SubEntities(SubEntitiesSelector.BODY_PART)).SubEntities;
            int x = 1;
            int y = line;
            foreach (var bodyPart in bodyParts)
            {
                if (y == line)
                {
                    y += this.DrawBodyPartStatus(bodyPart, x, y, mechDestroyed, console);
                }
                else
                {
                    this.DrawBodyPartStatus(bodyPart, x, y, mechDestroyed, console);
                    y = line;
                    x += 20;
                }

            }
        }

        public void DrawMech1Status(RLConsole console)
        {
            this.DrawMechStatus(this.arena.Mech1, console);
        }

        public void DrawMech2Status(RLConsole console)
        {
            this.DrawMechStatus(this.arena.Mech2, console);
        }

        public void DrawHUD(RLConsole console)
        {
            int line = 1;

            // HUD line
            console.Print(1, line, "##### HUD #####", RLColor.Black);
            line += 2;

            // Current turn status
            console.Print(1, line, "Next Action: " + arena.NextExecutorEntity.ToString() + "          ", RLColor.Black);
            line += 2;

            var playerTicksToLive = arena.Mech1.TryGetTicksToLive(arena.CurrentTick);
            var playerCooldown = arena.Mech1.HandleQuery(new GameQuery_TicksCooldown()).Value;
            console.Print(1, line, "Ticks to next move: " + playerTicksToLive + " [" + playerCooldown + "]    ", RLColor.Black);
            line += 2;

            foreach (var subTimeTracker in arena.Mech1.TryGetSubEntities(SubEntitiesSelector.TRACKS_TIME))
            {
                var ticksToLive = subTimeTracker.HandleQuery(new GameQuery_TicksToLive(arena.CurrentTick)).TicksToLive;
                console.Print(1, line, subTimeTracker.ToString() + " active in: " + ticksToLive + "    ", RLColor.Black);
                line += 2;
            }

            console.Print(1, line, "Current Tick: " + arena.CurrentTick + "           ", RLColor.Black);
            line += 2;
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
