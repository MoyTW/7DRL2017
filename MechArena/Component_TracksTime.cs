using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MechArena
{
    public class Component_TracksTime : Component
    {
        private int lastActivationTick;
        private EntityAttributeType CooldownAttribute { get; }

        public Component_TracksTime(EntityAttributeType cooldownAttribute)
        {
            this.lastActivationTick = -9999;
            this.CooldownAttribute = cooldownAttribute;
        }

        public void RegisterActivated(int activationTick)
        {
            this.lastActivationTick = activationTick;
        }

        private void HandleDelay(GameEvent_Delay ev)
        {
            this.lastActivationTick += ev.DelayTicks;
            ev.Completed = true;
        }

        protected override GameEvent _HandleEvent(GameEvent ev)
        {
            if (ev is GameEvent_Delay)
                this.HandleDelay((GameEvent_Delay)ev);

            return ev;
        }

        private void HandleQueryTicksToLive(GameQuery_TicksToLive q)
        {
            var cooldown = this.Parent.TryGetAttribute(this.CooldownAttribute).Value;
            var ticksToLive = this.lastActivationTick + cooldown - q.CurrentTick;

            if (q.CurrentTick == 0 && this.lastActivationTick == -9999)
            {
                this.lastActivationTick -= ticksToLive;
                q.RegisterTicksToLive(0);
            }
            else
            {
                q.RegisterTicksToLive(ticksToLive);
            }
        }

        private void HandleQueryTicksCooldown(GameQuery_TicksCooldown q)
        {
            q.RegisterTicksCooldown(this.Parent.TryGetAttribute(this.CooldownAttribute).Value);
        }

        private void HandleQueryNextTimeTracker(GameQuery_NextTimeTracker q)
        {
            if (!this.Parent.TryGetDestroyed())
            {
                q.RegisterEntity(this.Parent);
            }
        }

        protected override GameQuery _HandleQuery(GameQuery q)
        {
            if (q is GameQuery_TicksToLive)
                this.HandleQueryTicksToLive((GameQuery_TicksToLive)q);
            if (q is GameQuery_TicksCooldown)
                this.HandleQueryTicksCooldown((GameQuery_TicksCooldown)q);
            if (q is GameQuery_NextTimeTracker)
                this.HandleQueryNextTimeTracker((GameQuery_NextTimeTracker)q);

            return q;
        }
    }
}
