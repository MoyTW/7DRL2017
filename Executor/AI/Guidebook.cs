using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Executor.AI
{
    [Serializable()]
    public class Guidebook
    {
        private List<ActionClause> builtRules;
        public IEnumerable<ActionClause> ActionClauses { get { return this.builtRules; } }

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
