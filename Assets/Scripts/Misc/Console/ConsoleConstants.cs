using UnityEngine;

public class ConsoleConstants
{
    public static KeyCode toggleKey = KeyCode.BackQuote;
    public static int MaxConsoleLines = 100;

    public static bool shouldLogToFile = true;
    public static bool shouldOutputDebugLogs = true;

    public static string commandPrefix = "> ";

    public static string fileLoggerFileName = "log.txt";
    public static bool fileLoggerAddTimestamp = true;
}
