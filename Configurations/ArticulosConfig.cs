using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Proyecto_DentalNova.Models;

namespace Proyecto_DentalNova.Configurations
{
    public class ArticulosConfig : IEntityTypeConfiguration<Articulo>
    {
        public void Configure(EntityTypeBuilder<Articulo> builder)
        {
            builder.Property(prop => prop.Categoria).HasColumnType("smallint").IsRequired();
            builder.Property(prop => prop.Nombre).HasMaxLength(100).IsRequired();
            builder.Property(prop => prop.Descripcion).HasMaxLength(200).IsRequired(false);
            builder.Property(prop => prop.Codigo).HasMaxLength(50).IsRequired();
            builder.HasIndex(prop => prop.Codigo).IsUnique();
            builder.Property(prop => prop.Reutilizable).IsRequired();
            builder.Property(prop => prop.Stock).IsRequired();
            builder.Property(prop => prop.Activo).HasDefaultValue(true).IsRequired();
        }
    }
}
