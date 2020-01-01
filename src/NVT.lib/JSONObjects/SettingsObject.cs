using System.Text.Json.Serialization;

using NVT.lib.Common;

namespace NVT.lib.JSONObjects
{
    public class SettingsObject
    {
        public string[] EnabledConnectionTypes { get; set; }

        public bool EnableIPLookup { get; set; }

        public string IPLookupURL { get; set; }

        public string LogLevel { get; set; }

        public bool EnableMap { get; set; }

        [JsonIgnore] 
        public string[] LogLevels => Constants.LOG_LEVELS;
    }
}