using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DentalNova.Core.Dtos
{
    public class RegistroCompletoDto
    {
        public UsuarioDtoIn Usuario { get; set; }
        public PerfilPacienteDtoIn Paciente { get; set; }
    }
}
