using System;
using System.IO;
using System.Linq;
using System.Text.Json;

using NVT.lib.Common;
using NVT.lib.JSONObjects;

using NLog;

namespace NVT.lib.Managers
{
    public class SettingsManager
    {
        private static readonly Logger Log = LogManager.GetCurrentClassLogger();

        private const string SETTINGS_FILENAME = "settings.json";

        public SettingsObject SettingsObject;

        public SettingsManager()
        {
            if (File.Exists(SETTINGS_FILENAME))
            {
                SettingsObject = LoadFile(SETTINGS_FILENAME);
            }

            if (SettingsObject != null)
            {
                return;
            }

            SettingsObject = InitializeDefaultSettings();

            WriteFile();
        }

        public bool WriteFile()
        {
            try
            {
                File.WriteAllText(SETTINGS_FILENAME, JsonSerializer.Serialize(SettingsObject));

                return true;
            }
            catch (Exception ex)
            {
                Log.Error($"An error occurred when saving {SETTINGS_FILENAME}: {ex}");

                return false;
            }
        }

        private SettingsObject InitializeDefaultSettings()
        {
            return new SettingsObject
            {
                EnabledConnectionTypes = DIContainer.GetDIService<ConnectionManager>().SupportedConnectionTypes,
                EnableIPLookup = true,
                IPLookupURL = Constants.FALLBACK_LOOKUPURL,
                EnableMap = true,
                LogLevel = Constants.LOG_LEVELS.Last()
            };
        }

        private static SettingsObject SanityCheck(SettingsObject settings)
        {
            if (settings.EnableIPLookup && string.IsNullOrEmpty(settings.IPLookupURL))
            {
                settings.IPLookupURL = Common.Constants.FALLBACK_LOOKUPURL;
            }

            return settings;
        }

        private SettingsObject LoadFile(string fileName)
        {
            try
            {
                var json = File.ReadAllText(fileName);

                var result = JsonSerializer.Deserialize<SettingsObject>(json);

                return SanityCheck(result);
            } catch (Exception ex)
            {
                Log.Error($"Error loading settings from {fileName}, exception: {ex}");

                return null;
            }
        }
    }
}