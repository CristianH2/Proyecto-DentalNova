using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DentalNova.Core.Dtos
{
    public class PacienteFilterDto : PaginacionDto // Hereda page/pageSize
    {
        public int? Id { get; set; }

        // Filtros de Usuario
        public string? NombreLike { get; set; }
        public string? ApellidosLike { get; set; }
        public string? CorreoLike { get; set; }
        public string? TelefonoLike { get; set; }

        // Filtros de Paciente
        public int? EdadMin { get; set; }
        public int? EdadMax { get; set; }
        public DateTime? FechaDesde { get; set; }
        public DateTime? FechaHasta { get; set; }

        // Booleanos (Historial)
        public bool ConAlergias { get; set; }
        public bool ConEnfermedadesCronicas { get; set; }
        public bool ConMedicamentosActuales { get; set; }
        public bool ConAntecedentesFamiliares { get; set; }
    }
}
