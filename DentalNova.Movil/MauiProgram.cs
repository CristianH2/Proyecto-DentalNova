using DentalNova.Movil.Pages;
using DentalNova.Movil.Services;
using DentalNova.Movil.ViewModels;
using Microsoft.Extensions.Logging;

namespace DentalNova.Movil
{
    public static class MauiProgram
    {
        public static MauiApp CreateMauiApp()
        {
            var builder = MauiApp.CreateBuilder();
            builder
                .UseMauiApp<App>()
                .ConfigureFonts(fonts =>
                {
                    fonts.AddFont("OpenSans-Regular.ttf", "OpenSansRegular");
                    fonts.AddFont("OpenSans-Semibold.ttf", "OpenSansSemibold");
                });

            // HttpClient (Global)
            builder.Services.AddSingleton(sp =>
            {
                var handler = new HttpClientHandler();
                handler.ServerCertificateCustomValidationCallback = (sender, cert, chain, sslPolicyErrors) => true;

                var client = new HttpClient(handler)
                {
                    BaseAddress = new Uri(AppConfig.BaseUrl),
                    Timeout = TimeSpan.FromSeconds(30)
                };
                return client;
            });

            // Servicios
            builder.Services.AddSingleton<ConfiguracionService>();
            builder.Services.AddSingleton<InicioDeSesionService>();
            builder.Services.AddSingleton<CitaService>();
            builder.Services.AddSingleton<OdontologoService>();
            builder.Services.AddSingleton<NotificacionService>();
            builder.Services.AddSingleton<PerfilService>();

            // UnitOfWork
            builder.Services.AddSingleton<UnitOfWork>();

            // ViewModels
            builder.Services.AddTransient<InicioDeSesionPage>();
            builder.Services.AddTransient<InicioDeSesionViewModel>();
            builder.Services.AddTransient<CitasViewModel>();
            builder.Services.AddTransient<AgendarViewModel>();
            builder.Services.AddTransient<NotificacionesViewModel>();
            builder.Services.AddTransient<PerfilViewModel>();
            builder.Services.AddTransient<RegistroViewModel>();

            // Páginas
            builder.Services.AddTransient<CitasPage>();
            builder.Services.AddTransient<AgendarPage>();
            builder.Services.AddTransient<NotificacionesPage>();
            builder.Services.AddTransient<PerfilPage>();
            builder.Services.AddTransient<RegistroPage>();


#if DEBUG
            builder.Logging.AddDebug();
#endif

            return builder.Build();
        }
    }
}
