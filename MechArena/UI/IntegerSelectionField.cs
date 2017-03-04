using RLNET;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Executor.UI
{
    class IntegerSelectionField
    {
        private string selectionString = "";
        public string SelectionString { get { return this.selectionString; } }

        public T HandleKeyPress<T>(RLKeyPress keyPress, IList<T> options)
        {
            if (keyPress != null)
            {
                switch (keyPress.Key)
                {
                    case RLKey.Number0:
                    case RLKey.Keypad0:
                        this.selectionString += "0";
                        break;
                    case RLKey.Number1:
                    case RLKey.Keypad1:
                        this.selectionString += "1";
                        break;
                    case RLKey.Number2:
                    case RLKey.Keypad2:
                        this.selectionString += "2";
                        break;
                    case RLKey.Number3:
                    case RLKey.Keypad3:
                        this.selectionString += "3";
                        break;
                    case RLKey.Number4:
                    case RLKey.Keypad4:
                        this.selectionString += "4";
                        break;
                    case RLKey.Number5:
                    case RLKey.Keypad5:
                        this.selectionString += "5";
                        break;
                    case RLKey.Number6:
                    case RLKey.Keypad6:
                        this.selectionString += "6";
                        break;
                    case RLKey.Number7:
                    case RLKey.Keypad7:
                        this.selectionString += "7";
                        break;
                    case RLKey.Number8:
                    case RLKey.Keypad8:
                        this.selectionString += "8";
                        break;
                    case RLKey.Number9:
                    case RLKey.Keypad9:
                        this.selectionString += "9";
                        break;
                    case RLKey.BackSpace:
                        if (this.selectionString.Length > 0)
                            this.selectionString = this.selectionString.Substring(0, this.selectionString.Length - 1);
                        break;
                    case RLKey.Enter:
                    case RLKey.KeypadEnter:
                        int index;
                        Int32.TryParse(this.selectionString, out index);
                        index--;

                        this.selectionString = "";

                        if (index >= 0 && index < options.Count)
                            return options[index];
                        break;
                    default:
                        break;
                }
            }
            return default(T);
        }

        public void Reset()
        {
            this.selectionString = "";
        }
    }
}
