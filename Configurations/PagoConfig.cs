using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Proyecto_DentalNova.Models;

namespace Proyecto_DentalNova.Configurations
{
    public class PagoConfig : IEntityTypeConfiguration<Pago>
    {
        public void Configure(EntityTypeBuilder<Pago> builder)
        {
            builder.Property(prop => prop.Monto).HasPrecision(10, 2).IsRequired();
            builder.Property(prop => prop.FechaPago).HasDefaultValueSql("GETDATE()").IsRequired();
            builder.Property(prop => prop.MetodoPago).IsRequired();
        }
    }
}