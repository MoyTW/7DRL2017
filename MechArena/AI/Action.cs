using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MechArena.AI
{
    abstract class Action : SingleClause
    {
        public abstract Boolean CanExecuteOn(GameQuery_Command commandQuery);
        public abstract GameEvent_Command GenerateCommand(GameQuery_Command commandQuery);
    }
}
