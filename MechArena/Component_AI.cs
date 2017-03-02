using MechArena.AI;
using MechArena.AI.Combat;
using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;

namespace MechArena
{
    [Serializable()]
    class Component_AI : Component
    {
        private Guidebook book;

        public Component_AI()
        {
            List<SingleClause> rawRules = new List<SingleClause>();
            rawRules.Add(new Action_MoveTowardsEnemy());
            rawRules.Add(new Condition_CanSeeEnemy());
            rawRules.Add(new Condition_Distance(ComparisonOperator.LESS_THAN_EQUAL, DistanceOption.THIS_WEAPON_RANGE));
            rawRules.Add(new Action_AttackEnemy());
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
