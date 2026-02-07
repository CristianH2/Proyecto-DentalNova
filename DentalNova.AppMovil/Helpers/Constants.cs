using System;
using System.Collections.Generic;
using System.Text;

namespace DentalNova.AppMovil.Helpers
{
    public static class Constants
    {
        public const string Port = "7037";
        public const string Scheme = "https";

        public static string BaseUrl
        {
            get
            {
                // Android Emulador usa 10.0.2.2 para ver al host
                if (DeviceInfo.Platform == DevicePlatform.Android)
                    return $"{Scheme}://10.0.2.2:{Port}/api";

                // Windows y iOS Simulators usan localhost
                return $"{Scheme}://localhost:{Port}/api";
            }
        }

        // Claves para SecureStorage
        public const string AuthTokenKey = "AuthToken";
        public const string UserIdKey = "UserId";
        public const string UserNameKey = "UserName";
    }
}
