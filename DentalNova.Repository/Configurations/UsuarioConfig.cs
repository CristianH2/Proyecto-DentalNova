using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using DentalNova.Core.Repository.Entities;

namespace DentalNova.Repository.Configurations
{
    public class UsuarioConfig : IEntityTypeConfiguration<Usuario>
    {
        public void Configure(EntityTypeBuilder<Usuario> builder)
        {
            builder.Property(prop => prop.Nombre).HasMaxLength(50).IsRequired();
            builder.Property(prop => prop.Apellidos).HasMaxLength(100).IsRequired();
            builder.Property(prop => prop.CorreoElectronico).HasMaxLength(100).IsRequired();
            builder.HasIndex(prop => prop.CorreoElectronico).IsUnique();
            builder.Property(prop => prop.Telefono).HasMaxLength(15);
            builder.Property(prop => prop.FechaNacimiento).HasColumnType("date");
            builder.Property(prop => prop.Genero).HasMaxLength(1);
            builder.Property(prop => prop.CURP).HasMaxLength(18).IsRequired();
            builder.HasIndex(prop => prop.CURP).IsUnique();
            builder.Property(prop => prop.Password).HasMaxLength(255).IsRequired();
            builder.Property(prop => prop.Activo).HasDefaultValue(true).IsRequired();
        }
    }

}