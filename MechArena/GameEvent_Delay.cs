using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MechArena
{
    class GameEvent_Delay : GameEvent_Command
    {
        public int DelayTicks { get { return 1; } }

        public GameEvent_Delay(Entity commandEntity) : base(commandEntity) { }
        public GameEvent_Delay(Entity commandEntity, Entity executorEntity) : base(commandEntity, executorEntity) { }
    }
}
