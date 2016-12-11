using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MechArena
{
    class Component_Position : Component
    {
        private int x, y;

        public int X { get { return this.x; } }
        public int Y { get { return this.y; } }

        public Component_Position(int x, int y)
        {
            this.x = x;
            this.y = y;
        }

        private void HandleMove(GameEvent_MoveSingle ev)
        {
            this.x += (int)ev.X;
            this.y += (int)ev.Y;
            ev.Completed = true;
        }

        protected override GameEvent _HandleEvent(GameEvent ev)
        {
            if (ev is GameEvent_MoveSingle)
                this.HandleMove((GameEvent_MoveSingle)ev);

            return ev;
        }

        private void HandleQueryPosition(GameQuery_Position q)
        {
            q.RegisterPosition(this.x, this.y);
        }

        protected override GameQuery _HandleQuery(GameQuery q)
        {
            if (q is GameQuery_Position)
                this.HandleQueryPosition((GameQuery_Position)q);

            return q;
        }
    }
}
