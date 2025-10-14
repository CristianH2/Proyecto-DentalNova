using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Proyecto_DentalNova.Models;

namespace Proyecto_DentalNova.Configurations
{
    public class CitaConfig : IEntityTypeConfiguration<Cita>
    {
        public void Configure(EntityTypeBuilder<Cita> builder)
        {
            builder.Property(prop => prop.FechaHora).IsRequired();
            builder.Property(prop => prop.DuracionMinutos).IsRequired();
            builder.Property(prop => prop.EstatusCita).IsRequired();
            builder.Property(prop => prop.MotivoConsulta).HasMaxLength(255).IsRequired(false);
            builder.Property(prop => prop.FechaCreacion).HasDefaultValueSql("GETDATE()").IsRequired();
            builder.Property(prop => prop.FechaActualizacion).IsRequired(false);

            // Relación con Paciente
            builder.HasOne(cita => cita.Paciente)
                   .WithMany(paciente => paciente.Citas)
                   .HasForeignKey(cita => cita.PacienteId)
                   .OnDelete(DeleteBehavior.Restrict);

            // Relación con Odontologo
            builder.HasOne(cita => cita.Odontologo)
                   .WithMany(odontologo => odontologo.Citas)
                   .HasForeignKey(cita => cita.OdontologoId)
                   .OnDelete(DeleteBehavior.Restrict);
        }
    }
}