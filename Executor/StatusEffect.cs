using System;

namespace Executor
{
    [Serializable()]
    // TODO: Should have a IHandlesEvents or something
    abstract public class StatusEffect : Component
    {
        public int Duration { get; private set; }
        public bool Expired { get { return this.Duration <= 0; } }

        abstract public string EffectLabel { get; }

        public StatusEffect(int duration)
        {
            this.Duration = duration;
        }

        abstract protected void _HandleEndTurn(GameEvent_EndTurn ev);

        private void HandleEndTurn(GameEvent_EndTurn ev)
        {
            this._HandleEndTurn(ev);

            this.Duration--;
        }

        protected override GameEvent _HandleEvent(GameEvent ev)
        {
            if (ev is GameEvent_EndTurn)
                this.HandleEndTurn((GameEvent_EndTurn)ev);

            return ev;
        }
    }
}

