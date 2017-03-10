using System;
using System.Collections.Generic;

namespace Executor.AI
{
    [Serializable()]
    public class ActionClause_Attack : ActionClause
    {
        public override string Label
        {
            get
            {
                return "Attacker";
            }
        }
        public override string Description
        {
            get
            {
                return "Attacks with primary weapon";
            }
        }

        public ActionClause_Attack() : base(
            new List<Condition>() {
                new Condition_CanSeeEnemy(),
                new Condition_Distance(ComparisonOperator.LESS_THAN_EQUAL, DistanceOption.MY_LONGEST_RANGE)
            },
            new Action_AttackEnemy()) { }
    }
}

