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
        public int MaxAP { get; private set; }
        public int CurrentAP { get; private set; }

        public Component_ActionExecutor(int maxAP)
        {
            this.MaxAP = maxAP;
            this.CurrentAP = maxAP;
        }

        protected override IImmutableSet<SubEntitiesSelector> _MatchingSelectors()
        {
            return ImmutableHashSet<SubEntitiesSelector>.Empty;
        }

        private void EndTurn(int spendTick)
        {
            this.Parent.HandleEvent(new GameEvent_EndTurn(spendTick, this.Parent));
            this.CurrentAP = this.MaxAP;
        }

        private void SpendAP(int spendTick, int cost)
        {
            this.CurrentAP -= cost;
            Log.DebugLine(this.Parent + " spent " + cost + " AP, AP Remaining: " + this.CurrentAP);
            if (this.CurrentAP < 0)
                throw new InvalidOperationException("Can't overspend AP!");
            else if (this.CurrentAP == 0)
                this.EndTurn(spendTick);
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

        private void HandleActivation(GameEvent_Activation ev)
        {
            this.SpendAP(ev.CommandTick, ev.APCost);
        }

        protected override GameEvent _HandleEvent(GameEvent ev)
        {
            if (ev is GameEvent_Command)
                this.HandleCommand((GameEvent_Command)ev);
            else if (ev is GameEvent_Activation)
                this.HandleActivation((GameEvent_Activation)ev);

            return ev;
        }

    }
}
