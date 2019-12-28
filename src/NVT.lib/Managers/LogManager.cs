using System.IO;

namespace NVT.lib.Managers
{
    public class LogManager
    {
        public static string AdjustLogLevel(string logLevel)
        {
            if (!File.Exists(Common.Constants.LOG_CONFIG))
            {
                return "Could not locate config file - reinstall";
            }

            return string.Empty;
        }
    }
}