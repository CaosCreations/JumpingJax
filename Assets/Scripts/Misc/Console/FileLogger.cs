using System;
using System.IO;
using UnityEngine;

public class FileLogger : MonoBehaviour
{
    private StreamWriter OutputStream;

    private void Awake()
    {

        if (ConsoleConstants.shouldLogToFile)
        {
            // Outputs to: C:\Users\<your-user>\AppData\LocalLow\DefaultCompany\UnityDebugConsole\log.txt
            string logFilePath = Path.Combine(Application.persistentDataPath, ConsoleConstants.fileLoggerFileName);
            OutputStream = new StreamWriter(logFilePath, true);

            // Delete large log files
            FileInfo fileInfo = new FileInfo(logFilePath);
            if(fileInfo.Length > ConsoleConstants.MaxLogSizeInBytes)
            {
                File.Delete(logFilePath);
            }
        }
    }

    private void OnDestroy()
    {
        if(OutputStream == null)
        {
            return;
        }

        OutputStream.Close();
        OutputStream = null;
    }
}