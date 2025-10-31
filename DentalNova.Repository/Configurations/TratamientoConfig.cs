using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using DentalNova.Core.Repository.Entities;

namespace DentalNova.Repository.Configurations
{
    public class TratamientoConfig : IEntityTypeConfiguration<Tratamiento>
    {
        public void Configure(EntityTypeBuilder<Tratamiento> builder)
        {
            builder.Property(prop => prop.Nombre).HasMaxLength(100).IsRequired();
            builder.Property(prop => prop.Descripcion).HasMaxLength(500).IsRequired(false);
            builder.Property(prop => prop.Costo).HasColumnType("decimal(10,2)").IsRequired();
            builder.Property(prop => prop.DuracionDias).IsRequired();
            builder.Property(prop => prop.Activo).HasDefaultValue(true).IsRequired();
        }
    }
}
