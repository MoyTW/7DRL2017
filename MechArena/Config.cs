using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MechArena
{
    public class Config
    {
        public static int NumMaps()
        {
            return 5;
        }

        public static int NumThreads()
        {
            return Environment.ProcessorCount + 2;
        }
    }
}
