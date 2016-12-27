using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MechArena
{
    public enum DelayDuration
    {
        SINGLE_TICK = 0,
        FULL_INTERVAL
    }
    class GameEvent_Delay : GameEvent_Command
    {
        public int DelayTicks { get; }
        public DelayDuration Duration { get; }

        public GameEvent_Delay(Entity delayEntity, DelayDuration duration)
            : base(delayEntity, delayEntity)
        {
            this.Duration = duration;
            if (this.Duration == DelayDuration.SINGLE_TICK)
            {
                this.DelayTicks = 1;
            }
            else
            {
                this.DelayTicks = delayEntity.HandleQuery(new GameQuery_TicksCooldown()).Value;
            }
        }
    }
}
