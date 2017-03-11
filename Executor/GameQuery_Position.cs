using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Executor
{
    [Serializable()]
    public class GameQuery_Position : GameQuery
    {
        private int x, y;
        private bool blocksMovement;

        public int X { get { return x; } }
        public int Y { get { return y; } }
        public bool BlocksMovement { get { return blocksMovement; } }

        public void RegisterPosition(int x, int y, bool blocksMovement)
        {
            this.x = x;
            this.y = y;
            this.blocksMovement = blocksMovement;
            this.Completed = true;
        }
    }
}
