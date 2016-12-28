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
        NEXT_ACTION,
        FULL_INTERVAL
    }
    class GameEvent_Delay : GameEvent_Command
    {
        public int DelayTicks { get; }
        public DelayDuration Duration { get; }

        public GameEvent_Delay(int currentTick, Entity commandEntity, Entity delayEntity, DelayDuration duration)
            : base(commandEntity, delayEntity)
        {
            this.Duration = duration;
            if (this.Duration == DelayDuration.SINGLE_TICK)
            {
                this.DelayTicks = 1;
            }
            else if (this.Duration == DelayDuration.NEXT_ACTION)
            {
                var timeTrackers = new List<Entity>(commandEntity.TryGetSubEntities(SubEntitiesSelector.TRACKS_TIME));
                if (commandEntity.HasComponentOfType<Component_TracksTime>())
                    timeTrackers.Add(commandEntity);
                timeTrackers.Remove(delayEntity);

                var nextEntity = timeTrackers.Where(e => !e.TryGetDestroyed())
                    .OrderBy(e => e.TryGetTicksToLive(currentTick))
                    .FirstOrDefault();

                if (nextEntity != null)
                    this.DelayTicks = nextEntity.TryGetTicksToLive(currentTick);
                else
                    this.DelayTicks = delayEntity.HandleQuery(new GameQuery_TicksCooldown()).Value;
            }
            else
            {
                this.DelayTicks = delayEntity.HandleQuery(new GameQuery_TicksCooldown()).Value;
            }
        }
    }
}
