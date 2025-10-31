using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using DentalNova.Core.Repository.Entities;

namespace DentalNova.Repository.Configurations
{
    public class HorarioOdontologoConfig : IEntityTypeConfiguration<HorarioOdontologo>
    {
        public void Configure(EntityTypeBuilder<HorarioOdontologo> builder)
        {
            builder.Property(prop => prop.DiaSemana).IsRequired();
            builder.Property(prop => prop.HoraInicio).HasColumnType("time").IsRequired();
            builder.Property(prop => prop.HoraFin).HasColumnType("time").IsRequired();
            builder.Property(prop => prop.Activo).HasDefaultValue(true).IsRequired();
            builder.Property(prop => prop.Consultorio).HasMaxLength(100).IsRequired();
        }
    }
}
