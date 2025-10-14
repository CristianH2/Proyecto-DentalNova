using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Proyecto_DentalNova.Models;

namespace Proyecto_DentalNova.Configurations
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
