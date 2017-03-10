using System;
using System.Collections.Generic;

namespace Executor.AI
{
    [Serializable()]
    public class ActionClause_SeekMaxWeaponRange : ActionClause
    {
        public override string Label
        {
            get
            {
                return "Cowardly";
            }
        }
        public override string Description
        {
            get
            {
                return "Keeps a lot of distance";
            }
        }

        public ActionClause_SeekMaxWeaponRange() : base(
            new List<Condition>() {
            new Condition_Distance(ComparisonOperator.LESS_THAN, DistanceOption.MY_LONGEST_RANGE)
            },
            new Action_MoveAwayEnemy()) { }
    }
}

