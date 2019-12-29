using System;
using System.IO;
using System.Linq;
using System.Text.Json;

using NVT.lib.JSONObjects;

using NLog;
using NVT.lib.Common;

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
                IPLookupURL = "http://localhost:5000/Lookup/",
                LogLevel = Constants.LOG_LEVELS.Last()
            };
        }

        private SettingsObject LoadFile(string fileName)
        {
            try
            {
                var json = File.ReadAllText(fileName);

                return JsonSerializer.Deserialize<SettingsObject>(json);
            } catch (Exception ex)
            {
                Log.Error($"Error loading settings from {fileName}, exception: {ex}");

                return null;
            }
        }
    }
}