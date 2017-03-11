using System;
using System.Collections.Immutable;

namespace Executor
{
    [Serializable()]
    public class Component_TracksTime : Component
    {
        public int LastActivationTick { get; private set; }
        private EntityAttributeType CooldownAttribute { get; }

        public Component_TracksTime(EntityAttributeType cooldownAttribute)
        {
            this.LastActivationTick = -9999;
            this.CooldownAttribute = cooldownAttribute;
        }

        public void Reset()
        {
            this.LastActivationTick = -9999;
        }

        protected override IImmutableSet<SubEntitiesSelector> _MatchingSelectors()
        {
            return ImmutableHashSet<SubEntitiesSelector>.Empty.Add(SubEntitiesSelector.TRACKS_TIME);
        }

        private void HandleEndTurn(GameEvent_EndTurn ev)
        {
            if (ev.CommandEntity == this.Parent)
            {
                this.LastActivationTick = ev.TurnTick;
            }
        }

        private void HandleDestroy(GameEvent_Destroy ev)
        {
            this.LastActivationTick = Int32.MaxValue;
        }

        protected override GameEvent _HandleEvent(GameEvent ev)
        {
            if (ev is GameEvent_EndTurn)
                this.HandleEndTurn((GameEvent_EndTurn)ev);
            else if (ev is GameEvent_Destroy)
                this.HandleDestroy((GameEvent_Destroy)ev);

            return ev;
        }

        private void HandleQueryTicksToLive(GameQuery_TicksToLive q)
        {
            // THIS IS A MESS! Hard to tell at what level this should be called at!
            var cooldown = this.Parent.TryGetAttribute(this.CooldownAttribute, this.Parent).Value;
            var ticksToLive = this.LastActivationTick + cooldown - q.CurrentTick;

            if (q.CurrentTick == 0 && this.LastActivationTick == -9999)
            {
                this.LastActivationTick -= ticksToLive;
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

        protected override GameQuery _HandleQuery(GameQuery q)
        {
            if (q is GameQuery_TicksToLive)
                this.HandleQueryTicksToLive((GameQuery_TicksToLive)q);
            if (q is GameQuery_TicksCooldown)
                this.HandleQueryTicksCooldown((GameQuery_TicksCooldown)q);

            return q;
        }
    }
}
