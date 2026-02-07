using DentalNova.Core.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Proyecto_DentalNova.Controllers
{
    public class RecordatoriosController : Controller
    {
        private readonly IRecordatorioService _recordatorioService;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public RecordatoriosController(IRecordatorioService recordatorioService, IHttpContextAccessor httpContextAccessor)
        {
            _recordatorioService = recordatorioService;
            _httpContextAccessor = httpContextAccessor;
        }

        // GET: Recordatorios (Buzón de Mensajes del Paciente)
        public async Task<IActionResult> Index()
        {
            // OBTENER ID DEL PACIENTE LOGUEADO
            var pacienteId = HttpContext.Session.GetInt32("PacienteId");

            if (pacienteId == null)
            {
                // Si no es paciente o no está logueado correctamente
                TempData["MensajeError"] = "No se pudo identificar su expediente de paciente.";
                return RedirectToAction("Index", "Home");
            }

            try
            {
                var mensajes = await _recordatorioService.ObtenerMisMensajesAsync(pacienteId.Value);
                return View(mensajes);
            }
            catch (Exception)
            {
                TempData["MensajeError"] = "No se pudo cargar el buzón de mensajes.";
                return RedirectToAction("Index", "Home");
            }
        }

        // POST: Recordatorios/Enviar/5
        // Esta acción la llama el botón en Cita/Details.cshtml
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Administrador,Odontologo")]
        public async Task<IActionResult> Enviar(int citaId)
        {
            try
            {
                await _recordatorioService.EnviarRecordatorioAsync(citaId);
                TempData["MensajeExito"] = "Recordatorio enviado exitosamente al paciente.";
            }
            catch (Exception ex)
            {
                TempData["MensajeError"] = "No se pudo enviar";
            }

            return RedirectToAction("Details", "Cita", new { id = citaId });
        }
    }
}
