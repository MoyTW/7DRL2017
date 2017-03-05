using RLNET;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Executor.UI
{
    class Menu_Targeting : IDisplay
    {
        private Entity target;
        private readonly IDisplay parent;
        private RLConsole targetingConsole;

        public const int targetingWidth = 60;
        public const int targetingHeight = 45;
        public readonly int x, y;

        public BodyPartLocation? TargetedLocation { get; private set; }

        public Menu_Targeting(IDisplay parent, int x, int y)
        {
            this.parent = parent;
            this.targetingConsole = new RLConsole(targetingWidth, targetingHeight);

            this.x = x;
            this.y = y;
        }

        public void SetTarget(Entity target)
        {
            this.target = target;
            this.TargetedLocation = null;
        }

        public void Reset()
        {
            this.target = null;
            this.TargetedLocation = null;
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
            this.parent.Blit(console);

            this.targetingConsole.SetBackColor(0, 0, targetingWidth, targetingHeight, RLColor.White);

            Drawer_Mech.DrawMechStatus(this.target, this.targetingConsole);
            RLConsole.Blit(this.targetingConsole, 0, 0, Menu_Arena.statusWidth, Menu_Arena.statusHeight, console,
                this.x, this.y);
        }

        private IDisplay HandleKeyPressed(RLKeyPress keyPress)
        {
            if (keyPress == null)
                throw new InvalidOperationException("Called HandleKeyPressed with null, don't do this!");

            switch (keyPress.Key)
            {
                case RLKey.Escape:
                    return this.parent;
                case RLKey.Keypad1:
                case RLKey.B:
                    this.TargetedLocation = BodyPartLocation.LEFT_LEG;
                    return this.parent;
                case RLKey.Keypad3:
                case RLKey.N:
                    this.TargetedLocation = BodyPartLocation.RIGHT_LEG;
                    return this.parent;
                case RLKey.Keypad4:
                case RLKey.H:
                case RLKey.Left:
                    this.TargetedLocation = BodyPartLocation.LEFT_ARM;
                    return this.parent;
                case RLKey.Keypad5:
                    this.TargetedLocation = BodyPartLocation.TORSO;
                    return this.parent;
                case RLKey.Keypad6:
                case RLKey.Right:
                case RLKey.L:
                    this.TargetedLocation = BodyPartLocation.RIGHT_ARM;
                    return this.parent;
                case RLKey.Keypad8:
                case RLKey.Up:
                case RLKey.K:
                    this.TargetedLocation = BodyPartLocation.HEAD;
                    return this.parent;
                default:
                    break;
            }
            return this;
        }
    }
}
