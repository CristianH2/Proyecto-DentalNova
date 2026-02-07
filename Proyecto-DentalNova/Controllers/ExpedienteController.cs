using DentalNova.Core.Dtos;
using DentalNova.Core.Helpers;
using DentalNova.Core.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Proyecto_DentalNova.Models.PacienteViewModel;

namespace Proyecto_DentalNova.Controllers
{
    [Authorize(Roles = "Odontologo")]
    public class ExpedienteController : Controller
    {
        private readonly IPacienteService _pacienteService;
        private readonly ICitaService _citaService;

        public ExpedienteController(IPacienteService pacienteService, ICitaService citaService)
        {
            _pacienteService = pacienteService;
            _citaService = citaService;
        }

        // GET: Expediente
        [HttpGet]
        public async Task<IActionResult> Index([Bind(Prefix = "Filtro")] PacienteFilterViewModel filtro)
        {
            var filtroDto = new PacienteFilterDto
            {
                Page = filtro.Page,
                PageSize = filtro.PageSize,
                Id = filtro.Id,
                NombreLike = filtro.NombreLike,
                ApellidosLike = filtro.ApellidosLike,
                ConAlergias = filtro.ConAlergias,
                ConEnfermedadesCronicas = filtro.ConEnfermedadesCronicas,
                ConAntecedentesFamiliares = filtro.ConAntecedentesFamiliares,
                ConMedicamentosActuales = filtro.ConMedicamentosActuales

            };

            var apiResult = await _pacienteService.ObtenerPacientesAsync(filtroDto);

            var pagedResults = PaginatedList<PacienteAdminDto>.Create(
                apiResult.Items,
                apiResult.TotalCount,
                apiResult.PageIndex,
                filtro.PageSize
            );

            var vm = new PacienteIndexViewModel
            {
                Filtro = filtro,
                Resultados = pagedResults
            };

            return View(vm);
        }

        // GET: Expediente/Details/5
        [HttpGet]
        public async Task<IActionResult> Details(int id)
        {

            var paciente = await _pacienteService.ObtenerPacientePorIdAsync(id);
            if (paciente == null) return NotFound();

            // Obtiene el historial de citas del paciente
            var filtroCitas = new CitaFilterDto
            {
                PacienteId = id,
                PageSize = 50
            };

            var citasResult = await _citaService.ObtenerListaPaginadaAsync(filtroCitas);
            var historial = citasResult.Items.OrderByDescending(c => c.FechaHora).ToList();

            var vm = new PacienteDetalleViewModel
            {
                Paciente = paciente,
                HistorialCitas = historial,
                TotalCitas = citasResult.TotalCount,
                UltimaVisita = historial.FirstOrDefault(c => c.EstatusCita == DentalNova.Core.Repository.Entities.Enumerables.EstatusCita.Completada)?.FechaHora
            };

            return View(vm);
        }
        
    }
}
