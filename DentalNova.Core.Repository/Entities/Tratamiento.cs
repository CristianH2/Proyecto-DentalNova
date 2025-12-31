using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace DentalNova.Core.Repository.Entities
{
    public class Tratamiento
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "El nombre del tratamiento es obligatorio.")]
        [StringLength(100)]
        [DisplayName("Nombre del Tratamiento")]
        public string Nombre { get; set; }

        [StringLength(500)]
        [DataType(DataType.MultilineText)]
        [DisplayName("Descripción")]
        public string? Descripcion { get; set; }

        [Required(ErrorMessage = "El costo es obligatorio.")]
        [Column(TypeName = "decimal(10, 2)")]
        [Range(0, 9_999_999.99, ErrorMessage = "El costo debe ser un valor positivo.")]
        [DisplayName("Costo")]
        public decimal Costo { get; set; }

        [Required(ErrorMessage = "La duración es obligatoria.")]
        [Range(1, 1000, ErrorMessage = "La duración debe estar entre 1 y 1000 días.")]
        [DisplayName("Duración (días)")]
        public int DuracionDias { get; set; }

        [DisplayName("Activo")]
        public bool Activo { get; set; } = true;

        // Propiedad de navegación para las citas asociadas a este tratamiento
        public List<CitaTratamiento>? CitasTratamientos { get; set; }
    }
}
