﻿using RLNET;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Executor.UI
{
    class Menu_PlanFocus : IDisplay
    {
        private readonly IDisplay parent;
        private readonly ArenaState arena;
        private ArenaState copyArena;

        private readonly Menu_Targeting targetingMenu;
        private List<CommandStub> focusCommands;

        public Menu_PlanFocus(IDisplay parent, ArenaState arena)
        {
            this.parent = parent;
            this.arena = arena;
            this.copyArena = arena.DeepCopy();

            this.targetingMenu = new Menu_Targeting(this, Config.TargetingWindowX, Config.TargetingWindowY);
            this.focusCommands = new List<CommandStub>();
        }

        public IList<CommandStub> InspectFocusCommands()
        {
            return this.focusCommands.AsReadOnly();
        }

        public void ResetFocusPlan()
        {
            this.copyArena = arena.DeepCopy();
            this.focusCommands.Clear();
            this.focusCommands.Add(new CommandStub_FocusBegin(this.arena.Player));
        }

        public void FinalizeFocusPlan()
        {
            this.focusCommands.Add(new CommandStub_FocusEnd(this.arena.Player));
        }

        public CommandStub PopStub()
        {
            var stub = this.focusCommands[0];
            this.focusCommands.RemoveAt(0);
            return stub;
        }

        private void QueueMoveCommand(int dx, int dy)
        {
            this.focusCommands.Add(new CommandStub_MoveSingle(this.arena.Player, dx, dy));
        }

        private IDisplay HandleKeyPressed(RLKeyPress keyPress)
        {
            if (keyPress == null)
                throw new InvalidOperationException("Called HandleKeyPressed with null, don't do this!");

            switch (keyPress.Key)
            {
                case RLKey.Escape:
                    this.focusCommands.Clear();
                    return this.parent;
                case RLKey.Enter:
                case RLKey.KeypadEnter:
                    this.FinalizeFocusPlan();
                    return this.parent;
                case RLKey.A:
                    this.targetingMenu.SetTarget(this.arena.Mech2);
                    return this.targetingMenu;
                case RLKey.Keypad1:
                case RLKey.B:
                    this.QueueMoveCommand(-1, 1);
                    break;
                case RLKey.Keypad2:
                case RLKey.Down:
                case RLKey.J:
                    this.QueueMoveCommand(0, 1);
                    break;
                case RLKey.Keypad3:
                case RLKey.N:
                    this.QueueMoveCommand(1, 1);
                    break;
                case RLKey.Keypad4:
                case RLKey.H:
                case RLKey.Left:
                    this.QueueMoveCommand(-1, 0);
                    break;
                case RLKey.Keypad6:
                case RLKey.Right:
                case RLKey.L:
                    this.QueueMoveCommand(1, 0);
                    break;
                case RLKey.Keypad7:
                case RLKey.Y:
                    this.QueueMoveCommand(-1, -1);
                    break;
                case RLKey.Keypad8:
                case RLKey.Up:
                case RLKey.K:
                    this.QueueMoveCommand(0, -1);
                    break;
                case RLKey.Keypad9:
                case RLKey.U:
                    this.QueueMoveCommand(1, -1);
                    break;
                default:
                    break;
            }

            return this;
        }

        public IDisplay OnRootConsoleUpdate(RLConsole console, RLKeyPress keyPress)
        {
            if (this.targetingMenu.TargetedLocation != null)
            {
                this.focusCommands.Add(new CommandStub_PrepareAttack(this.arena.Player, this.arena.Mech2,
                    (BodyPartLocation)this.targetingMenu.TargetedLocation));
                this.targetingMenu.Reset();
            }

            if (keyPress != null)
                return this.HandleKeyPressed(keyPress);
            else
                return this;
        }

        public void Blit(RLConsole console)
        {
            this.parent.Blit(console);
        }
    }
}
