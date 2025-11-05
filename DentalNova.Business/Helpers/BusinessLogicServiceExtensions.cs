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

            // Llama al registrador de la capa de seguridad
            services.AddSecurityServices(configuration);

            // Registra las reglas de negocio
            services.AddScoped<ITratamientoBL, TratamientoBL>();
            services.AddScoped<IUsuarioBL, UsuarioBL>();
            services.AddScoped<IArticuloBL, ArticuloBL>();
            services.AddScoped<IPacienteBL, PacienteBL>();

            // Registra la "Unidad de Trabajo" principal
            services.AddScoped<IUnitOfWork, UnitOfWork>();

            return services;
        }
    }
}
