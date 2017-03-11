using System;
using System.Collections.Generic;

namespace Executor.AI
{
    [Serializable()]
    public class ActionClause_Flee : ActionClause
    {
        public override string Label
        {
            get
            {
                return "Terrified";
            }
        }
        public override string Description
        {
            get
            {
                return "Runs and hides";
            }
        }

        public ActionClause_Flee() : base(
            new List<Condition>() { },
            new Action_MoveAwayEnemy()) { }
    }
}

