using DentalNova.Business.Helpers;
using DentalNova.Core.Dtos;
using DentalNova.Core.Interfaces;
using DentalNova.Core.Repository.Entities;
using DentalNova.Core.Repository.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static DentalNova.Core.Repository.Entities.Enumerables;

namespace DentalNova.Business.Rules
{
    public class RecordatorioBL : IRecordatorioBL
    {
        private readonly IRepository _repository;

        public RecordatorioBL(IRepository repository)
        {
            _repository = repository;
        }

        public async Task EnviarRecordatorioManualAsync(int citaId)
        {
            // Obtener información necesaria (Paciente y Doctor)
            var cita = await _repository.Cita.ObtenerPorIdAsync(citaId);

            if (cita == null)
                throw new Exception("La cita solicitada no existe.");

            // Validaciones
            if (cita.EstatusCita == EstatusCita.Cancelada)
                throw new Exception("No se pueden enviar recordatorios a citas canceladas.");

            if (cita.EstatusCita == EstatusCita.Completada)
                throw new Exception("Esta cita ya fue completada, no es necesario enviar recordatorio.");

            // Validar si ya existe un recordatorio previo para evitar duplicados
            var yaEnviado = await _repository.Recordatorio.ExisteRecordatorioParaCitaAsync(citaId);
            if (yaEnviado)
                throw new Exception("Ya se ha enviado un recordatorio para esta cita anteriormente.");

            // Mensaje Automático
            // Formato: "Hola Juan, le recordamos su cita programada para el Viernes 20 de Octubre a las 04:00 PM con el Dr. Pérez."
            var fechaTexto = cita.FechaHora.ToString("dddd dd 'de' MMMM 'a las' hh:mm tt");
            var nombrePaciente = cita.Paciente.Usuario.Nombre; // Asumiendo estructura Paciente->Usuario
            var nombreDoctor = cita.Odontologo.Usuario.Apellidos;

            var mensaje = $"Hola {nombrePaciente}, le recordamos su cita dental programada para el {fechaTexto} con el/la Dr(a). {nombreDoctor}.";

            var recordatorio = new Recordatorio
            {
                CitaId = citaId,
                Mensaje = mensaje,
                FechaEnvio = DateTime.Now,
                Enviado = true
            };

            await _repository.Recordatorio.AgregarAsync(recordatorio);
        }

        public async Task<List<RecordatorioDto>> ObtenerBuzonPacienteAsync(int pacienteId)
        {
            var listaEntidades = await _repository.Recordatorio.ObtenerPorPacienteAsync(pacienteId);

            // Mapeo a DTOs
            return listaEntidades.Select(r => r.ToDto()).ToList();
        }
    }
}
