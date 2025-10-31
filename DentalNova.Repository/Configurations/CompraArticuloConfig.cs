using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using DentalNova.Core.Repository.Entities;

namespace DentalNova.Repository.Configurations
{
    public class CompraArticuloConfig : IEntityTypeConfiguration<CompraArticulo>
    {
        public void Configure(EntityTypeBuilder<CompraArticulo> builder)
        {
            builder.Property(prop => prop.Cantidad).IsRequired();
            builder.Property(prop => prop.PrecioUnitario).IsRequired().HasColumnType("decimal(10,2)");
            builder.Property(prop => prop.Subtotal).IsRequired().HasColumnType("decimal(10,2)");
            builder.Property(prop => prop.FechaCompra).IsRequired().HasColumnType("datetime");
            builder.Property(prop => prop.MetodoPago).IsRequired();
            builder.Property(prop => prop.Proveedor).IsRequired().HasMaxLength(50);
        }
    }
}