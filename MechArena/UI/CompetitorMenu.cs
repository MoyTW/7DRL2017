using MechArena.Tournament;

using RLNET;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MechArena.UI
{
    class CompetitorMenuToCompetitorHistory
    {
        public Competitor SelectedCompetitor { get; }

        public CompetitorMenuToCompetitorHistory(Competitor selection)
        {
            this.SelectedCompetitor = selection;
        }
    }

    class CompetitorMenu
    {
        private IntegerSelectionField selectionField = new IntegerSelectionField();
        private bool gotoMainMenu = false;
        private CompetitorMenuToCompetitorHistory transition;

        public bool GotoMainMenu { get { return this.gotoMainMenu; } }
        public CompetitorMenuToCompetitorHistory Transition { get { return this.transition; } }

        public void OnRootConsoleUpdate(RLRootConsole rootConsole, Schedule_Tournament tournament)
        {
            RLKeyPress keyPress = rootConsole.Keyboard.GetKeyPress();
            if (keyPress != null)
            {
                var selection = this.selectionField.HandleKeyPress(keyPress, tournament.AllCompetitors());
                if (selection != null)
                    this.transition = new CompetitorMenuToCompetitorHistory(selection);

                switch (keyPress.Key)
                {
                    case RLKey.Escape:
                        this.gotoMainMenu = true;
                        break;
                    default:
                        break;
                }
            }
        }

        public void Blit(RLConsole console, Schedule_Tournament tournament)
        {
            int tableWidth = 30;
            int currentX = 1;
            int lineStart = 3;
            int line = lineStart;

            console.SetBackColor(0, 0, console.Width, console.Height, RLColor.Black);
            console.Print(console.Width / 2 - 6, 1, "COMPETITOR MENU", RLColor.White);

            foreach (var c in tournament.AllCompetitors())
            {
                if (tournament.IsEliminated(c.CompetitorID))
                    console.Print(currentX, line, c.Label, RLColor.Red);
                else
                    console.Print(currentX, line, c.Label, RLColor.White);

                line++;
                if (line > console.Height - 3)
                {
                    line = lineStart;
                    currentX += tableWidth;
                }
            }

            line += 3;
            console.Print(currentX, line, "Inspect", RLColor.White);
            line += 1;
            console.Print(currentX, line, "# " + this.selectionField.SelectionString, RLColor.White);
        }
    }
}
