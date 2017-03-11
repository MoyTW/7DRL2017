using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Executor
{
    public static class Config
    {
        public const int ZERO = 0;
        public const int ONE = 1;

        public const int DefaultEntityAP = 1;
        public const int DefaultFocusAPBonus = 3;
        public const int DefaultFocusFreeMoves = 2;

        public static int ArenaWidth { get { return 70; } }
        public static int ArenaHeight { get { return 70; } }

        public static int TargetingWindowX { get { return 50; } }
        public static int TargetingWindowY { get { return 23; } }

        public static int MinPatrolDistance { get { return 20; } }

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
