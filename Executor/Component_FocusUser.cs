using System;
using System.Collections.Immutable;

namespace Executor
{
    [Serializable()]
    public class Component_FocusUser : Component
    {
        private int focusAPBonus;

        public bool InFocus { get; private set; }
        public int MaxFreeMoves { get; }
        public int CurrentFreeMoves { get; private set; }

        public Component_FocusUser(int focusAPBonus=Config.DefaultFocusAPBonus,
            int focusFreeMoves=Config.DefaultFocusFreeMoves) 
        {
            this.focusAPBonus = focusAPBonus;
            this.MaxFreeMoves = focusFreeMoves;
            this.CurrentFreeMoves = focusFreeMoves;
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

        private void HandleEndTurn(GameEvent_EndTurn ev)
        {
            this.CurrentFreeMoves = this.MaxFreeMoves;
        }

        private void HandleMoveSingle(GameEvent_MoveSingle ev)
        {
            if (ev.CommandEntity != this.Parent)
                throw new InvalidOperationException("!?");
            if (this.InFocus && this.CurrentFreeMoves > 0)
            {
                ev.MakeFreeAction();
                this.CurrentFreeMoves--;
            }
        }

        protected override GameEvent _HandleEvent(GameEvent ev)
        {
            if (ev is GameEvent_FocusBegin)
                this.HandleFocusBegin((GameEvent_FocusBegin)ev);
            else if (ev is GameEvent_FocusEnd)
                this.HandleFocusEnd((GameEvent_FocusEnd)ev);
            else if (ev is GameEvent_EndTurn)
                this.HandleEndTurn((GameEvent_EndTurn)ev);
            else if (ev is GameEvent_MoveSingle)
                this.HandleMoveSingle((GameEvent_MoveSingle)ev);

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

