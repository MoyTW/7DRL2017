using RLNET;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;

namespace Executor.UI
{
    class Menu_Arena : IDisplay
    {
        private readonly IDisplay parent;
        private readonly ArenaState arena;
        private readonly Menu_Targeting targetingMenu;
        private readonly Menu_PlanFocus planFocusMenu;

        public const int arenaWidth = 50;
        public const int arenaHeight = 50;
        private RLConsole arenaConsole;

        private readonly int hudWidth = 20;
        private readonly int hudHeight = 90;
        private RLConsole hudConsole;

        private readonly int focusListWidth = 30;
        private readonly int focusListHeight = 90;
        private RLConsole focusListConsole;

        public const int statusWidth = 60;
        public const int statusHeight = 45;
        private RLConsole status1Console;
        private RLConsole status2Console;

        public bool MatchEnded { get { return this.arena.IsMatchEnded(); } }

        public Menu_Arena(IDisplay parent, ArenaState arena)
        {
            this.parent = parent;
            this.arena = arena;
            this.targetingMenu = new Menu_Targeting(this, Config.TargetingWindowX, Config.TargetingWindowY);
            this.planFocusMenu = new Menu_PlanFocus(this, arena);

            arenaConsole = new RLConsole(Menu_Arena.arenaWidth, Menu_Arena.arenaHeight);
            hudConsole = new RLConsole(this.hudWidth, this.hudHeight);
            this.focusListConsole = new RLConsole(this.focusListWidth, this.focusListHeight);
            status1Console = new RLConsole(Menu_Arena.statusWidth, Menu_Arena.statusHeight);
            status2Console = new RLConsole(Menu_Arena.statusWidth, Menu_Arena.statusHeight);
        }

        #region IDisplay Fns

        public IDisplay OnRootConsoleUpdate(RLConsole console, RLKeyPress keyPress)
        {
            // Drawing sets
            this.arenaConsole.SetBackColor(0, 0, Menu_Arena.arenaWidth, Menu_Arena.arenaHeight, RLColor.Black);
            this.arenaConsole.Print(1, 1, "Arena", RLColor.White);

            this.hudConsole.SetBackColor(0, 0, this.hudWidth, this.hudHeight, RLColor.LightGray);

            this.status1Console.SetBackColor(0, 0, Menu_Arena.statusWidth, Menu_Arena.statusHeight, RLColor.LightBlue);
            this.status2Console.SetBackColor(0, 0, Menu_Arena.statusWidth, Menu_Arena.statusHeight, RLColor.LightCyan);

            // Logic
            if (this.arena.IsMatchEnded())
                return this.parent;
            else if (this.targetingMenu.TargetedLocation != null)
            {
                var stub = new CommandStub_PrepareAttack(this.arena.Player.EntityID, this.arena.Mech2.EntityID,
                    this.arena.Mech2.Label, (BodyPartLocation)this.targetingMenu.TargetedLocation);
                this.arena.ResolveStub(stub);
                this.targetingMenu.Reset();
                return this;
            }
            else if (this.planFocusMenu.InspectFocusCommands().Count != 0 &&
                this.arena.NextCommandEntity == this.arena.Player)
            {
                this.arena.ResolveStub(this.planFocusMenu.PopStub());
                // TODO: hahaha turns!
                Thread.Sleep(25);
                return this;
            }
            else if (keyPress != null)
                return this.HandleKeyPressed(keyPress);
            else
            {
                this.arena.TryFindAndExecuteNextCommand();
                //Thread.Sleep(50); // inelegant way of forcing games to display slow enough to spectate
                return this;
            }
        }

        public void Blit(RLConsole console)
        {
            this.arenaConsole.SetBackColor(0, 0, Menu_Arena.arenaWidth, Menu_Arena.arenaHeight, RLColor.Black);
            this.arenaConsole.Print(1, 1, "Arena", RLColor.White);

            this.status1Console.SetBackColor(0, 0, Menu_Arena.statusWidth, Menu_Arena.statusHeight, RLColor.LightBlue);
            this.status2Console.SetBackColor(0, 0, Menu_Arena.statusWidth, Menu_Arena.statusHeight, RLColor.LightCyan);

            this.DrawArena(this.arenaConsole);
            RLConsole.Blit(this.arenaConsole, 0, 0, Menu_Arena.arenaWidth, Menu_Arena.arenaHeight, console, 0, 0);

            this.hudConsole.SetBackColor(0, 0, this.hudWidth, this.hudHeight, RLColor.LightGray);
            this.DrawHUD(this.hudConsole);
            RLConsole.Blit(this.hudConsole, 0, 0, this.hudWidth, this.hudHeight, console,
                Menu_Arena.arenaWidth + Menu_Arena.statusWidth, 0);

            this.focusListConsole.SetBackColor(0, 0, this.focusListWidth, this.focusListHeight, RLColor.LightMagenta);
            this.DrawFocusList(this.focusListConsole);
            RLConsole.Blit(this.focusListConsole, 0, 0, this.focusListWidth, this.focusListHeight, console,
                Menu_Arena.arenaWidth + Menu_Arena.statusWidth + this.hudWidth, 0);

            Drawer_Mech.DrawMechStatus(this.arena.Player, this.status1Console);
            RLConsole.Blit(this.status1Console, 0, 0, Menu_Arena.statusWidth, Menu_Arena.statusHeight, console,
                Menu_Arena.arenaWidth, 0);

            Drawer_Mech.DrawMechStatus(this.arena.Mech2, this.status2Console);
            RLConsole.Blit(this.status2Console, 0, 0, Menu_Arena.statusWidth, Menu_Arena.statusHeight, console,
                Menu_Arena.arenaWidth, Menu_Arena.statusHeight);
        }

        #endregion

        private void TryPlayerMove(int dx, int dy)
        {
            var stub = new CommandStub_MoveSingle(this.arena.Player.EntityID, dx, dy);
            this.arena.ResolveStub(stub);
        }

        private IDisplay HandleKeyPressed(RLKeyPress keyPress)
        {
            if (keyPress == null)
                throw new InvalidOperationException("Called HandleKeyPressed with null, don't do this!");

            switch (keyPress.Key)
            {
                case RLKey.Escape:
                    return this.parent;
                    /*
                case RLKey.P:
                    this.arena.PlayerDelayAction(DelayDuration.SINGLE_TICK);
                    break;
                    */
                case RLKey.E:
                    // Technically, this doesn't limit it to the player mech.
                    return new Menu_MechDetails(this, this.arena.Player);
                case RLKey.A:
                    this.targetingMenu.SetTarget(this.arena.Mech2);
                    return this.targetingMenu;
                case RLKey.F:
                    this.planFocusMenu.ResetFocusPlan();
                    return this.planFocusMenu;
                case RLKey.Keypad1:
                case RLKey.B:
                    this.TryPlayerMove(-1, 1);
                    break;
                case RLKey.Keypad2:
                case RLKey.Down:
                case RLKey.J:
                    this.TryPlayerMove(0, 1);
                    break;
                case RLKey.Keypad3:
                case RLKey.N:
                    this.TryPlayerMove(1, 1);
                    break;
                case RLKey.Keypad4:
                case RLKey.H:
                case RLKey.Left:
                    this.TryPlayerMove(-1, 0);
                    break;
                case RLKey.Keypad6:
                case RLKey.Right:
                case RLKey.L:
                    this.TryPlayerMove(1, 0);
                    break;
                case RLKey.Keypad7:
                case RLKey.Y:
                    this.TryPlayerMove(-1, -1);
                    break;
                case RLKey.Keypad8:
                case RLKey.Up:
                case RLKey.K:
                    this.TryPlayerMove(0, -1);
                    break;
                case RLKey.Keypad9:
                case RLKey.U:
                    this.TryPlayerMove(1, -1);
                    break;
                default:
                    break;
            }
            return this;
        }

        #region Drawing

        private IEnumerable<Tuple<Entity,int>> ArenaTimeTrackers()
        {
            var trackers = new List<Entity>();

            trackers.Add(this.arena.Player);
            trackers.Add(this.arena.Mech2);

            return trackers.Select(e => new Tuple<Entity,int>(e, e.TryGetTicksToLive(this.arena.CurrentTick)))
                .OrderBy(t => t.Item2);
        }

        public void DrawFocusList(RLConsole console)
        {
            int line = 0;
            console.Print(0, line,   "##############################", RLColor.Black);
            console.Print(0, ++line, "#                            #", RLColor.Black);
            console.Print(0, ++line, "#        FOCUS STATUS        #", RLColor.Black);
            console.Print(0, ++line, "#                            #", RLColor.Black);
            line++;
            if (this.planFocusMenu.InspectFocusCommands().Count != 0)
            {
                console.Print(0, line, "#", RLColor.Black);
                console.Print(7, line, "Turn: " + this.planFocusMenu.EndTick + "           ", RLColor.Black);
                console.Print(18, line, "AP: " + this.planFocusMenu.RemainingAP + "           ", RLColor.Black);
                console.Print(29, line, "#", RLColor.Black);
            }
            else
            {
                console.Print(0, line, "#        NOT PLANNING        #", RLColor.Black);
            }
            console.Print(0, ++line, "#                            #", RLColor.Black);
            console.Print(0, ++line, "##############################", RLColor.Black);


            console.Print(0, line,   "##############################", RLColor.Black);
            console.Print(0, ++line, "#                            #", RLColor.Black);
            console.Print(0, ++line, "#         FOCUS LIST         #", RLColor.Black);
            console.Print(0, ++line, "#                            #", RLColor.Black);
            console.Print(0, ++line, "##############################", RLColor.Black);
            console.Print(0, ++line, "+----------------------------+", RLColor.Black);
            line++;

            foreach (var stub in this.planFocusMenu.InspectFocusCommands())
            {
                console.Print(2, line, stub.ToString() + "                  ", RLColor.Black);
                console.Print(0, line, "|", RLColor.Black);
                console.Print(29, line, "|", RLColor.Black);
                line++;
                console.Print(0, line, "+----------------------------+", RLColor.Black);
                line++;
            }

            // This is awkward. I'm not sure how to best do it better. Alas I am time-limited.
            // I mean, it's not very efficient, but at this scale the inefficiency? Literally too small to notice.
            for (int i = this.focusListHeight; i > line - 1; i--)
            {
                console.Print(0, i, "                              ", RLColor.Black);
            }
        }

        public void DrawHUD(RLConsole console)
        {
            int line = 0;

            // HUD line
            console.Print(0, line,   "####################", RLColor.Black);
            console.Print(0, ++line, "#                  #", RLColor.Black);
            console.Print(0, ++line, "#    TURN ORDER    #", RLColor.Black);
            console.Print(0, ++line, "#                  #", RLColor.Black);
            console.Print(0, ++line, "#     Turn: " + arena.CurrentTick + "           ", RLColor.Black);
            console.Print(19, line, "#", RLColor.Black);
            console.Print(0, ++line, "#                  #", RLColor.Black);
            console.Print(0, ++line, "####################", RLColor.Black);
            console.Print(0, ++line, "+------------------+", RLColor.Black);
            line++;

            var trackers = this.ArenaTimeTrackers();
            foreach (var tracker in trackers)
            {
                var cd = tracker.Item1.HandleQuery(new GameQuery_TicksCooldown()).Value;
                console.Print(2, line, tracker.Item1.Label + "                  ", RLColor.Black);
                console.Print(0, line, "|", RLColor.Black);
                console.Print(19, line, "|", RLColor.Black);
                line++;
                console.Print(0, line, "+------------------+", RLColor.Black);
                line++;
            }
        }

        public void DrawArena(RLConsole console)
        {
            var mech2Position = arena.Mech2.TryGetPosition();

            // Use RogueSharp to calculate the current field-of-view for the player
            var position = arena.Player.TryGetPosition();
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
