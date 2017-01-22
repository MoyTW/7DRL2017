using System;
using System.Collections.Generic;

namespace MechArena
{
    [Serializable()]
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

        protected override ISet<SubEntitiesSelector> _MatchingSelectors()
        {
            return new HashSet<SubEntitiesSelector>() { SubEntitiesSelector.ACTIVE_TRACKS_TIME };
        }

        private void HandleDelay(GameEvent_Delay ev)
        {
            if (ev.ExecutorEntity == this.Parent)
            {
                this.lastActivationTick += ev.DelayTicks;
                ev.Completed = true;
            }
        }

        protected override GameEvent _HandleEvent(GameEvent ev)
        {
            if (ev is GameEvent_Delay)
                this.HandleDelay((GameEvent_Delay)ev);

            return ev;
        }

        private void HandleQueryTicksToLive(GameQuery_TicksToLive q)
        {
            // THIS IS A MESS! Hard to tell at what level this should be called at!
            var cooldown = this.Parent.TryGetAttribute(this.CooldownAttribute, this.Parent).Value;
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
            // THIS IS A MESS! Hard to tell at what level this should be called at!
            q.RegisterTicksCooldown(this.Parent.TryGetAttribute(this.CooldownAttribute, this.Parent).Value);
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
