using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using DentalNova.Core.Repository.Entities;

namespace DentalNova.Repository.Configurations
{
    public class CitaTratamientoConfig : IEntityTypeConfiguration<CitaTratamiento>
    {
        public void Configure(EntityTypeBuilder<CitaTratamiento> builder)
        {
            builder.Property(prop => prop.Observaciones).HasMaxLength(200).IsRequired(false);
            builder.Property(prop => prop.CostoFinal).HasPrecision(10, 2).IsRequired();
            builder.Property(prop => prop.EstatusTratamiento).IsRequired();
        }
    }
}