using System;
using System.Collections.Generic;
using System.Text;

namespace DentalNova.Movil.Services
{
    public static class AppConfig
    {
        // Pruebas 
        /*
        public static string BaseUrl = DeviceInfo.Platform == DevicePlatform.Android
            ? "https://10.0.2.2:7037/"  // Android Emulador /5260
            : "https://localhost:7037/"; // Windows
        */

        // Producción
        public static string BaseUrl = "https://api-dentalnova.azurewebsites.net/";
    }
}
