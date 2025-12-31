using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DentalNova.Core.Repository.Interfaces
{
    public interface IRepository
    {
        ITratamientoRepository Tratamiento { get; }
        IUsuarioRepository Usuario { get; }
        IArticuloRepository Articulo { get; }
        IRolRepository Rol { get; }
        IPacienteRepository Paciente { get; }

        ICitaRepository Cita { get; }
        ICitaTratamientoRepository CitaTratamiento { get; }
        IHorarioOdontologoRepository HorarioOdontologo { get; }
        IOdontologoRepository Odontologo { get; }
        IEspecialidadRepository Especialidad { get; }
    }
}
