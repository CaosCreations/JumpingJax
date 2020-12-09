using UnityEngine;

public class ConsoleConstants
{
    public static KeyCode toggleKey = KeyCode.BackQuote;
    public static int MaxConsoleLines = 100;

    public static bool shouldLogToFile = true;
    public static bool shouldOutputDebugLogs = true;

    public static string commandPrefix = "> ";

    public static string fileLoggerFileName = "jjConsoleLog.txt";
    public static bool fileLoggerAddTimestamp = true;
    public static long MaxLogSizeInBytes = 10000000; // 10 MB


    public static Color highlightColor = new Color(.5f, .6f, 1f);
    public static Color autocompleteColor = new Color(.9f, .9f, .9f);
    
}
