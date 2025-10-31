using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using DentalNova.Core.Repository.Entities;

namespace DentalNova.Repository.Configurations
{
    public class PacienteConfig : IEntityTypeConfiguration<Paciente>
    {
        public void Configure(EntityTypeBuilder<Paciente> builder)
        {
            builder.Property(prop => prop.Edad).HasColumnType("tinyint").IsRequired();
            builder.Property(prop => prop.ConAlergias).HasDefaultValue(false).IsRequired();
            builder.Property(prop => prop.ConEnfermedadesCronicas).HasDefaultValue(false).IsRequired();
            builder.Property(prop => prop.ConMedicamentosActuales).HasDefaultValue(false).IsRequired();
            builder.Property(prop => prop.ConAntecedentesFamiliares).HasDefaultValue(false).IsRequired();
            builder.Property(prop => prop.Alergias).HasMaxLength(255).IsRequired(false);
            builder.Property(prop => prop.EnfermedadesCronicas).HasMaxLength(255).IsRequired(false);
            builder.Property(prop => prop.MedicamentosActuales).HasMaxLength(255).IsRequired(false);
            builder.Property(prop => prop.AntecedentesFamiliares).HasMaxLength(255).IsRequired(false);
            builder.Property(prop => prop.Observaciones).HasMaxLength(255).IsRequired(false);
            builder.Property(prop => prop.FechaCreacion).HasDefaultValueSql("GETDATE()").IsRequired();
            builder.Property(prop => prop.FechaActualizacion).IsRequired(false);
        }
    }
}