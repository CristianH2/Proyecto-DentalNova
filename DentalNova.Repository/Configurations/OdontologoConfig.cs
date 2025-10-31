using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using DentalNova.Core.Repository.Entities;

namespace DentalNova.Repository.Configurations
{
    public class OdontologoConfig : IEntityTypeConfiguration<Odontologo>
    {
        public void Configure(EntityTypeBuilder<Odontologo> builder)
        {
            builder.Property(prop => prop.CedulaProfesional).HasMaxLength(50).IsRequired();
            //builder.Property(prop => prop.Especialidad).HasMaxLength(100).IsRequired();
            builder.Property(prop => prop.AnioGraduacion).HasColumnType("smallint").IsRequired(false);
            builder.Property(prop => prop.Institucion).HasMaxLength(150).IsRequired(false);
            builder.Property(prop => prop.FechaIngreso).HasColumnType("date").IsRequired();
        }
    }

}