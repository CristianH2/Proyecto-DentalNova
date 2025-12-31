using DentalNova.Core.Repository.Interfaces;
using DentalNova.Repository.Daos;
using DentalNova.Repository.DataContext;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DentalNova.Repository.Helpers
{
    public static class InfrastructureServiceExtensions
    {
        public static IServiceCollection AddInfrastructureServices(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddDbContext<ApplicationDbContext>(options =>
                options.UseSqlServer(configuration.GetConnectionString("DefaultConnection")));

            //Registro de los repositorios
            services.AddScoped<ITratamientoRepository, TratamientoRepository>();
            services.AddScoped<IUsuarioRepository, UsuarioRepository>();
            services.AddScoped<IArticuloRepository, ArticuloRepository>();
            services.AddScoped<IRolRepository, RolRepository>();
            services.AddScoped<IPacienteRepository, PacienteRepository>();

            services.AddScoped<ICitaRepository, CitaRepository>();
            services.AddScoped<ICitaTratamientoRepository, CitaTratamientoRepository>();
            services.AddScoped<IHorarioOdontologoRepository, HorarioOdontologoRepository>();
            services.AddScoped<IOdontologoRepository, OdontologoRepository>();
            services.AddScoped<IEspecialidadRepository, EspecialidadRepository>();

            services.AddScoped<IRolRepository, RolRepository>();

            // "Unidad de Trabajo" del repositorio
            services.AddScoped<IRepository, Repositorio>();

            return services;
        }
    }
}
