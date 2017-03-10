using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Executor.AI
{
    [Serializable()]
    public abstract class AIAction : SingleClause
    {
        public abstract Boolean CanExecuteOn(GameQuery_Command commandQuery);
        public abstract CommandStub GenerateCommand(GameQuery_Command commandQuery);
    }
}
