using System;
using System.Collections.Generic;
using System.Text;

namespace DentalNova.Movil.Models
{
    public class RegistroCompletoDto
    {
        public UsuarioRegistroDto Usuario { get; set; }
        public PacienteRegistroDto Paciente { get; set; }
    }

    // 2. Datos de Usuario/Cuenta
    public class UsuarioRegistroDto
    {
        public string Nombre { get; set; }
        public string Apellidos { get; set; }
        public string CorreoElectronico { get; set; }
        public string Curp { get; set; }
        public string Password { get; set; }
        public string Telefono { get; set; }
        public DateTime FechaNacimiento { get; set; }
        public string Genero { get; set; }
    }

    // 3. Datos Médicos
    public class PacienteRegistroDto
    {
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
