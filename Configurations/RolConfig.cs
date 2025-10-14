using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Proyecto_DentalNova.Models;

namespace Proyecto_DentalNova.Configurations
{
    public class RolConfig : IEntityTypeConfiguration<Rol>
    {
        public void Configure(EntityTypeBuilder<Rol> builder)
        {
            builder.Property(prop => prop.Nombre).HasMaxLength(50).IsRequired();
            builder.Property(prop => prop.Descripcion).HasMaxLength(250);

        }
    }
}
