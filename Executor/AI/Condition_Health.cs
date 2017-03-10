using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Executor.AI
{
    [Serializable()]
    class Condition_Health : Condition
    {
        public override bool IsMet(GameQuery_Command commandQuery)
        {
            throw new NotImplementedException();
        }
    }
}
