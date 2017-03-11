using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Executor
{
    [Serializable()]
    public class GameQuery
    {
        // Initializes as False, should probably be explicit to reduce confusion.
        public bool Completed { get; set; }
    }
}
