using Microsoft.EntityFrameworkCore;
using DentalNova.Repository.Configurations;
using DentalNova.Core.Repository.Entities;


namespace DentalNova.Repository.DataContext
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext() { }

        public ApplicationDbContext(DbContextOptions options) : base(options) { }

        protected override void ConfigureConventions(ModelConfigurationBuilder configurationBuilder) { }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
            modelBuilder.ApplyConfiguration(new UsuarioConfig());
            modelBuilder.ApplyConfiguration(new RolConfig());
            modelBuilder.ApplyConfiguration(new LogActividadConfig());
            modelBuilder.ApplyConfiguration(new PacienteConfig());
            modelBuilder.ApplyConfiguration(new OdontologoConfig());
            modelBuilder.ApplyConfiguration(new HorarioOdontologoConfig());
            modelBuilder.ApplyConfiguration(new CitaConfig());
            modelBuilder.ApplyConfiguration(new PagoConfig());
            modelBuilder.ApplyConfiguration(new TratamientoConfig());
            modelBuilder.ApplyConfiguration(new CitaTratamientoConfig());
            modelBuilder.ApplyConfiguration(new RecordatorioConfig());
            modelBuilder.ApplyConfiguration(new ArticulosConfig());
            modelBuilder.ApplyConfiguration(new CompraArticuloConfig());
            modelBuilder.ApplyConfiguration(new EspecialidadConfig());
        }

        public DbSet<Usuario> Usuarios { get; set; }
        public DbSet<Rol> Roles { get; set; }
        public DbSet<LogActividad> LogActividades { get; set; }
        public DbSet<Paciente> Pacientes { get; set; }
        public DbSet<Odontologo> Odontologos { get; set; }
        public DbSet<HorarioOdontologo> HorariosOdontologos { get; set; }
        public DbSet<Cita> Citas { get; set; }
        public DbSet<Pago> Pagos { get; set; }
        public DbSet<Tratamiento> Tratamientos { get; set; }
        public DbSet<CitaTratamiento> CitasTratamientos { get; set; }
        public DbSet<Recordatorio> Recordatorios { get; set; }
        public DbSet<Articulo> Articulos { get; set; }
        public DbSet<CompraArticulo> CompraArticulos { get; set; }
        public DbSet<Especialidad> Especialidades { get; set; }
    }
}


    
