using System;
using System.IO;

namespace Senapp.Engine.Utilities
{
    public class Settings
    {
        public enum ConfigSettings
        {
            SKYBOX_FILE_PREFIX
        }

        private static string loadedSettingsConfig = string.Empty;
        private static bool hasLoadedSettingsFile = false;

        public static string GetSetting(ConfigSettings setting)
        {
            if (!hasLoadedSettingsFile)
            {
                loadedSettingsConfig = Resources.GetFile("settings");
                hasLoadedSettingsFile = true;
            }

            var settingName = setting.ToString();

            if (loadedSettingsConfig.Contains(settingName))
            {
                string value = string.Empty;
                using (var sr = new StringReader(loadedSettingsConfig))
                {
                    while(sr.Peek() != -1)
                    {
                        var line = sr.ReadLine();
                        if (line.StartsWith(settingName))
                        {
                            value = line.Substring(settingName.Length + 1);
                        }
                    }
                }
                return value;
            }
            else
            {
                Console.WriteLine($"Could not find setting {settingName} in settings.ini");
                return string.Empty;
            }
        }
    }
}
