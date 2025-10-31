using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using DentalNova.Core.Repository.Entities;

namespace DentalNova.Repository.Configurations
{
    public class LogActividadConfig : IEntityTypeConfiguration<LogActividad>
    {
        public void Configure(EntityTypeBuilder<LogActividad> builder)
        {
            builder.Property(prop => prop.FechaHora).HasDefaultValueSql("GETDATE()");
            builder.Property(prop => prop.AccionRealizada).HasMaxLength(50).IsRequired();
            builder.Property(prop => prop.Detalles).HasMaxLength(150).IsRequired(false);

        }
    }
}
