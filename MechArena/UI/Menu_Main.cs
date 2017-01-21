using RLNET;
using System;
using MechArena.Tournament;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MechArena.UI
{
    class Menu_Main : IDisplay
    {
        private readonly bool _playPlayerMatches = false;

        private ICompetitor player;
        private Schedule_Tournament tournament;

        private Menu_Arena arenaMenu;

        public int Width { get; }
        public int Height { get; }

        public Menu_Main(int width, int height, ICompetitor player, Schedule_Tournament tournament)
        {
            this.Width = width;
            this.Height = height;
            this.player = player;
            this.tournament = tournament;
        }

        private Tuple<Match, ArenaState> BuildArena(Match m)
        {
            Console.WriteLine("Match : " + m + " mapID: " + m.MapID + " arenaSeed: ");
            var arena = ArenaBuilder.BuildArena(Menu_Arena.arenaWidth, Menu_Arena.arenaHeight, m.MatchID,
                this.tournament.PickMapID(), this.tournament.GenArenaSeed(), (CompetitorEntity)m.Competitor1,
                (CompetitorEntity)m.Competitor2);
            return new Tuple<Match, ArenaState>(m, arena);
        }

        private static MatchResult RunArena(Tuple<Match, ArenaState> matchAndArena)
        {
            var match = matchAndArena.Item1;
            var matchArena = matchAndArena.Item2;

            while (!matchArena.IsMatchEnded())
            {
                matchArena.TryFindAndExecuteNextCommand();
            }
            return match.BuildResult(matchArena.WinnerID(), matchArena.MapID, matchArena.ArenaSeed);
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
                // TODO: Don't just dump the info onto the console, actually display it
                // argh UI work is the *worst*!
                case RLKey.H:
                    return new Menu_CompetitorListing(this, this.tournament);
                case RLKey.M:
                    Log.InfoLine("########## UPCOMING PLAYER MATCHES ##########");
                    foreach (var m in this.tournament.ScheduledMatches(this.player.CompetitorID))
                    {
                        Log.InfoLine(m);
                    }
                    return this;
                case RLKey.T:
                    Log.DebugLine("T Pressed!");
                    Log.DebugLine("Round: " + this.tournament.RoundNum());

                    var results = this.tournament.ScheduledMatches()
                        .TakeWhile(m => !m.HasCompetitor(this.player.CompetitorID))
                        // BuildArena sequential because of the RNG draws
                        .Select(m => BuildArena(m))
                        // Setting degree of parallelism not required - default is fn of # processors already
                        .AsParallel().WithDegreeOfParallelism(Config.NumThreads())
                        .Select(ma => Task.Run(() => RunArena(ma)));

                    // Reporting happens in order
                    foreach (var task in results)
                    {
                        Log.DebugLine("Winner of " + task.Result.OriginalMatch + " is " + task.Result.Winner);
                        this.tournament.ReportResult(task.Result);
                    }

                    // TODO: Get rid of _match!
                    var match = this.tournament.NextMatch();
                    // If it's a player match, resolve it or stop
                    if (match != null && match.HasCompetitor(this.player.CompetitorID))
                    {
                        if (_playPlayerMatches)
                        {
                            Log.InfoLine("Next match is player!");
                        }
                        else
                        {
                            var playerResult = match.BuildResult(this.player.CompetitorID, "0", 0);
                            Log.InfoLine("Player wins match!");
                            this.tournament.ReportResult(playerResult);
                        }
                    }

                    if (this.tournament.NextMatch() == null)
                    {
                        Log.InfoLine("===== WINNER IS =====");
                        Log.InfoLine("Winner is " + this.tournament.Winners()[0]);
                    }

                    return this;
                case RLKey.N:
                    var nextMatch = this.tournament.NextMatch();
                    if (nextMatch != null)
                    {
                        var arena = ArenaBuilder.BuildArena(Menu_Arena.arenaWidth, Menu_Arena.arenaHeight,
                            nextMatch.MatchID, this.tournament.PickMapID(), this.tournament.GenArenaSeed(),
                            (CompetitorEntity)nextMatch.Competitor1, (CompetitorEntity)nextMatch.Competitor2);

                        this.arenaMenu = new Menu_Arena(this, arena, this.tournament);
                        return this.arenaMenu;
                    }
                    else
                    {
                        return this;
                    }
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
            console.Print(baseX - 4, baseY, "Main Menu", RLColor.White);

            console.Print(baseX - 4, baseY + 2, "Options", RLColor.White);
            console.Print(baseX - 2, baseY + 3, "N) Play Next Match", RLColor.White);
            console.Print(baseX - 2, baseY + 4, "R) Return To Game", RLColor.White);
            console.Print(baseX - 2, baseY + 5, "T) Fast-Forward Tournament", RLColor.White);
            console.Print(baseX - 2, baseY + 6, "H) View Match History", RLColor.White);
            console.Print(baseX - 2, baseY + 7, "M) View Upcoming Matches", RLColor.White);
            console.Print(baseX - 2, baseY + 8, "Esc) Quit", RLColor.White);

            console.Print(baseX - 4, baseY + 10, "Arena Keys", RLColor.White);
            console.Print(baseX - 2, baseY + 11, "Movement: NumPad, HJKLYUBN, Arrow Keys", RLColor.White);
            console.Print(baseX - 2, baseY + 12, "Fire Weapons: F", RLColor.White);
            console.Print(baseX - 2, baseY + 13, "Delay For One TU: P", RLColor.White);
            console.Print(baseX - 2, baseY + 14, "Delay Until Next Action: Space", RLColor.White);
            console.Print(baseX - 2, baseY + 15, "Delay For Full Cooldown: Enter", RLColor.White);
        }
    }
}
