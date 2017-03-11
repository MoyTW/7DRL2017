using System;
using System.Collections.Immutable;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Executor
{
    [Serializable()]
    class Component_ActionExecutor : Component
    {
        private int maxAP, currentAP;

        public Component_ActionExecutor(int maxAP)
        {
            this.maxAP = maxAP;
            this.currentAP = maxAP;
        }

        protected override IImmutableSet<SubEntitiesSelector> _MatchingSelectors()
        {
            return ImmutableHashSet<SubEntitiesSelector>.Empty;
        }

        private void HandleFocusEnd(GameEvent_FocusEnd ev)
        {
            if (this.Parent != ev.ExecutorEntity)
                throw new InvalidOperationException("Why is a focus end being broadcast to the wrong entity?");
            this.currentAP = 0;
        }

        private void HandleCommand(GameEvent_Command ev)
        {
            if (this.Parent == ev.ExecutorEntity)
                return;

            var executor = this.Parent.TryGetSubEntities(SubEntitiesSelector.ALL)
                .Where(e => e == ev.ExecutorEntity)
                .FirstOrDefault();

            if (executor != null || ev.CommandEntity == ev.ExecutorEntity)
            {
                ev.ExecutorEntity.HandleEvent(ev);
                if (!ev.Completed)
                    throw new InvalidOperationException("Executor " + ev.ExecutorEntity + " couldn't complete event!");
            }
            else
            {
                throw new InvalidOperationException("Executor " + ev.ExecutorEntity + " is not in Command Entity " +
                    ev.CommandEntity);
            }
        }

        private void EndTurn(int spendTick)
        {
            this.Parent.HandleEvent(new GameEvent_EndTurn(spendTick, this.Parent));
            this.currentAP = this.Parent.TryGetAttribute(EntityAttributeType.MAX_AP).Value;
        }

        private void SpendAP(int spendTick, int cost)
        {
            this.currentAP -= cost;
            Log.DebugLine(this.Parent + " spent " + cost + " AP, AP Remaining: " + this.currentAP);
            if (this.currentAP < 0)
                throw new InvalidOperationException("Can't overspend AP!");
            else if (this.currentAP == 0)
                this.EndTurn(spendTick);
        }

        private void HandleActivation(GameEvent_Activation ev)
        {
            this.SpendAP(ev.CommandTick, ev.APCost);
        }

        private void HandleDelay(GameEvent_Delay ev)
        {
            if (ev.CommandEntity == this.Parent)
            {
                ev.Completed = true;
            }
        }

        protected override GameEvent _HandleEvent(GameEvent ev)
        {
            if (ev is GameEvent_Delay)
                this.HandleDelay((GameEvent_Delay)ev);
            else if (ev is GameEvent_FocusEnd)
                this.HandleFocusEnd((GameEvent_FocusEnd)ev);
            else if (ev is GameEvent_Command)
                this.HandleCommand((GameEvent_Command)ev);
            else if (ev is GameEvent_Activation)
                this.HandleActivation((GameEvent_Activation)ev);

            return ev;
        }

        private void HandleQueryEntityAttribute(GameQuery_EntityAttribute q)
        {
            if (this.Parent == q.BaseEntity)
            {
                if (q.AttributeType == EntityAttributeType.CURRENT_AP)
                    q.RegisterBaseValue(this.currentAP);
                else if (q.AttributeType == EntityAttributeType.MAX_AP)
                    q.RegisterBaseValue(this.maxAP);
            }
        }

        protected override GameQuery _HandleQuery(GameQuery q)
        {
            if (q is GameQuery_EntityAttribute)
                this.HandleQueryEntityAttribute((GameQuery_EntityAttribute)q);

            return q;
        }
    }
}
