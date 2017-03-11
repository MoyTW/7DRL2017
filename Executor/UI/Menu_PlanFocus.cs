using RLNET;
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
        public int StartTick { get; private set; }
        public int EndTick { get; private set; }
        public int RemainingFreeMoves {
            get
            {
                return this.copyArena.Player.GetComponentOfType<Component_FocusUser>().CurrentFreeMoves;
            }
        }
        public int RemainingAP { 
            get
            { 
                return this.copyArena.Player.TryGetAttribute(EntityAttributeType.CURRENT_AP).Value; 
            } 
        }

        private readonly Menu_Targeting targetingMenu;
        private List<Tuple<CommandStub,GameQuery_Position>> focusCommands;

        public Menu_PlanFocus(IDisplay parent, ArenaState arena)
        {
            this.parent = parent;
            this.arena = arena;

            this.targetingMenu = new Menu_Targeting(this, Config.TargetingWindowX, Config.TargetingWindowY);
            this.focusCommands = new List<Tuple<CommandStub,GameQuery_Position>>();
        }

        public IEnumerable<CommandStub> InspectFocusCommands()
        {
            return this.focusCommands.Select(t => t.Item1);
        }

        public IEnumerable<GameQuery_Position> InspectFocusPath()
        {
            return this.focusCommands.Select(t => t.Item2);
        }

        public void Reset()
        {
            this.focusCommands.Clear();
        }

        public void ResetFocusPlan()
        {
            this.copyArena = arena.DeepCopy();
            this.copyArena.RemoveAllAIEntities();
            this.StartTick = arena.CurrentTick;
            this.focusCommands.Clear();
            this.QueueStub(new CommandStub_FocusBegin(this.arena.Player.EntityID));
        }

        public void FinalizeFocusPlan()
        {
            this.QueueStub(new CommandStub_FocusEnd(this.arena.Player.EntityID));
        }

        public CommandStub LastStub()
        {
            return this.focusCommands.Last().Item1;
        }

        public CommandStub PopStub()
        {
            var tuple = this.focusCommands[0];
            this.focusCommands.RemoveAt(0);
            return tuple.Item1;
        }

        private void QueueStub(CommandStub stub)
        {
            this.copyArena.ResolveStub(stub);
            while (!this.copyArena.ShouldWaitForPlayerInput)
            {
                this.copyArena.TryFindAndExecuteNextCommand();
            }
            this.EndTick = this.copyArena.CurrentTick;

            var tuple = new Tuple<CommandStub,GameQuery_Position>(stub, this.copyArena.Player.TryGetPosition());
            this.focusCommands.Add(tuple);
        }

        private void QueueMoveCommand(int dx, int dy)
        {
            this.QueueStub(new CommandStub_MoveSingle(this.arena.Player.EntityID, dx, dy));
        }

        private void UndoLastCommand()
        {
            if (this.focusCommands.Count == 1)
                return;
            else
            {
                var holder = new List<Tuple<CommandStub,GameQuery_Position>>(this.focusCommands);
                this.ResetFocusPlan();
                holder.RemoveAt(0);
                holder.RemoveAt(holder.Count - 1);
                foreach (var tuple in holder)
                {
                    Console.WriteLine("Holder: " + tuple.Item1);
                    this.QueueStub(tuple.Item1);
                }
            }
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
                case RLKey.BackSpace:
                    this.UndoLastCommand();
                    break;
                case RLKey.Enter:
                case RLKey.KeypadEnter:
                    this.FinalizeFocusPlan();
                    return this.parent;
                // Actions
                case RLKey.Space:
                    this.QueueStub(new CommandStub_Delay(this.arena.Player.EntityID, 1));
                    break;
                case RLKey.A:
                    this.targetingMenu.Reset();
                    return this.targetingMenu;
                case RLKey.D:
                    var blockEffect = new StatusEffect_Blocking(1, 3);
                    this.QueueStub(new CommandStub_ReceiveStatusEffect(this.arena.Player.EntityID,
                            this.arena.Player.Label, this.arena.Player.EntityID, blockEffect, APCost: Config.ONE));
                    break;
                // Movement
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
            if (this.targetingMenu.CompletedTargeting)
            {
                this.QueueStub(new CommandStub_PrepareDirectionalAttack(this.arena.Player.EntityID,
                    this.targetingMenu.TargetedX, this.targetingMenu.TargetedY,
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
