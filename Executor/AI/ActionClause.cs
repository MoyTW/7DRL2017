using System;
using System.Collections.Generic;

namespace Executor.AI
{
    [Serializable()]
    class ActionClause
    {
        private IEnumerable<Condition> conditions;
        private AIAction action;

        public ActionClause(IEnumerable<Condition> conditions, AIAction action)
        {
            this.conditions = conditions;
            this.action = action;
        }

        public bool ShouldTakeAction(GameQuery_Command commandQuery)
        {
            if (!this.action.CanExecuteOn(commandQuery))
                return false;

            foreach (var c in this.conditions)
            {
                if (!c.IsMet(commandQuery))
                    return false;
            }

            return true;
        }

        public CommandStub CommandForQuery(GameQuery_Command commandQuery)
        {
            return this.action.GenerateCommand(commandQuery);
        }
    }
}

