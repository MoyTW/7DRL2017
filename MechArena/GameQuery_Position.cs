using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MechArena
{
    class GameQuery_Position : GameQuery
    {
        private int x, y;
        public int X { get { return x; } }
        public int Y { get { return y; } }

        public void RegisterPosition(int x, int y)
        {
            this.x = x;
            this.y = y;
            this.Completed = true;
        }
    }
}
