using UnityEngine;

namespace Oneleif.debugconsole
{
    public class ConsoleConstants
    {
        public static KeyCode toggleKey = KeyCode.BackQuote;

        public static bool shouldLogToFile = true;
        public static bool shouldOutputDebugLogs = true;

        public static string commandPrefix = "> ";

        public static string fileLoggerFileName = "log.txt";
        public static bool fileLoggerAddTimestamp = true;
    }
}
