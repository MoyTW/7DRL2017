using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Executor
{
    class GameEvent_Delay : GameEvent_Command
    {
        public int DelayTicks { get; }
 
        public GameEvent_Delay(int commandTick, int APCost, Entity commandEntity)
            : base(commandTick, APCost, commandEntity)
        { }
    }
}
