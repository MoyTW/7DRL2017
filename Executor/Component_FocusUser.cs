using System;
using System.Collections.Immutable;

namespace Executor
{
    [Serializable()]
    public class Component_FocusUser : Component
    {
        private int focusAPBonus;

        public bool InFocus { get; private set; }

        public Component_FocusUser(int focusAPBonus=6) 
        {
            this.focusAPBonus = focusAPBonus;
            this.InFocus = false;
        }

        protected override IImmutableSet<SubEntitiesSelector> _MatchingSelectors()
        {
            return ImmutableHashSet<SubEntitiesSelector>.Empty;
        }

        private void HandleFocusBegin(GameEvent_FocusBegin ev)
        {
            this.InFocus = true;
            ev.Completed = true;
        }

        private void HandleFocusEnd(GameEvent_FocusEnd ev)
        {
            this.InFocus = false;
            ev.Completed = true;
        }

        protected override GameEvent _HandleEvent(GameEvent ev)
        {
            if (ev is GameEvent_FocusBegin)
                this.HandleFocusBegin((GameEvent_FocusBegin)ev);
            else if (ev is GameEvent_FocusEnd)
                this.HandleFocusEnd((GameEvent_FocusEnd)ev);

            return ev;
        }

        private void HandleQueryEntityAttribute(GameQuery_EntityAttribute q)
        {
            if (this.InFocus && this.Parent == q.BaseEntity && q.AttributeType == EntityAttributeType.MAX_AP)
                q.AddFlatModifier(this.focusAPBonus, this.Parent);
        }

        protected override GameQuery _HandleQuery(GameQuery q)
        {
            if (q is GameQuery_EntityAttribute)
                this.HandleQueryEntityAttribute((GameQuery_EntityAttribute)q);

            return q;
        }
    }
}

