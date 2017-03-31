using System.Configuration;

namespace UnturnedBot.Discord.Utils
{
    static class Settings
    {
        public static void AddOrUpdate(string key, string value)
        {
            var configFile = ConfigurationManager.OpenExeConfiguration(ConfigurationUserLevel.None);
            var settings = configFile.AppSettings.Settings;

            if (settings[key] == null)
            {
                settings.Add(key, value);
            }
            else
            {
                settings[key].Value = value;
            }

            configFile.Save(ConfigurationSaveMode.Modified);
            ConfigurationManager.RefreshSection(configFile.AppSettings.SectionInformation.Name);
        }
        public static string GetKeyOf(string value)
        {
            var configFile = ConfigurationManager.OpenExeConfiguration(ConfigurationUserLevel.None);

            ConfigurationManager.RefreshSection(configFile.AppSettings.SectionInformation.Name);
            var settings = configFile.AppSettings.Settings;

            foreach (var key in settings.AllKeys)
            {
                if (settings[key].Value == value)
                    return value;
            }
            return null;
        }
        public static string GetValue(string key)
        {
            var configFile = ConfigurationManager.OpenExeConfiguration(ConfigurationUserLevel.None);

            var keyValue = configFile.AppSettings.Settings[key];
            if (keyValue == null) return null;

            return keyValue.Value;
        }
    }
}
