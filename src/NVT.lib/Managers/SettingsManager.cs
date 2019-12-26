using System;
using System.IO;
using System.Text.Json;

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

            if (SettingsObject == null)
            {
                SettingsObject = InitializeDefaultSettings();

                WriteFile();
            }
        }

        public bool WriteFile()
        {
            try
            {
                File.WriteAllText(SETTINGS_FILENAME, JsonSerializer.Serialize(SettingsObject));

                return true;
            } catch (Exception ex)
            {
                Log.Error($"An error occurred when saving {SETTINGS_FILENAME}: {ex}");

                return false;
            }
        }

        private SettingsObject InitializeDefaultSettings()
        {
            return new SettingsObject
            {
                EnabledConnectionTypes = new ConnectionManager().SupportedConnectionTypes,
                EnableIPLookup = true,
                IPLookupURL = string.Empty
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