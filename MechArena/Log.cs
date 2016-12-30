using System;

namespace MechArena
{
    public static class Log
    {
        private static bool printDebug = false;

        public static void EnableDebugLog()
        {
            Log.printDebug = true;
            Log.Debug("Debug log enabled!");
        }

        public static void DebugLine(object s)
        {
            if (Log.printDebug)
            {
                Console.WriteLine(s);
            }
        }

        public static void Debug(object s)
        {
            if(Log.printDebug)
            {
                Console.Write(s);
            }
        }
    }
}
