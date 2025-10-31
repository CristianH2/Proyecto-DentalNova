using System.ComponentModel.DataAnnotations;

namespace Proyecto_DentalNova.Models.PacienteViewModel
{
    public class PacienteFilterViewModel
    {
        // --- Filtros de Usuario ---
        [Display(Name = "ID del Paciente")]
        public int? Id { get; set; }

        [Display(Name = "Nombre")]
        public string? NombreLike { get; set; }

        [Display(Name = "Apellidos")]
        public string? ApellidosLike { get; set; }

        [Display(Name = "Correo Electrónico")]
        public string? CorreoLike { get; set; }

        [Display(Name = "Teléfono")]
        public string? TelefonoLike { get; set; }

        // --- Filtros de Paciente ---
        [Display(Name = "Edad Mínima")]
        public int? EdadMin { get; set; }

        [Display(Name = "Edad Máxima")]
        public int? EdadMax { get; set; }

        [Display(Name = "Registrado Desde")]
        public DateTime? FechaDesde { get; set; }

        [Display(Name = "Registrado Hasta")]
        public DateTime? FechaHasta { get; set; }

        // --- Filtros de Historial Clínico ---
        [Display(Name = "Con Alergias")]
        public bool ConAlergias { get; set; }

        [Display(Name = "Con Enf. Crónicas")]
        public bool ConEnfermedadesCronicas { get; set; }

        [Display(Name = "Con Medicamentos")]
        public bool ConMedicamentosActuales { get; set; }

        [Display(Name = "Con Antecedentes Fam.")]
        public bool ConAntecedentesFamiliares { get; set; }

        // --- Paginación ---
        public int Page { get; set; } = 1;
        public int PageSize { get; set; } = 10;
    }
}
