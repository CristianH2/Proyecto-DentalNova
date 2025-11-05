using System;
using System.Collections.Generic;
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
}
