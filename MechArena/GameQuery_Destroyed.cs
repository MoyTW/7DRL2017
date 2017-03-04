using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Executor
{
    public class GameQuery_Destroyed : GameQuery
    {
        private bool destroyed;
        public bool Destroyed { get { return this.destroyed; } }

        public GameQuery_Destroyed()
        {
            this.destroyed = false;
        }

        public void RegisterDestroyed()
        {
            this.destroyed = true;
            this.Completed = true;
        }
    }
}
