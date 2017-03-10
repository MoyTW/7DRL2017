using System;
using System.Collections.Generic;

namespace Executor.AI
{
    [Serializable()]
    public class ActionClause_Approach : ActionClause
    {
        public override string Label
        {
            get
            {
                return "Combatative";
            }
        }
        public override string Description
        {
            get
            {
                return "Engages you in combat";
            }
        }

        public ActionClause_Approach(): base(
            new List<Condition>() { },
            new Action_MoveTowardsEnemy()) { }
    }
}

