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
        private Entity self;

        ActionClause(Entity self, IEnumerable<Condition> conditions, Action action)
        {
            this.conditions = conditions;
            this.action = action;
            this.self = self;
        }

        public bool ShouldTakeAction(ArenaState state)
        {
            return !conditions.Any(c => !c.IsMet(self, state));
        }
    }

    class Guidebook
    {
        private ArenaState state;
        private List<SingleClause> rawRules;
        private List<ActionClause> builtRules;

        private ActionClause FirstActiveAction()
        {
            return this.builtRules.FirstOrDefault(a => a.ShouldTakeAction(state));
        }
    }
}
