using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using DentalNova.Core.Repository.Validation;

namespace DentalNova.Core.Repository.Entities
{
    public class Paciente
    {
        [DisplayName("ID")]
        public int Id { get; set; }

        [Required(ErrorMessage = "La edad es obligatoria.")]
        [Range(0, 120, ErrorMessage = "La edad debe ser un valor válido.")]
        [DisplayName("Edad")]
        public int Edad { get; set; }

        // * Checkboxes * 
        [DisplayName("¿Tiene alergias?")]
        public bool ConAlergias { get; set; }

        [DisplayName("¿Tiene enfermedades crónicas?")]
        public bool ConEnfermedadesCronicas { get; set; }

        [DisplayName("¿Toma medicamentos actuales?")]
        public bool ConMedicamentosActuales { get; set; }

        [DisplayName("¿Tiene antecedentes familiares?")]
        public bool ConAntecedentesFamiliares { get; set; }

        // * TextArea *
        [RequiredIf("ConAlergias", true, ErrorMessage = "Por favor, especifique las alergias.")]
        [StringLength(255)]
        [DisplayName("Alergias")]
        public string? Alergias { get; set; }

        [RequiredIf("ConEnfermedadesCronicas", true, ErrorMessage = "Por favor, especifique las enfermedades crónicas.")]
        [StringLength(255)]
        [DisplayName("Enfermedades Crónicas")]
        public string? EnfermedadesCronicas { get; set; }

        [RequiredIf("ConMedicamentosActuales", true, ErrorMessage = "Por favor, especifique los medicamentos actuales.")]
        [StringLength(255)]
        [DisplayName("Medicamentos Actuales")]
        public string? MedicamentosActuales { get; set; }

        [RequiredIf("ConAntecedentesFamiliares", true, ErrorMessage = "Por favor, especifique los antecedentes familiares.")]
        [StringLength(255)]
        [DisplayName("Antecedentes Familiares")]
        public string? AntecedentesFamiliares { get; set; }

        // * TextArea Observaciones *
        [StringLength(500)]
        [DisplayName("Observaciones")]
        public string? Observaciones { get; set; }

        // * Fechas de Creación y Actualización *
        [DataType(DataType.DateTime)]
        public DateTime FechaCreacion { get; set; }

        [DataType(DataType.DateTime)]
        public DateTime? FechaActualizacion { get; set; }

        // --- Llaves Foráneas (Foreign Keys) ---
        [Required(ErrorMessage = "Debe seleccionar un usuario para asociarlo como paciente.")]
        [DisplayName("Usuario")]
        public int UsuarioId { get; set; }

        // --- Propiedades de Navegación ---
        public virtual Usuario? Usuario { get; set; }
        public virtual List<Pago>? Pagos { get; set; }
        public virtual List<Cita>? Citas { get; set; }
    }
}
