using RLNET;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Executor.UI
{
    class Menu_Targeting : IDisplay
    {
        private readonly IDisplay parent;
        private RLConsole targetingConsole;

        public const int targetingWidth = 60;
        public const int targetingHeight = 45;
        public readonly int x, y;

        public int TargetedX { get; private set; }
        public int TargetedY { get; private set; }
        public BodyPartLocation? TargetedLocation { get; private set; }
        public bool CompletedDirection { get { return !(this.TargetedX == 0 && this.TargetedY == 0); } }
        public bool CompletedLocation { get { return this.TargetedLocation != null; } }
        public bool CompletedTargeting { get { return this.CompletedDirection && this.CompletedLocation; } }

        public Menu_Targeting(IDisplay parent, int x, int y)
        {
            this.parent = parent;
            this.targetingConsole = new RLConsole(targetingWidth, targetingHeight);

            this.x = x;
            this.y = y;
        }

        public void Reset()
        {
            this.TargetedX = 0;
            this.TargetedY = 0;
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

            this.DrawTargetingMenu(this.targetingConsole);
            RLConsole.Blit(this.targetingConsole, 0, 0, Menu_Arena.statusWidth, Menu_Arena.statusHeight, console,
                this.x, this.y);
        }

        private void TargetDirection(int x, int y)
        {
            this.TargetedX = x;
            this.TargetedY = y;
        }

        private void HandleTargetDirection(RLKeyPress keyPress)
        {
            switch (keyPress.Key)
            {
                case RLKey.Keypad1:
                case RLKey.B:
                    this.TargetDirection(-1, 1);
                    break;
                case RLKey.Keypad2:
                case RLKey.Down:
                case RLKey.J:
                    this.TargetDirection(0, 1);
                    break;
                case RLKey.Keypad3:
                case RLKey.N:
                    this.TargetDirection(1, 1);
                    break;
                case RLKey.Keypad4:
                case RLKey.H:
                case RLKey.Left:
                    this.TargetDirection(-1, 0);
                    break;
                case RLKey.Keypad6:
                case RLKey.Right:
                case RLKey.L:
                    this.TargetDirection(1, 0);
                    break;
                case RLKey.Keypad7:
                case RLKey.Y:
                    this.TargetDirection(-1, -1);
                    break;
                case RLKey.Keypad8:
                case RLKey.Up:
                case RLKey.K:
                    this.TargetDirection(0, -1);
                    break;
                case RLKey.Keypad9:
                case RLKey.U:
                    this.TargetDirection(1, -1);
                    break;
                default:
                    break;
            }
        }

        private void HandleTargetLocation(RLKeyPress keyPress)
        {
            switch (keyPress.Key)
            {
                case RLKey.Keypad1:
                case RLKey.B:
                    this.TargetedLocation = BodyPartLocation.LEFT_LEG;
                    break;
                case RLKey.Keypad3:
                case RLKey.N:
                    this.TargetedLocation = BodyPartLocation.RIGHT_LEG;
                    break;
                case RLKey.Keypad4:
                case RLKey.H:
                case RLKey.Left:
                    this.TargetedLocation = BodyPartLocation.LEFT_ARM;
                    break;
                case RLKey.Keypad2:
                case RLKey.Down:
                case RLKey.J:
                case RLKey.Keypad5:
                    this.TargetedLocation = BodyPartLocation.TORSO;
                    break;
                case RLKey.Keypad6:
                case RLKey.Right:
                case RLKey.L:
                    this.TargetedLocation = BodyPartLocation.RIGHT_ARM;
                    break;
                case RLKey.Keypad8:
                case RLKey.Up:
                case RLKey.K:
                    this.TargetedLocation = BodyPartLocation.HEAD;
                    break;
                default:
                    break;
            }
        }

        private IDisplay HandleKeyPressed(RLKeyPress keyPress)
        {
            if (keyPress == null)
                throw new InvalidOperationException("Called HandleKeyPressed with null, don't do this!");

            switch (keyPress.Key)
            {
                case RLKey.Escape:
                    return this.parent;
            }

            if (!this.CompletedDirection)
                this.HandleTargetDirection(keyPress);
            else
                this.HandleTargetLocation(keyPress);

            if (this.CompletedTargeting)
                return this.parent;
            else
                return this;
        }

        private void DrawTargetingMenu(RLConsole console)
        {
            int centerX = console.Width / 2;
            int centerY = console.Height / 2;

            for (int tx = -1; tx < 2; tx++)
            {
                for (int ty = -1; ty < 2; ty++)
                {
                    console.Print(centerX - 5 + tx * 4, centerY + ty * 4, " ", RLColor.White);
                }
            }

            console.Print(centerX - 7, centerY - 2, "\\ | /", RLColor.Black);
            console.Print(centerX - 7, centerY, "- @ -", RLColor.Black);
            console.Print(centerX - 7, centerY + 2, "/ | \\", RLColor.Black);

            console.Print(centerX + 3, centerY - 2, "  H  ", RLColor.Black);
            console.Print(centerX + 3, centerY,     "A T A", RLColor.Black);
            console.Print(centerX + 3, centerY + 2, "L   L", RLColor.Black);

            if (!this.CompletedDirection)
            {
                console.Print(centerX - 9, centerY - 6, "Select A Direction              ", RLColor.Black);
            }
            else if (this.CompletedDirection)
            {
                console.Print(centerX - 9, centerY - 6, "Select A Body Part              ", RLColor.Black);
                console.Print(centerX - 5 + this.TargetedX * 4, centerY + this.TargetedY * 4, "#", RLColor.Green);
            }

            console.Print(centerX - 13, centerY + 6, "Note: Target with move key", RLColor.Black);
        }
    }
}
