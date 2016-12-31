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
        public Schedule_Tournament Tournament { get; }
        public Competitor SelectedCompetitor { get; }

        public CompetitorMenuToCompetitorHistory(Schedule_Tournament tournament, Competitor selection)
        {
            this.Tournament = tournament;
            this.SelectedCompetitor = selection;
        }
    }

    class CompetitorMenu
    {
        private string historySelection;
        private bool gotoMainMenu = false;
        private CompetitorMenuToCompetitorHistory transition;

        public bool GotoMainMenu { get { return this.gotoMainMenu; } }
        public CompetitorMenuToCompetitorHistory Transition { get { return this.transition; } }

        public void OnRootConsoleUpdate(RLRootConsole rootConsole, Schedule_Tournament tournament)
        {
            RLKeyPress keyPress = rootConsole.Keyboard.GetKeyPress();
            if (keyPress != null)
            {
                switch (keyPress.Key)
                {
                    case RLKey.Number0:
                    case RLKey.Keypad0:
                        this.historySelection += "0";
                        break;
                    case RLKey.Number1:
                    case RLKey.Keypad1:
                        this.historySelection += "1";
                        break;
                    case RLKey.Number2:
                    case RLKey.Keypad2:
                        this.historySelection += "2";
                        break;
                    case RLKey.Number3:
                    case RLKey.Keypad3:
                        this.historySelection += "3";
                        break;
                    case RLKey.Number4:
                    case RLKey.Keypad4:
                        this.historySelection += "4";
                        break;
                    case RLKey.Number5:
                    case RLKey.Keypad5:
                        this.historySelection += "5";
                        break;
                    case RLKey.Number6:
                    case RLKey.Keypad6:
                        this.historySelection += "6";
                        break;
                    case RLKey.Number7:
                    case RLKey.Keypad7:
                        this.historySelection += "7";
                        break;
                    case RLKey.Number8:
                    case RLKey.Keypad8:
                        this.historySelection += "8";
                        break;
                    case RLKey.Number9:
                    case RLKey.Keypad9:
                        this.historySelection += "9";
                        break;
                    case RLKey.BackSpace:
                        if (this.historySelection.Length > 0)
                            this.historySelection = this.historySelection.Substring(0, this.historySelection.Length - 1);
                        break;
                    case RLKey.Enter:
                    case RLKey.KeypadEnter:
                        int index;
                        Int32.TryParse(this.historySelection, out index);
                        index--;
                        var comps = tournament.AllCompetitors();
                        if (index >= 0 && index < comps.Count)
                        {
                            var selection = tournament.AllCompetitors()[index];
                            this.transition = new CompetitorMenuToCompetitorHistory(tournament, selection);
                        }
                        else
                        {
                            Log.InfoLine("No such competitor #" + index);
                        }

                        this.historySelection = "";
                        break;
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
            console.Print(currentX, line, "# " + this.historySelection, RLColor.White);
        }
    }
}
