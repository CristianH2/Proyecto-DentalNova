using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using static DentalNova.Core.Repository.Entities.Enumerables;

namespace DentalNova.Core.Repository.Entities
{
    public class CitaTratamiento
    {
        [DisplayName("ID")]
        public int Id { get; set; }

        [StringLength(200)]
        [DataType(DataType.MultilineText)]
        [DisplayName("Observaciones")]
        public string? Observaciones { get; set; }

        [Required(ErrorMessage = "El costo final es obligatorio.")]
        [Column(TypeName = "decimal(10, 2)")]
        [Range(0, 9_999_999.99, ErrorMessage = "El costo debe ser un valor positivo.")]
        [DisplayName("Costo Final")]
        public decimal CostoFinal { get; set; }

        [Required(ErrorMessage = "Debe seleccionar un estatus para el tratamiento.")]
        [DisplayName("Estatus del Tratamiento")]
        public EstatusTratamiento EstatusTratamiento { get; set; }

        // --- Llaves Foráneas (Foreign Keys) ---
        [Required]
        public int CitaId { get; set; }

        [Required(ErrorMessage = "Debe seleccionar un tratamiento.")]
        [DisplayName("Tratamiento")]
        public int TratamientoId { get; set; }

        // --- Propiedades de Navegación ---
        public virtual Cita? Cita { get; set; }
        public virtual Tratamiento? Tratamiento { get; set; }
    }
}
