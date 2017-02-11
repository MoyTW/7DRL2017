using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MechArena.AI
{
    [Serializable()]
    class Condition_MoveSpeed : Condition
    {
        public override bool IsMet(GameQuery_Command commandQuery)
        {
            throw new NotImplementedException();
        }

        public override IEnumerable<SingleClause> EnumerateClauses()
        {
            throw new NotImplementedException();
        }
    }
}
