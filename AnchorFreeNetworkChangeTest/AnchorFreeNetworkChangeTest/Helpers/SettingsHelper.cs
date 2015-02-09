using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AnchorFreeNetworkChangeTest.Helpers
{
    public static class SettingsHelper
    {
        private const string SSIDKey = "SSID";
       
        public static string SSID
        {
            get { return GetValue<string>(SSIDKey); }
            set { SetValue(SSIDKey, value); }
        }

        private static void SetValue<T>(string k, T v)
        {
            try
            {
                var settings = Windows.Storage.ApplicationData.Current.LocalSettings;
                settings.Values[k] = v;
            }
            catch (Exception ex)
            {
                Debug.WriteLine("Can't set settings value for '{0}': {1}", k, ex);
            }
        }

        private static T GetValue<T>(string k)
        {
            var settings = Windows.Storage.ApplicationData.Current.LocalSettings;
            var v = settings.Values[k];
            if (v == null)
                return default(T);
            try
            {
                return (T)v;
            }
            catch (InvalidCastException)
            {
                Debug.WriteLine("Can't get settings value for '{0}'", k);
                return default(T);
            }
        }

    }
}
