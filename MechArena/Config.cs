using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MechArena
{
    public static class Config
    {
        public static int ArenaWidth { get { return 50; } }
        public static int ArenaHeight { get { return 50; } }

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
