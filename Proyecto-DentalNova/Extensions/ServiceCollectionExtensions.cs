using DentalNova.Core.Interfaces;
using Proyecto_DentalNova.Handlers;
using Proyecto_DentalNova.Services;

namespace Proyecto_DentalNova.Extensions
{
    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection AddDentalNovaApiServices(this IServiceCollection services, IConfiguration configuration)
        {
            // Obtener la URL
            //var baseUrl = configuration["Api:BaseUrl"] ?? "http://localhost:5260/";
            var baseUrl = configuration["Api:BaseUrl"] ?? "https://api-dentalnova.azurewebsites.net/";

            // Registrar el handler de autenticación
            services.AddTransient<AuthHeaderHandler>();

            // Configuración común para todos los clientes HTTP
            Action<IServiceProvider, HttpClient> configureClient = (sp, http) =>
            {
                http.BaseAddress = new Uri(baseUrl);
                http.DefaultRequestHeaders.Add("Accept", "application/json");
            };

            // Registrar los servicios HTTP con el handler de autenticación
            services.AddHttpClient<IUsuarioService, UsuarioServiceApi>(configureClient)
                .AddHttpMessageHandler<AuthHeaderHandler>();
            services.AddHttpClient<IPacienteService, PacienteServiceApi>(configureClient)
                .AddHttpMessageHandler<AuthHeaderHandler>();
            services.AddHttpClient<IOdontologoService, OdontologoServiceApi>(configureClient)
                .AddHttpMessageHandler<AuthHeaderHandler>();
            services.AddHttpClient<ITratamientoService, TratamientoServiceApi>(configureClient)
                .AddHttpMessageHandler<AuthHeaderHandler>();
            services.AddHttpClient<IHorarioOdontologoService, HorarioOdontologoServiceApi>(configureClient)
                .AddHttpMessageHandler<AuthHeaderHandler>();
            services.AddHttpClient<ICitaService, CitaServiceApi>(configureClient)
                .AddHttpMessageHandler<AuthHeaderHandler>();
            services.AddHttpClient<IArticuloService, ArticuloServiceApi>(configureClient)
                .AddHttpMessageHandler<AuthHeaderHandler>();
            services.AddHttpClient<IPagoService, PagoServiceApi>(configureClient)
                .AddHttpMessageHandler<AuthHeaderHandler>();
            services.AddHttpClient<IRecordatorioService, RecordatorioServiceApi>(configureClient)
                .AddHttpMessageHandler<AuthHeaderHandler>();
            services.AddHttpClient<IDashboardService, DashboardServiceApi>(configureClient)
                .AddHttpMessageHandler<AuthHeaderHandler>();

            services.AddHttpClient<IAuthService, AuthServiceApi>(configureClient);

            return services;
        }
    }
}
