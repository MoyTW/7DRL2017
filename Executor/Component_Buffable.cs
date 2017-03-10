using System;
using System.Collections.Generic;
using System.Collections.Immutable;

namespace Executor
{
    [Serializable()]
    public class Component_Buffable : Component
    {
        private List<StatusEffect> applyQueue = new List<StatusEffect>();
        private List<StatusEffect> activeEffects = new List<StatusEffect>();

        public Component_Buffable() { }

        protected override IImmutableSet<SubEntitiesSelector> _MatchingSelectors()
        {
            return ImmutableHashSet<SubEntitiesSelector>.Empty;
        }

        private void HandleReceiveStatusEffect(GameEvent_ReceiveStatusEffect ev)
        {
            if (this.Parent == ev.CommandEntity)
            {
                this.applyQueue.Add(ev.Effect);
                ev.Completed = true;
            }
        }

        private void HandleEndTurn(GameEvent_EndTurn ev)
        {
            for(int i = this.activeEffects.Count - 1; i >= 0; i--)
            {
                if (this.activeEffects[i].Expired)
                    this.activeEffects.RemoveAt(i);
            }
            foreach (var effect in this.applyQueue)
            {
                this.activeEffects.Add(effect);
            }
            this.applyQueue.Clear();
        }

        protected override GameEvent _HandleEvent(GameEvent ev)
        {
            if (ev is GameEvent_ReceiveStatusEffect)
                this.HandleReceiveStatusEffect((GameEvent_ReceiveStatusEffect)ev);

            foreach (var effect in this.activeEffects)
                effect.HandleEvent(ev);

            if (ev is GameEvent_EndTurn)
                this.HandleEndTurn((GameEvent_EndTurn)ev);

            return ev;
        }

        protected override GameQuery _HandleQuery(GameQuery q)
        {
            foreach (var effect in this.activeEffects)
                effect.HandleQuery(q);
            
            return q;
        }
    }
}

