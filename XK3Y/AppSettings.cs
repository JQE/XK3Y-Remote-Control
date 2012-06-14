using System.IO.IsolatedStorage;
using System.Net;

namespace XK3Y
{
    public class AppSettings
    {
        private static readonly IsolatedStorageSettings settings = IsolatedStorageSettings.ApplicationSettings;

        private const string IP = "ipAddress";
        private const string refreshRate = "refreshRate";

        public static T GetValueOrDefault<T>(string key, T defaultValue = default(T))
        {
            return (settings.Contains(key)) ? (T) settings[key] : defaultValue;
        }

        public static void SetValue<T>(string key, T value)
        {
            settings[key] = value;
        }

        public static void Save()
        {
            settings.Save();
        }

        public static IPAddress IPAddress
        {
            get
            {
                string s = GetValueOrDefault(IP, string.Empty);
                IPAddress ip;
                return (!string.IsNullOrEmpty(s) && IPAddress.TryParse(s, out ip)) ? ip : null;
            }
            set { SetValue(IP, value.ToString()); }
        }

        public static int RefreshRate
        {
            get { return GetValueOrDefault(refreshRate, 5); }
            set { SetValue(refreshRate, value); }
        }
    }
}
