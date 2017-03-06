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

        private void SpendAP(int cost)
        {
            this.CurrentAP -= cost;
            Log.DebugLine(this.Parent + " spent " + cost + " AP, AP Remaining: " + this.CurrentAP);
            if (this.CurrentAP < 0)
                throw new InvalidOperationException("Can't overspend AP!");
            else if (this.CurrentAP == 0)
                Console.WriteLine("Here the turn of " + this.Parent + " should end!");
        }

        private void HandleCommand(GameEvent_Command ev)
        {
            // I don't like this! It can't ensure that ev was completed, because to re-dispatch to the parent would
            // trap it in an infinite loop, and to put it at the bottom would mean if the event *is* completed it never
            // reaches this code. Eugh!
            if (this.Parent == ev.ExecutorEntity)
            {
                this.SpendAP(ev.APCost);
                return;
            }

            var executor = this.Parent.TryGetSubEntities(SubEntitiesSelector.ALL)
                .Where(e => e == ev.ExecutorEntity)
                .FirstOrDefault();

            if (executor != null || ev.CommandEntity == ev.ExecutorEntity)
            {
                ev.ExecutorEntity.HandleEvent(ev);
                if (!ev.Completed)
                    throw new InvalidOperationException("Executor " + ev.ExecutorEntity + " couldn't complete event!");
                this.SpendAP(ev.APCost);
            }
            else
            {
                throw new InvalidOperationException("Executor " + ev.ExecutorEntity + " is not in Command Entity " +
                    ev.CommandEntity);
            }
        }

        protected override GameEvent _HandleEvent(GameEvent ev)
        {
            if (ev is GameEvent_Command)
                this.HandleCommand((GameEvent_Command)ev);

            return ev;
        }

    }
}
