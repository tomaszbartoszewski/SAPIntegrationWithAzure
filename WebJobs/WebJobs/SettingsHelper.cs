using System.Configuration;
using Microsoft.Azure;

namespace OutboundStudent
{
    public static class SettingsHelper
    {
        public static string GetSetting(string key)
        {
#if DEBUG
            var value = ConfigurationManager.AppSettings.Get(key);
#else
            var value = CloudConfigurationManager.GetSetting(key);
#endif
            if (value == null) throw new ConfigurationErrorsException($"No value configured for key: '{key}'");
            return value;
        }
    }
}
