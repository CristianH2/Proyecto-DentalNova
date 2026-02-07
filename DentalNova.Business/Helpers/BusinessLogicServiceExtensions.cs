using DentalNova.Business.Rules;
using DentalNova.Core.Interfaces;
using DentalNova.Repository.Helpers;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using DentalNova.Security;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DentalNova.Business.Helpers
{
    public static class BusinessLogicServiceExtensions
    {
        public static IServiceCollection AddBusinessLogicServices(this IServiceCollection services, IConfiguration configuration)
        {
            // Llama al registrador de la capa de infraestructura
            services.AddInfrastructureServices(configuration);

            // Registra las reglas de negocio
            services.AddScoped<ITratamientoBL, TratamientoBL>();
            services.AddScoped<IUsuarioBL, UsuarioBL>();
            services.AddScoped<IArticuloBL, ArticuloBL>();
            services.AddScoped<IPacienteBL, PacienteBL>();
            services.AddScoped<ICitaBL, CitaBL>();
            services.AddScoped<IOdontologoBL, OdontologoBL>();
            services.AddScoped<IHorarioOdontologoBL, HorarioOdontologoBL>();
            services.AddScoped<IPagoBL, PagoBL>();
            services.AddScoped<IRecordatorioBL, RecordatorioBL>();
            services.AddScoped<IAuthBL, AuthBL>();
            services.AddScoped<IDashboardBL, DashboardBL>();

            // Registra la "Unidad de Trabajo" principal
            services.AddScoped<IUnitOfWork, UnitOfWork>();

            return services;
        }
    }
}
