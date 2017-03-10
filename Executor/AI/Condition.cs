﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Executor.AI
{
    [Serializable()]
    public abstract class Condition : SingleClause
    {
        public abstract bool IsMet(GameQuery_Command commandQuery);
    }
}
