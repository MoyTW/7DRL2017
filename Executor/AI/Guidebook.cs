using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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

    [Serializable()]
    public class Guidebook
    {
        private List<ActionClause> builtRules;

        public Guidebook(IEnumerable<SingleClause> rawRules)
        {
            this.builtRules = Guidebook.BuildRules(rawRules);
        }

        private static List<ActionClause> BuildRules(IEnumerable<SingleClause> rawRules)
        {
            var builtRules = new List<ActionClause>();
            var acc = new List<Condition>();
            foreach (var clause in rawRules)
            {
                if (clause is AIAction)
                {
                    builtRules.Add(new ActionClause(acc, (AIAction)clause));
                    acc = new List<Condition>();
                }
                else if (clause is Condition)
                    acc.Add((Condition)clause);
                else
                    Log.ErrorLine("Can't process clause " + clause);
            }
            return builtRules;
        }

        public void TryRegisterCommand(GameQuery_Command commandRequest)
        {
            CommandStub command = this.builtRules.Where(r => r.ShouldTakeAction(commandRequest))
                .Select(r => r.CommandForQuery(commandRequest))
                .FirstOrDefault();
            if (command != null)
                commandRequest.RegisterCommand(command);
        }
    }
}
