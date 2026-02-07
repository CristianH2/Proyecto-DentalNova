using DentalNova.Core.Dtos;
using DentalNova.Core.Interfaces;
using DentalNova.Core.Repository.Entities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Proyecto_DentalNova.Extensions;
using Proyecto_DentalNova.Models.ConsultaViewModel;

namespace Proyecto_DentalNova.Controllers
{
    [Authorize(Roles = "Odontologo")]
    public class ConsultaController : Controller
    {
        private readonly ICitaService _citaService;
        private readonly IPacienteService _pacienteService;

        public ConsultaController(ICitaService citaService, IPacienteService pacienteService)
        {
            _citaService = citaService;
            _pacienteService = pacienteService;
        }

        // GET: Consulta/Atender/5
        public async Task<IActionResult> Atender(int id)
        {
            // Obtener la cita
            var cita = await _citaService.ObtenerPorIdAsync(id);
            if (cita == null) return NotFound();

            // Verificar que la cita pertenezca al odontólogo logueado
            var odontologoId = User.GetOdontologoId();
            if (odontologoId.HasValue && cita.OdontologoId != odontologoId.Value)
            {
                return Forbid(); // Retorna 403 Prohibido si intenta ver cita ajena
            }

            // Verificar que la cita esté vigente (No cancelada ni completada)
            if (cita.EstatusCita == Enumerables.EstatusCita.Cancelada)
            {
                TempData["MensajeError"] = "Esta cita está cancelada y no se puede atender.";
                return RedirectToAction("MisCitas", "Cita");
            }

            if (cita.EstatusCita == Enumerables.EstatusCita.Completada) {}

            // Obtener Expediente del Paciente (Alergias, Antecedentes)
            var paciente = await _pacienteService.ObtenerPacientePorIdAsync(cita.PacienteId);

            var vm = new ConsultaViewModel
            {
                Cita = cita,
                Paciente = paciente,
                NotasEvolucion = cita.MotivoConsulta
            };

            return View(vm);
        }

        // POST: Consulta/Finalizar
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Finalizar(int id, ConsultaViewModel model)
        {
            if (id != model.Cita.Id) return BadRequest();

            // Limpiar ModelState de campos que no enviamos en el form
            ModelState.Remove("Paciente");
            ModelState.Remove("Cita.PacienteNombre");
            ModelState.Remove("Cita.OdontologoNombre");
            ModelState.Remove("Cita.OdontologoColor");
            ModelState.Remove("Cita.MotivoConsulta"); // Lo usamos para notas

            // Para los tratamientos (lista)
            foreach (var key in ModelState.Keys.Where(k => k.Contains("TratamientoNombre") || k.Contains("Paciente")).ToList())
            {
                ModelState.Remove(key);
            }

            if (!ModelState.IsValid)
            {
                // Si falta la nota
                model.Paciente = await _pacienteService.ObtenerPacientePorIdAsync(model.Cita.PacienteId);
                model.Cita = await _citaService.ObtenerPorIdAsync(id);
                TempData["MensajeError"] = "Por favor, complete todos los campos obligatorios.";
                return View("Atender", model);
            }

            var odontologoId = User.GetOdontologoId();

            // Guardamos la nota
            var citaUpdate = new CitaDtoIn
            {
                Id = id,
                PacienteId = model.Cita.PacienteId,
                OdontologoId = model.Cita.OdontologoId,
                FechaHora = model.Cita.FechaHora,
                DuracionMinutos = model.Cita.DuracionMinutos,

                // Notas realizadas por el odontólogo
                MotivoConsulta = $"NOTA: {model.NotasEvolucion} \nRECOMENDACIONES: {model.Recomendaciones}",
                EstatusCita = Enumerables.EstatusCita.Completada,
                TratamientosIds = model.Cita.Tratamientos.Select(t => t.TratamientoId).ToList()
            };

            try
            {
                // Actualizar datos (Nota y Estatus)
                await _citaService.ActualizarAsync(id, citaUpdate);
                await _citaService.CambiarEstatusAsync(id, Enumerables.EstatusCita.Completada);

                TempData["MensajeExito"] = "Consulta finalizada exitosamente. El historial ha sido actualizado.";
                return RedirectToAction("MisCitas", "Cita");
            }
            catch (Exception ex)
            {
                TempData["MensajeError"] = "Error al finalizar: " + ex.Message;
                model.Paciente = await _pacienteService.ObtenerPacientePorIdAsync(model.Cita.PacienteId);
                model.Cita = await _citaService.ObtenerPorIdAsync(id);
                return View("Atender", model);
            }
        }
    }
}
