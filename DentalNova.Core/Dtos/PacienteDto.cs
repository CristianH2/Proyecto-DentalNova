using DentalNova.Core.Repository.Validation;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DentalNova.Core.Dtos
{
    public class PacienteDto
    {
        public int Id { get; set; }
        public int Edad { get; set; } // Edad calculada
        public bool ConAlergias { get; set; }
        public string? Alergias { get; set; }
        public bool ConEnfermedadesCronicas { get; set; }
        public string? EnfermedadesCronicas { get; set; }
        public bool ConMedicamentosActuales { get; set; }
        public string? MedicamentosActuales { get; set; }
        public bool ConAntecedentesFamiliares { get; set; }
        public string? AntecedentesFamiliares { get; set; }
        public string? Observaciones { get; set; }
        public DateTime FechaCreacion { get; set; }
        public DateTime? FechaActualizacion { get; set; }
    }

    public class PacienteAdminDtoIn
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "Debe seleccionar un usuario para asociarlo como paciente.")]
        [DisplayName("Usuario")]
        public int UsuarioId { get; set; }

        [Required(ErrorMessage = "La edad es obligatoria.")]
        [Range(0, 120, ErrorMessage = "La edad debe ser un valor válido.")]
        [DisplayName("Edad")]
        public int Edad { get; set; }

        [DisplayName("¿Tiene alergias?")]
        public bool ConAlergias { get; set; }

        [RequiredIf("ConAlergias", true, ErrorMessage = "Por favor, especifique las alergias.")]
        [StringLength(255)]
        [DisplayName("Alergias")]
        public string? Alergias { get; set; }

        [DisplayName("¿Tiene enfermedades crónicas?")]
        public bool ConEnfermedadesCronicas { get; set; }

        [RequiredIf("ConEnfermedadesCronicas", true, ErrorMessage = "Por favor, especifique las enfermedades crónicas.")]
        [StringLength(255)]
        [DisplayName("Enfermedades Crónicas")]
        public string? EnfermedadesCronicas { get; set; }

        [DisplayName("¿Toma medicamentos actuales?")]
        public bool ConMedicamentosActuales { get; set; }

        [RequiredIf("ConMedicamentosActuales", true, ErrorMessage = "Por favor, especifique los medicamentos actuales.")]
        [StringLength(255)]
        [DisplayName("Medicamentos Actuales")]
        public string? MedicamentosActuales { get; set; }

        [DisplayName("¿Tiene antecedentes familiares?")]
        public bool ConAntecedentesFamiliares { get; set; }

        [RequiredIf("ConAntecedentesFamiliares", true, ErrorMessage = "Por favor, especifique los antecedentes familiares.")]
        [StringLength(255)]
        [DisplayName("Antecedentes Familiares")]
        public string? AntecedentesFamiliares { get; set; }

        [StringLength(500)]
        [DisplayName("Observaciones")]
        public string? Observaciones { get; set; }
    }

    public class PacienteAdminDto
    {
        public int Id { get; set; }
        public int UsuarioId { get; set; }

        // Datos del Usuario (para mostrar en la tabla)
        public string Nombre { get; set; }
        public string Apellidos { get; set; }
        public string NombreCompleto => $"{Nombre} {Apellidos}";
        public string CorreoElectronico { get; set; }
        public string? Telefono { get; set; }

        // Datos del Paciente
        public int Edad { get; set; }
        public DateTime FechaCreacion { get; set; }
        public bool ConAlergias { get; set; }
        public bool ConEnfermedadesCronicas { get; set; }
        public bool ConMedicamentosActuales { get; set; }
        public bool ConAntecedentesFamiliares { get; set; }
        public string? Alergias { get; set; }
        public string? EnfermedadesCronicas { get; set; }
        public string? MedicamentosActuales { get; set; }
        public string? AntecedentesFamiliares { get; set; }
        public string? Observaciones { get; set; }
    }
}
