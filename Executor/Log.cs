using System;

namespace Executor
{
    public static class Log
    {
        private static bool printDebug = false;
        private static bool printInfo = true;
        private static bool printError = true;

        public static void ToggleDebugLog() { SetDebugLog(!printDebug); }

        public static void SetDebugLog(bool setting)
        {
            Log.printDebug = setting;
            if (setting)
                Log.Debug("Debug log enabled!");
            else
                Log.Debug("Debug log disabled!");
        }

        public static void ToggleInfoLog() { SetInfoLog(!printInfo); }

        public static void SetInfoLog(bool setting)
        {
            Log.printInfo = setting;
            if (setting)
                Log.Debug("Info log enabled!");
            else
                Log.Debug("Info log disabled!");
        }

        public static void ToggleErrorLog() { SetErrorLog(!printError); }

        public static void SetErrorLog(bool setting)
        {
            Log.printError = setting;
            if (setting)
                Log.Debug("Error log enabled!");
            else
                Log.Debug("Error log disabled!");
        }

        public static void Debug(object s) { if (Log.printDebug) Console.Write(s); }
        public static void DebugLine(object s) { if (Log.printDebug) Console.WriteLine(s); }

        public static void Info(object s) { if (Log.printInfo) Console.Write(s); }
        public static void InfoLine(object s) { if (Log.printInfo) Console.WriteLine(s); }

        public static void Error(object s) { if (Log.printError) Console.Write(s); }
        public static void ErrorLine(object s) { if (Log.printError) Console.WriteLine(s); }
    }
}
