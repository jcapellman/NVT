using System.IO;

using NLog;

namespace NVT.lib.Managers
{
    public class LogConfigurationManager
    {
        public static string AdjustLogLevel(string logLevel)
        {
            if (!File.Exists(Common.Constants.LOG_CONFIG))
            {
                return Resources.AppResources.LogConfigurationManager_FileNotFound;
            }

            NLog.LogManager.Configuration.RemoveRuleByName("*");

            NLog.LogManager.Configuration.AddRule(LogLevel.Debug, LogLevel.Error, "logFile", "*");

            NLog.LogManager.ReconfigExistingLoggers();

            var lines = File.ReadAllLines(Common.Constants.LOG_CONFIG);

            for (var x = 0; x < lines.Length; x++)
            {
                if (!lines[x].Trim().StartsWith("<logger"))
                {
                    continue;
                }

                lines[x] = $"<logger name=\"*\" minlevel=\"{logLevel}\" writeTo=\"logfile\" />";

                break;
            }

            File.WriteAllLines(Common.Constants.LOG_CONFIG, lines);

            return string.Empty;
        }
    }
}