using Executor.Tournament;
using RLNET;

namespace Executor.UI
{
    class Menu_CompetitorDetails : IDisplay
    {
        private RLConsole statusConsole;
        private IntegerSelectionField selectionField;
        private IDisplay parentDisplay;
        private ICompetitor player;
        private Schedule_Tournament tournament;
        public ICompetitor SelectedCompetitor { get; }

        public string SelectedID { get { return this.SelectedCompetitor.CompetitorID; } }

        public Menu_CompetitorDetails(IDisplay parentDisplay, ICompetitor player, Schedule_Tournament tournament,
            ICompetitor selectedCompetitor)
        {
            this.statusConsole = new RLConsole(Menu_Arena.statusWidth, Menu_Arena.statusHeight);
            this.statusConsole.SetBackColor(0, 0, Menu_Arena.statusWidth, Menu_Arena.statusHeight,
                RLColor.LightBlue);
            this.selectionField = new IntegerSelectionField();

            this.parentDisplay = parentDisplay;
            this.player = player;
            this.tournament = tournament;
            this.SelectedCompetitor = selectedCompetitor;
        }

        public IDisplay OnRootConsoleUpdate(RLConsole rootConsole, RLKeyPress keyPress)
        {
            var selectedMatch = this.selectionField.HandleKeyPress(keyPress,
                this.tournament.MatchHistory(this.SelectedID));

            if (selectedMatch != null)
            {
                if (selectedMatch.HasCompetitor(this.player.CompetitorID))
                {
                    Log.InfoLine("Can't replay player matches!");
                    this.selectionField.Reset();
                    return this;
                }
                else
                {
                    var arena = ArenaBuilder.BuildReplayArena(this.tournament, selectedMatch);
                    return new Menu_Arena(this, arena, this.tournament);
                }
            }
            else if (keyPress != null)
            {
                switch (keyPress.Key) {
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
            int tableWidth = 60;
            int currentX = 1;
            int lineStart = 3;
            int line = lineStart;

            console.SetBackColor(0, 0, console.Width, console.Height, RLColor.Black);
            console.Print(console.Width / 2 - 10, 1, "COMPETITOR HISTORY MENU", RLColor.White);

            if (tournament.IsEliminated(this.SelectedCompetitor.CompetitorID))
                console.Print(currentX, line, this.SelectedCompetitor.Label, RLColor.Red);
            else
                console.Print(currentX, line, this.SelectedCompetitor.Label, RLColor.White);
            line += 2;

            int i = 1;
            foreach (var result in tournament.MatchHistory(this.SelectedCompetitor.CompetitorID))
            {
                var resultString = i + ")" + this.SelectedCompetitor + " versus " +
                    result.OpponentOf(this.SelectedCompetitor.CompetitorID);
                if (result.Winner.CompetitorID == this.SelectedCompetitor.CompetitorID)
                    console.Print(currentX, line, resultString, RLColor.Green);
                else
                    console.Print(currentX, line, resultString, RLColor.Red);
                i++;

                line++;
                if (line > console.Height - Menu_Arena.statusHeight - 3)
                {
                    line = lineStart;
                    currentX += tableWidth;
                }
            }

            line += 3;
            console.Print(currentX, line, "Inspect", RLColor.White);
            line += 1;
            console.Print(currentX, line, "# " + this.selectionField.SelectionString, RLColor.White);

            // Status of mech
            Drawer_Mech.DrawMechStatus(((CompetitorEntity)this.SelectedCompetitor).Mech, this.statusConsole);
            RLConsole.Blit(statusConsole, 0, 0, Menu_Arena.statusWidth, Menu_Arena.statusHeight, console, 0,
                console.Height - Menu_Arena.statusHeight);
        }

    }
}
