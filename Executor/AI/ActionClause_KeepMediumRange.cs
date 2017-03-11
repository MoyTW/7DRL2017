using System;
using System.Collections.Generic;

namespace Executor.AI
{
    [Serializable()]
    public class ActionClause_KeepMediumRange : ActionClause
    {
        public override string Label
        {
            get
            {
                return "Cautious";
            }
        }
        public override string Description
        {
            get
            {
                return "Keeps foes at medium range";
            }
        }

        public ActionClause_KeepMediumRange() : base(
            new List<Condition>() {
                new Condition_Distance(ComparisonOperator.LESS_THAN_EQUAL, DistanceOption.MY_MEDIUM_RANGE)
            },
            new Action_MoveAwayEnemy()) { }
    }
}

