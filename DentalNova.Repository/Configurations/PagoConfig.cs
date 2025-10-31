using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using DentalNova.Core.Repository.Entities;

namespace DentalNova.Repository.Configurations
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