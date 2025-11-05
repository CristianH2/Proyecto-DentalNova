using DentalNova.Core.Repository.Validation;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DentalNova.Core.Dtos
{
    public class PerfilPacienteDtoIn
    {
        [Required]
        public bool ConAlergias { get; set; }
        [Required]
        public bool ConEnfermedadesCronicas { get; set; }
        [Required]
        public bool ConMedicamentosActuales { get; set; }
        [Required]
        public bool ConAntecedentesFamiliares { get; set; }

        [RequiredIf("ConAlergias", true, ErrorMessage = "Por favor, especifique las alergias.")]
        [StringLength(255)]
        public string? Alergias { get; set; }

        [RequiredIf("ConEnfermedadesCronicas", true, ErrorMessage = "Por favor, especifique las enfermedades crónicas.")]
        [StringLength(255)]
        public string? EnfermedadesCronicas { get; set; }

        [RequiredIf("ConMedicamentosActuales", true, ErrorMessage = "Por favor, especifique los medicamentos actuales.")]
        [StringLength(255)]
        public string? MedicamentosActuales { get; set; }

        [RequiredIf("ConAntecedentesFamiliares", true, ErrorMessage = "Por favor, especifique los antecedentes familiares.")]
        [StringLength(255)]
        public string? AntecedentesFamiliares { get; set; }

        [StringLength(500)]
        public string? Observaciones { get; set; }
    }
}
