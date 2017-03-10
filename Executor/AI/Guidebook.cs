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
        private List<ActionClause> clauses;
        public IList<ActionClause> ActionClauses { get { return this.clauses.AsReadOnly(); } }

        public Guidebook(List<ActionClause> clauses)
        {
            this.clauses = clauses;
        }

        public void TryRegisterCommand(GameQuery_Command commandRequest)
        {
            CommandStub command = this.clauses.Where(r => r.ShouldTakeAction(commandRequest))
                .Select(r => r.CommandForQuery(commandRequest))
                .FirstOrDefault();
            if (command != null)
                commandRequest.RegisterCommand(command);
        }
    }
}
