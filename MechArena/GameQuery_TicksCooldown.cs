using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Executor
{
    class GameQuery_TicksCooldown : GameQuery
    {
        private int ticksCooldown;

        public int Value { get { return this.ticksCooldown; } }

        public void RegisterTicksCooldown(int ticksCooldown)
        {
            this.ticksCooldown = ticksCooldown;
            this.Completed = true;
        }
    }
}
