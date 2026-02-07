using System;
using System.Collections.Generic;
using System.Text;

namespace DentalNova.Movil.Models
{
    public class PerfilPacienteDto
    {
        public int Id { get; set; }
        public int Edad { get; set; }
        public bool ConAlergias { get; set; }
        public string Alergias { get; set; }
        public bool ConEnfermedadesCronicas { get; set; }
        public string EnfermedadesCronicas { get; set; }
        public bool ConMedicamentosActuales { get; set; }
        public string MedicamentosActuales { get; set; }
        public bool ConAntecedentesFamiliares { get; set; }
        public string AntecedentesFamiliares { get; set; }
        public string Observaciones { get; set; }
    }
}
