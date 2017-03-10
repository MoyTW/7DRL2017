using System;
using System.Collections.Generic;

namespace Executor.AI
{
    [Serializable()]
    public class ActionClause_AvoidMelee : ActionClause
    {
        public override string Label
        {
            get
            {
                return "Wary";
            }
        }
        public override string Description
        {
            get
            {
                return "Avoids close combat";
            }
        }

        public ActionClause_AvoidMelee() : base(
            new List<Condition>() {
                new Condition_Distance(ComparisonOperator.LESS_THAN_EQUAL, DistanceOption.MELEE_RANGE)
            },
            new Action_MoveAwayEnemy()) { }
    }
}

