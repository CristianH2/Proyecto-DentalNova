using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Proyecto_DentalNova.Models;

namespace Proyecto_DentalNova.Configurations
{
    public class RecordatorioConfig : IEntityTypeConfiguration<Recordatorio>
    {
        public void Configure(EntityTypeBuilder<Recordatorio> builder)
        {
            builder.Property(prop => prop.FechaEnvio).IsRequired(false);
            builder.Property(prop => prop.Mensaje).HasMaxLength(500).IsRequired(false);
            builder.Property(prop => prop.Enviado).HasDefaultValue(false);
        }
    }
}