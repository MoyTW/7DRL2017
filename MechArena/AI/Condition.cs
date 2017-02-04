using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MechArena.AI
{
    abstract class Condition : SingleClause
    {
        public abstract bool IsMet(Entity self, ArenaState state);
    }
}
