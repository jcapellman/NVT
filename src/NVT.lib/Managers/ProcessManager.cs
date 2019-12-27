using System;
using System.Diagnostics;

using NLog;

namespace NVT.lib.Managers
{
    public class ProcessManager
    {
        private static readonly Logger Log = LogManager.GetCurrentClassLogger();

        public static string KillProcess(int processId)
        {
            try
            {
                var process = Process.GetProcessById(processId);

                process.Kill();

                return $"{Resources.AppResources.ProcessManager_KillProcess_Success} ({processId})";
            }
            catch (Exception ex)
            {
                Log.Error($"Exception when attempting to kill process: {ex}");

                return $"{Resources.AppResources.ProcessManager_KillProcess_Failure} ({processId}) - check logs for more information";
            }
        }
    }
}