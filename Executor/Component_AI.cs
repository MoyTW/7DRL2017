using Executor.AI;
using Executor.AI.Combat;
using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;

namespace Executor
{
    [Serializable()]
    class Component_AI : Component
    {
        private Guidebook book;

        public Component_AI()
        {
            List<SingleClause> rawRules = new List<SingleClause>();

            // Kite away from melee
            rawRules.Add(new Condition_Distance(ComparisonOperator.LESS_THAN_EQUAL, DistanceOption.MELEE_RANGE));
            rawRules.Add(new Action_MoveAwayEnemy());
            // Shoot
            rawRules.Add(new Condition_CanSeeEnemy());
            rawRules.Add(new Condition_Distance(ComparisonOperator.LESS_THAN_EQUAL, DistanceOption.MY_LONGEST_RANGE));
            rawRules.Add(new Action_AttackEnemy());
            // Move Towards
            rawRules.Add(new Action_MoveTowardsEnemy());
            // Catchall
            rawRules.Add(new Action_Delay());

            this.book = new Guidebook(rawRules);
        }

        public Component_AI(Guidebook book)
        {
            this.book = book;
        }

        protected override IImmutableSet<SubEntitiesSelector> _MatchingSelectors()
        {
            return ImmutableHashSet<SubEntitiesSelector>.Empty;
        }

        private void HandleQueryCommand(GameQuery_Command q)
        {
            this.book.TryRegisterCommand(q);
        }

        protected override GameQuery _HandleQuery(GameQuery q)
        {
            if (q is GameQuery_Command)
                this.HandleQueryCommand((GameQuery_Command)q);

            return q;

        }
    }
}
