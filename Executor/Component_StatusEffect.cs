using System;

namespace Executor
{
    abstract public class Component_StatusEffect : Component
    {
        public int DurationMax { get; }
        public int DurationRemaining { get; private set; }
        abstract public string EffectLabel { get; }

        public Component_StatusEffect(int durationMax)
        {
            this.DurationMax = durationMax;
            this.DurationRemaining = durationMax;
        }

        abstract protected void _HandleEndTurn(GameEvent_EndTurn ev);

        private void HandleEndTurn(GameEvent_EndTurn ev)
        {
            this._HandleEndTurn(ev);

            this.DurationRemaining--;
            if (this.DurationRemaining == 0)
                this.Parent.RemoveComponent(this);
        }

        protected override GameEvent _HandleEvent(GameEvent ev)
        {
            if (ev is GameEvent_EndTurn)
                this.HandleEndTurn((GameEvent_EndTurn)ev);

            return ev;
        }
    }
}

