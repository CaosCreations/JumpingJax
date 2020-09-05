using System;
using System.IO;
using UnityEngine;

namespace Oneleif.debugconsole
{
    public class FileLogger : MonoBehaviour
    {
        private StreamWriter OutputStream;

        private void Start()
        {
            if (ConsoleConstants.shouldLogToFile)
            {
                // Outputs to: C:\Users\<your-user>\AppData\LocalLow\DefaultCompany\UnityDebugConsole\log.txt
                string logFilePath = Path.Combine(Application.persistentDataPath, ConsoleConstants.fileLoggerFileName);
                OutputStream = new StreamWriter(logFilePath, false);
                // TODO: clear old log files if they're too big
            }
        }

        private void OnDestroy()
        {
            OutputStream.Close();
            OutputStream = null;
        }

        public void LogToFile(string message)
        {
            if (!ConsoleConstants.shouldLogToFile)
            {
                return;
            }

            if (ConsoleConstants.fileLoggerAddTimestamp)
            {
                DateTime now = DateTime.Now;
                message = string.Format("[{0:H:mm:ss}] {1}", now, message);
            }

            if (OutputStream != null)
            {
                OutputStream.WriteLine(message);
                OutputStream.Flush();
            }
        }
    }
}