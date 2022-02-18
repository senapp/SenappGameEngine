using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace Senapp.Engine.Utilities
{
    public enum ConfigSettings
    {
        SKYBOX_FILE_PREFIX,
        ANTI_ALIASING,
        FXAA_SAMPLES,
        MSAA_SAMPLES,
        COLOUR_BITS,
        DEPTH_BITS,
        STENCIL_BITS,
        ACCUM_BITS,
    }

    public class Settings
    {
        public static string SettingsFilePath => $"{SenappGameEngineFolder}/Settings.ini";
        public static string SenappGameEngineFolder => $"{MyGamesFolder}/SenappGameEngine";
        public static string MyGamesFolder => $"{Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments)}/My Games/";

        public static bool SetSetting<T>(ConfigSettings setting, T data)
        {
            SettingsSetup();
            var settingName = setting.ToString();

            if (loadedSettingsConfig.Contains(settingName))
            {
                string value = string.Empty;
                using (var sr = new StringReader(loadedSettingsConfig))
                {
                    while (sr.Peek() != -1)
                    {
                        var line = sr.ReadLine();
                        if (line.StartsWith(settingName))
                        {
                            value = line[(settingName.Length + 1)..];
                        }
                    }
                }

                var index = loadedSettingsConfig.IndexOf(settingName);
                loadedSettingsConfig = loadedSettingsConfig.Remove(index, settingName.Length + 1 + value.Length);
                loadedSettingsConfig = loadedSettingsConfig.Insert(index, $"{settingName}={data}");
                File.WriteAllText(SettingsFilePath, loadedSettingsConfig);
                return true;
            }
            else
            {
                StringBuilder sb = new();
                sb.AppendLine(loadedSettingsConfig);
                sb.AppendLine($"{settingName}={data}");
                loadedSettingsConfig = sb.ToString();
                File.WriteAllText(SettingsFilePath, loadedSettingsConfig);
                return true;
            }
        }
        public static T GetSetting<T>(ConfigSettings setting, T createSettingWithData = default)
        {
            SettingsSetup();

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
                            value = line[(settingName.Length + 1)..];
                        }
                    }
                }

                if (!string.IsNullOrEmpty(value))
                {
                    if (typeof(T).IsEnum)
                    {
                        return (T)Enum.Parse(typeof(T), value);
                    }

                    return (T)Convert.ChangeType(value, typeof(T));
                }
                else
                {
                    return default;
                }
            }
            else
            {
                if (EqualityComparer<T>.Default.Equals(createSettingWithData, default))
                {
                    Console.WriteLine($"[ENGINE] Could not find setting {settingName} in settings.ini");
                    return default;
                }
                else
                {
                    StringBuilder sb = new();
                    sb.AppendLine(loadedSettingsConfig);
                    sb.AppendLine($"{settingName}={createSettingWithData}");
                    loadedSettingsConfig = sb.ToString();
                    File.WriteAllText(SettingsFilePath, loadedSettingsConfig);
                    return createSettingWithData;
                }
            }
        }

        private static void SettingsSetup()
        {
            if (!Directory.Exists(MyGamesFolder))
            {
                Directory.CreateDirectory(MyGamesFolder);
            }

            if (!Directory.Exists(SenappGameEngineFolder))
            {
                Directory.CreateDirectory(SenappGameEngineFolder);
            }

            if (!File.Exists(SettingsFilePath))
            {
                loadedSettingsConfig = Resources.GetFile("settings");
                File.WriteAllText(SettingsFilePath, loadedSettingsConfig);
            }
            else
            {
                var res = File.ReadAllLines(SettingsFilePath);
                StringBuilder sb = new();
                foreach (var line in res)
                {
                    sb.AppendLine(line);
                }
                loadedSettingsConfig = sb.ToString();
            }

            if (!hasLoadedSettingsFile)
            {
                hasLoadedSettingsFile = true;
            }
        }

        private static string loadedSettingsConfig = string.Empty;
        private static bool hasLoadedSettingsFile = false;
    }
}
