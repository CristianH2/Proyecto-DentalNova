using DentalNova.Core.Repository.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DentalNova.Repository.Daos
{
    public class Repositorio : IRepository
    {
        public ITratamientoRepository Tratamiento { get; }
        public IUsuarioRepository Usuario { get; }
        public IArticuloRepository Articulo { get; }
        public IRolRepository Rol { get; }
        public IPacienteRepository Paciente { get; }

        public ICitaRepository Cita { get; }
        public ICitaTratamientoRepository CitaTratamiento { get; }
        public IHorarioOdontologoRepository HorarioOdontologo { get; }
        public IOdontologoRepository Odontologo { get; }
        public IEspecialidadRepository Especialidad { get; }

        public Repositorio( 
            ITratamientoRepository tratamientoRepository, 
            IUsuarioRepository usuarioRepository,
            IArticuloRepository articuloRepository,
            IRolRepository rolRepository,
            IPacienteRepository pacienteRepository,

            ICitaRepository citaRepository,
            ICitaTratamientoRepository citaTratamientoRepository,
            IHorarioOdontologoRepository horarioOdontologoRepository,
            IOdontologoRepository odontologoRepository,

            IEspecialidadRepository especialidadRepository
            )
        {
            Tratamiento = tratamientoRepository;
            Usuario = usuarioRepository;
            Articulo = articuloRepository;
            Rol = rolRepository;
            Paciente = pacienteRepository;

            Cita = citaRepository;
            CitaTratamiento = citaTratamientoRepository;
            HorarioOdontologo = horarioOdontologoRepository;
            Odontologo = odontologoRepository;
            Especialidad = especialidadRepository;
        }
    }
}
