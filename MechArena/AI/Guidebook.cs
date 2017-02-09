using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MechArena.AI
{
    class ActionClause
    {
        private IEnumerable<Condition> conditions;
        private Action action;

        public ActionClause(IEnumerable<Condition> conditions, Action action)
        {
            this.conditions = conditions;
            this.action = action;
        }

        public bool ShouldTakeAction(GameQuery_Command commandQuery)
        {
            return !conditions.Any(c => !c.IsMet(commandQuery));
        }
    }

    class Guidebook
    {
        private List<SingleClause> rawRules;
        private List<ActionClause> builtRules;

        private ActionClause FirstActiveAction(GameQuery_Command commandQuery)
        {
            return this.builtRules.FirstOrDefault(a => a.ShouldTakeAction(commandQuery));
        }
    }
}
