using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;

using NLog;

using NVT.lib.Objects;

namespace NVT.lib.Managers
{
    public class ProcessManager
    {
        private static readonly Logger Log = LogManager.GetCurrentClassLogger();

        public static Dictionary<int, ProcessItem> GetCurrentProcesses()
        {
            var processes = Process.GetProcesses();

            return processes.ToDictionary(a => a.Id,b => new ProcessItem(b));
        }

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