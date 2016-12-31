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
        public Competitor SelectedCompetitor { get; }

        private string matchSelection;
        private bool gotoCompetitorMenu = false;

        public bool GotoCompetitorMenu { get { return this.gotoCompetitorMenu; } }

        public CompetitorHistory(Competitor selectedCompetitor)
        {
            this.SelectedCompetitor = selectedCompetitor;
        }

        public void OnRootConsoleUpdate(RLRootConsole rootConsole, Schedule_Tournament tournament)
        {
            RLKeyPress keyPress = rootConsole.Keyboard.GetKeyPress();
            if (keyPress != null)
            {
                switch (keyPress.Key)
                {
                    case RLKey.Number0:
                    case RLKey.Keypad0:
                        this.matchSelection += "0";
                        break;
                    case RLKey.Number1:
                    case RLKey.Keypad1:
                        this.matchSelection += "1";
                        break;
                    case RLKey.Number2:
                    case RLKey.Keypad2:
                        this.matchSelection += "2";
                        break;
                    case RLKey.Number3:
                    case RLKey.Keypad3:
                        this.matchSelection += "3";
                        break;
                    case RLKey.Number4:
                    case RLKey.Keypad4:
                        this.matchSelection += "4";
                        break;
                    case RLKey.Number5:
                    case RLKey.Keypad5:
                        this.matchSelection += "5";
                        break;
                    case RLKey.Number6:
                    case RLKey.Keypad6:
                        this.matchSelection += "6";
                        break;
                    case RLKey.Number7:
                    case RLKey.Keypad7:
                        this.matchSelection += "7";
                        break;
                    case RLKey.Number8:
                    case RLKey.Keypad8:
                        this.matchSelection += "8";
                        break;
                    case RLKey.Number9:
                    case RLKey.Keypad9:
                        this.matchSelection += "9";
                        break;
                    case RLKey.BackSpace:
                        if (this.matchSelection.Length > 0)
                            this.matchSelection = this.matchSelection.Substring(0, this.matchSelection.Length - 1);
                        break;
                    case RLKey.Enter:
                    case RLKey.KeypadEnter:
                        int index;
                        Int32.TryParse(this.matchSelection, out index);
                        index--;
                        Console.WriteLine("Selected match #" + index);

                        this.matchSelection = "";
                        break;
                    case RLKey.Escape:
                        this.gotoCompetitorMenu = true;
                        break;
                    default:
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
                if (line > console.Height - 3)
                {
                    line = lineStart;
                    currentX += tableWidth;
                }
            }

            line += 3;
            console.Print(currentX, line, "Inspect", RLColor.White);
            line += 1;
            console.Print(currentX, line, "# " + this.matchSelection, RLColor.White);
        }

    }
}
