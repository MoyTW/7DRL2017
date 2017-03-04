using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Executor
{
    class GameQuery_TicksToLive : GameQuery
    {
        private int ticksToLive;

        public int CurrentTick { get; }
        public int TicksToLive {
            get
            {
                if (!this.Completed)
                    throw new ArgumentException("TicksToLive dropped through/was not resolved!");
                return ticksToLive;
            }
        }
        public bool IsLive
        {
            get
            {
                if (!this.Completed)
                    throw new ArgumentException("TicksToLive dropped through/was not resolved!");
                return ticksToLive == 0;
            }
        }

        public GameQuery_TicksToLive(int currentTick)
        {
            this.CurrentTick = currentTick;
        }

        public void RegisterTicksToLive(int ticksToLive)
        {
            this.ticksToLive = ticksToLive;
            if (this.ticksToLive < 0)
                this.ticksToLive = 0;
            this.Completed = true;
        }
    }
}
