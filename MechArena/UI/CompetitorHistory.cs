using MechArena.Tournament;

using RLNET;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MechArena.UI
{
    class CompetitorHistory
    {
        private RLConsole statusConsole;
        private IntegerSelectionField selectionField;

        public Competitor SelectedCompetitor { get; }
        public string SelectedID { get { return this.SelectedCompetitor.CompetitorID; } }

        private MatchResult selectedMatch;
        private bool gotoCompetitorMenu = false;

        public bool GotoCompetitorMenu { get { return this.gotoCompetitorMenu; } }
        public MatchResult SelectedMatch { get { return this.selectedMatch; } }

        public CompetitorHistory(Competitor selectedCompetitor)
        {
            this.statusConsole = new RLConsole(ArenaDrawer.statusWidth, ArenaDrawer.statusHeight);
            this.statusConsole.SetBackColor(0, 0, ArenaDrawer.statusWidth, ArenaDrawer.statusHeight,
                RLColor.LightBlue);
            this.selectionField = new IntegerSelectionField();
            this.SelectedCompetitor = selectedCompetitor;
        }

        public void ResetCompetitorHistory()
        {
            this.selectionField.Reset();
            this.selectedMatch = null;
            this.gotoCompetitorMenu = false;
        }

        public void OnRootConsoleUpdate(RLRootConsole rootConsole, Schedule_Tournament tournament)
        {
            RLKeyPress keyPress = rootConsole.Keyboard.GetKeyPress();
            this.selectedMatch = this.selectionField.HandleKeyPress(keyPress,
                tournament.MatchHistory(this.SelectedID));

            if (keyPress != null)
            {
                switch (keyPress.Key) {
                    case RLKey.Escape:
                        this.gotoCompetitorMenu = true;
                        break;
                }
            }
        }

        public void Blit(RLConsole console, Schedule_Tournament tournament)
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
                if (line > console.Height - ArenaDrawer.statusHeight - 3)
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
            ArenaDrawer.DrawMechStatus(((CompetitorEntity)this.SelectedCompetitor).Mech, this.statusConsole);
            RLConsole.Blit(statusConsole, 0, 0, ArenaDrawer.statusWidth, ArenaDrawer.statusHeight, console, 0,
                console.Height - ArenaDrawer.statusHeight);
        }

    }
}
