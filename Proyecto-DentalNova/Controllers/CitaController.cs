using DentalNova.Core.Dtos;
using DentalNova.Core.Helpers;
using DentalNova.Core.Interfaces;
using DentalNova.Core.Repository.Entities;
using DentalNova.Repository.DataContext;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Proyecto_DentalNova.Extensions;
using Proyecto_DentalNova.Models.CitaViewModel;
using static DentalNova.Core.Repository.Entities.Enumerables;

namespace Proyecto_DentalNova.Controllers
{
    public class CitaController : Controller
    {
        private readonly ICitaService _citaService;
        private readonly IPacienteService _pacienteService;
        private readonly IOdontologoService _odontologoService;
        private readonly ITratamientoService _tratamientoService;
        private readonly IHorarioOdontologoService _horarioService;

        public CitaController(
            ICitaService citaService,
            IPacienteService pacienteService,
            IOdontologoService odontologoService,
            ITratamientoService tratamientoService,
            IHorarioOdontologoService horarioService)
        {
            _citaService = citaService;
            _pacienteService = pacienteService;
            _odontologoService = odontologoService;
            _tratamientoService = tratamientoService;
            _horarioService = horarioService;
        }

        // --- Helpers Privados para llenar listas ---

        private async Task<CitaVM> ConstruirViewModelAsync(CitaVM? vmExistente = null)
        {
            var vm = vmExistente ?? new CitaVM();

            // Cargar Pacientes
            var filtroPacientes = new PacienteFilterDto { PageSize = 1000 };
            var pacientesResult = await _pacienteService.ObtenerPacientesAsync(filtroPacientes);

            vm.Pacientes = pacientesResult.Items.Select(p => new SelectListItem
            {
                Value = p.Id.ToString(),
                Text = $"{p.Nombre} {p.Apellidos} (Edad: {p.Edad})"
            });

            // Cargar Odontólogos
            var filtroOdontologos = new OdontologoFilterDto { PageSize = 1000 };
            var odontologosResult = await _odontologoService.ObtenerOdontologosAsync(filtroOdontologos);

            vm.Odontologos = odontologosResult.Items.Select(o => new SelectListItem
            {
                Value = o.Id.ToString(),
                Text = $"{o.Nombre} {o.Apellidos} - {o.CedulaProfesional}"
            });

            // Cargar Tratamientos Activos
            var filtroTratamientos = new TratamientoFilterDto { Activo = true, PageSize = 1000 };
            var tratamientosResult = await _tratamientoService.ObtenerTratamientosAdminAsync(filtroTratamientos);

            vm.Tratamientos = tratamientosResult.Items.Select(t => new SelectListItem
            {
                Value = t.Id.ToString(),
                Text = $"{t.Nombre} (${t.Costo:N2})"
            });

            // Cargar Enums (Duración y Estatus)
            vm.Duraciones = Enum.GetValues(typeof(DuracionMinutos)).Cast<DuracionMinutos>()
                .Select(d => new SelectListItem { Value = d.ToString(), Text = $"{(int)d} Minutos" });

            // 'Programada' por defecto al crear
            vm.Estatus = new List<SelectListItem> { new SelectListItem { Value = "Programada", Text = "Programada", Selected = true } };

            return vm;
        }

        // --- GET: Cita/Create ---
        [HttpGet]
        [Authorize(Roles = "Administrador")]
        public async Task<IActionResult> Create()
        {
            var vm = await ConstruirViewModelAsync();
            return View(vm);
        }

        // --- POST: Cita/Create ---
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Administrador")]
        public async Task<IActionResult> Create(CitaVM model)
        {
            // Asignamos los tratamientos seleccionados
            model.Cita.TratamientosIds = model.TratamientosSeleccionados;

            // Validaciones
            if (model.Cita.PacienteId <= 0) ModelState.AddModelError("Cita.PacienteId", "Seleccione un paciente.");
            if (model.Cita.OdontologoId <= 0) ModelState.AddModelError("Cita.OdontologoId", "Seleccione un odontólogo.");

            if (!ModelState.IsValid)
            {
                var vmRecargado = await ConstruirViewModelAsync(model);
                return View(vmRecargado);
            }

            try
            {
                var id = await _citaService.CrearAsync(model.Cita);

                TempData["MensajeExito"] = "Cita agendada correctamente.";
                return RedirectToAction(nameof(Details), new { id });
            }
            catch (Exception ex)
            {
                TempData["MensajeError"] = ex.Message;
                var vmRecargado = await ConstruirViewModelAsync(model);
                return View(vmRecargado);
            }
        }

        // --- GET: Cita/Details/5 ---
        [HttpGet]
        [Authorize(Roles = "Administrador,Odontologo")]
        public async Task<IActionResult> Details(int id)
        {
            try
            {
                var cita = await _citaService.ObtenerPorIdAsync(id);
                if (cita == null) return NotFound();
                return View(cita);
            }
            catch
            {
                return NotFound();
            }
        }

        // --- POST: Cita/Cancelar/5 ---
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Administrador")]
        public async Task<IActionResult> Cancelar(int id)
        {
            try
            {
                await _citaService.CambiarEstatusAsync(id, EstatusCita.Cancelada);
                TempData["MensajeExito"] = "La cita ha sido cancelada.";
            }
            catch (Exception ex)
            {
                TempData["MensajeError"] = "Error al cancelar: " + ex.Message;
            }
            return RedirectToAction(nameof(Details), new { id });
        }

        // --- POST: Cita/Completar/5 ---
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Administrador")]
        public async Task<IActionResult> Completar(int id)
        {
            try
            {
                await _citaService.CambiarEstatusAsync(id, EstatusCita.Completada);
                TempData["MensajeExito"] = "La cita se marcó como completada.";
            }
            catch (Exception ex)
            {
                TempData["MensajeError"] = "Error al completar: " + ex.Message;
            }
            return RedirectToAction(nameof(Details), new { id });
        }

        // --- GET: Cita ---
        [HttpGet]
        [Authorize(Roles = "Administrador")]
        public async Task<IActionResult> Index([Bind(Prefix = "Filtro")] CitaFilterDto filtro)
        {
            if (filtro.Page < 1) filtro.Page = 1;
            if (filtro.PageSize < 1) filtro.PageSize = 10;

            // Mostrar desde hoy
            //if (!filtro.FechaInicio.HasValue && !filtro.FechaFin.HasValue) filtro.FechaInicio = DateTime.Today; 

            // Llamar API
            var apiResult = await _citaService.ObtenerListaPaginadaAsync(filtro);

            // Crear lista paginada manual
            var pagedResults = PaginatedList<CitaDto>.Create(
                apiResult.Items,
                apiResult.TotalCount,
                filtro.Page, // Usamos la página solicitada
                filtro.PageSize
            );

            // Cargar Listas para los filtros
            var odontologosDto = await _odontologoService.ObtenerOdontologosAsync(new OdontologoFilterDto { PageSize = 1000 });
            var pacientesDto = await _pacienteService.ObtenerPacientesAsync(new PacienteFilterDto { PageSize = 1000 });

            // Construir ViewModel
            var vm = new CitaIndexViewModel
            {
                Filtro = filtro,
                Resultados = pagedResults,

                Odontologos = odontologosDto.Items.Select(o => new SelectListItem
                {
                    Value = o.Id.ToString(),
                    Text = $"{o.Nombre} {o.Apellidos}"
                }),
                Pacientes = pacientesDto.Items.Select(p => new SelectListItem
                {
                    Value = p.Id.ToString(),
                    Text = $"{p.Nombre} {p.Apellidos}"
                }),
                Estatus = Enum.GetValues(typeof(EstatusCita)).Cast<EstatusCita>()
                    .Select(e => new SelectListItem { Value = ((int)e).ToString(), Text = e.ToString() })
            };

            return View(vm);
        }

        // --- POST: Cita/Delete/5 ---
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Administrador")]
        public async Task<IActionResult> Delete(int id)
        {
            try
            {
                await _citaService.EliminarAsync(id); // Soft Delete
                TempData["MensajeExito"] = "La cita ha sido eliminada del listado activo.";
            }
            catch (Exception ex)
            {
                TempData["MensajeError"] = "No se pudo eliminar la cita: " + ex.Message;
            }
            return RedirectToAction(nameof(Index));
        }

        // --- GET: Cita/Edit/5 ---
        [HttpGet]
        [Authorize(Roles = "Administrador")]
        public async Task<IActionResult> Edit(int id)
        {
            try
            {
                // Obtener la cita actual
                var citaDto = await _citaService.ObtenerPorIdAsync(id);
                if (citaDto == null) return NotFound();

                // DTO de Salida -> ViewModel de Entrada
                var vm = new CitaVM
                {
                    Cita = new CitaDtoIn
                    {
                        Id = citaDto.Id,
                        PacienteId = citaDto.PacienteId,
                        OdontologoId = citaDto.OdontologoId,
                        FechaHora = citaDto.FechaHora,
                        DuracionMinutos = citaDto.DuracionMinutos,
                        MotivoConsulta = citaDto.MotivoConsulta,
                        EstatusCita = citaDto.EstatusCita
                    },

                    TratamientosSeleccionados = citaDto.Tratamientos.Select(t => t.TratamientoId).ToList()
                };

                // Cargar las listas desplegables
                vm = await ConstruirViewModelAsync(vm);

                return View(vm);
            }
            catch (Exception ex)
            {
                TempData["MensajeError"] = "No se pudo cargar la cita para edición: " + ex.Message;
                return RedirectToAction(nameof(Index));
            }
        }

        // --- POST: Cita/Edit/5 ---
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Administrador")]
        public async Task<IActionResult> Edit(int id, CitaVM model)
        {
            if (id != model.Cita.Id) return BadRequest();

            model.Cita.TratamientosIds = model.TratamientosSeleccionados;

            if (!ModelState.IsValid)
            {
                var vmRecargado = await ConstruirViewModelAsync(model);
                return View(vmRecargado);
            }

            try
            {
                await _citaService.ActualizarAsync(id, model.Cita);
                TempData["MensajeExito"] = "La cita ha sido actualizada correctamente.";
                return RedirectToAction(nameof(Details), new { id });
            }
            catch (Exception ex)
            {
                TempData["MensajeError"] = ex.Message;
                var vmRecargado = await ConstruirViewModelAsync(model);
                return View(vmRecargado);
            }
        }

        // --- POST: Cita/NoAsistida/5 ---
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Administrador,Odontologo")]
        public async Task<IActionResult> NoAsistida(int id)
        {
            try
            {
                await _citaService.CambiarEstatusAsync(id, EstatusCita.NoAsistida);
                TempData["MensajeExito"] = "La cita se marcó como NO ASISTIDA.";
            }
            catch (Exception ex)
            {
                TempData["MensajeError"] = "Error al actualizar estatus: " + ex.Message;
            }
            return RedirectToAction(nameof(Details), new { id });
        }

        // --- GET: GetHorariosOdontologo/5 ---
        [HttpGet]
        [Authorize(Roles = "Administrador,Odontologo")]
        public async Task<IActionResult> GetHorariosOdontologo(int id)
        {
            try
            {
                var horarios = await _horarioService.ObtenerPorOdontologoAsync(id);

                // Proyectamos a un objeto anónimo simple para el JSON
                var resultado = horarios
                    .OrderBy(h => h.DiaSemana)
                    .ThenBy(h => h.HoraInicio)
                    .Select(h => new {
                        dia = h.DiaSemana.ToString(),
                        horario = $"{h.HoraInicio:hh\\:mm} - {h.HoraFin:hh\\:mm}",
                        consultorio = h.Consultorio
                    });

                return Json(resultado);
            }
            catch
            {
                return Json(new List<object>());
            }
        }

        // --- POST: Cita/Reactivar/5 ---
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Administrador")]
        public async Task<IActionResult> Reactivar(int id)
        {
            try
            {
                await _citaService.CambiarEstatusAsync(id, EstatusCita.Programada);
                TempData["MensajeExito"] = "La cita ha sido reactivada correctamente.";
            }
            catch (Exception ex)
            {
                TempData["MensajeError"] = ex.Message;
            }
            return RedirectToAction(nameof(Details), new { id });
        }

        // --- GET: Cita/GetEventosOdontologo ---
        [HttpGet]
        [Authorize(Roles = "Administrador,Odontologo")]
        public async Task<IActionResult> GetEventosOdontologo(int odontologoId, DateTime start, DateTime end)
        {
            // Crear filtro para el rango de fechas que pide el calendario
            var filtro = new CitaFilterDto
            {
                OdontologoId = odontologoId,
                FechaInicio = start,
                FechaFin = end,
                Page = 1,
                PageSize = 1000
            };

            var resultado = await _citaService.ObtenerListaPaginadaAsync(filtro);

            // Mapear a formato FullCalendar
            var eventos = resultado.Items
                .Where(c => c.EstatusCita != EstatusCita.Cancelada) // No mostrar canceladas
                .Select(c => new
                {
                    id = c.Id,
                    // Mostramos Nombre + Estatus en el título
                    title = $"{c.PacienteNombre} ({c.EstatusTexto})",
                    start = c.FechaHora.ToString("yyyy-MM-ddTHH:mm:ss"),
                    end = c.FechaHora.AddMinutes((double)c.DuracionMinutos).ToString("yyyy-MM-ddTHH:mm:ss"),

                    backgroundColor = c.EstatusCita switch
                    {
                        EstatusCita.Programada => "#0d6efd", // Azul
                        EstatusCita.Completada => "#198754", // Verde
                        EstatusCita.Cancelada => "#dc3545",  // Rojo
                        EstatusCita.NoAsistida => "#ffc107", // Amarillo 
                        _ => "#6c757d"                       // Gris (Default)
                    },

                    borderColor = c.EstatusCita switch
                    {
                        EstatusCita.Programada => "#0d6efd",
                        EstatusCita.Completada => "#198754",
                        EstatusCita.Cancelada => "#dc3545",
                        EstatusCita.NoAsistida => "#ffc107",
                        _ => "#6c757d"
                    },

                    // El amarillo con letras negras
                    textColor = c.EstatusCita == EstatusCita.NoAsistida ? "#000000" : "#ffffff"
                });

            return Json(eventos);
        }

        // -----------------------------------------------
        // Controladores para los Odontólogos
        // -----------------------------------------------

        // GET: Cita/MisCitas
        [Authorize(Roles = "Odontologo")]
        public async Task<IActionResult> MisCitas([Bind(Prefix = "Filtro")] CitaFilterDto filtro)
        {
            // Obtener el ID del odontólogo desde el Token
            var odontologoId = User.GetOdontologoId();

            if (!odontologoId.HasValue)
            {
                TempData["MensajeError"] = "No se pudo identificar su perfil profesional.";
                return RedirectToAction("Index", "Home");
            }

            filtro.OdontologoId = odontologoId.Value;
            if (filtro.Page < 1) filtro.Page = 1;
            if (filtro.PageSize < 1) filtro.PageSize = 10;
            if (!filtro.FechaInicio.HasValue && !filtro.FechaFin.HasValue)
            {
                filtro.FechaInicio = DateTime.Today;
            }

            // Obtener datos
            var apiResult = await _citaService.ObtenerListaPaginadaAsync(filtro);

            var pagedResults = PaginatedList<CitaDto>.Create(
                apiResult.Items,
                apiResult.TotalCount,
                filtro.Page,
                filtro.PageSize
            );

            // Cargar filtros
            var pacientesDto = await _pacienteService.ObtenerPacientesAsync(new PacienteFilterDto { PageSize = 1000 });

            var vm = new CitaIndexViewModel
            {
                Filtro = filtro,
                Resultados = pagedResults,

                // Lista de pacientes para el buscador
                Pacientes = pacientesDto.Items.Select(p => new SelectListItem
                {
                    Value = p.Id.ToString(),
                    Text = $"{p.Nombre} {p.Apellidos}"
                }),

                // Estatus
                Estatus = Enum.GetValues(typeof(EstatusCita)).Cast<EstatusCita>()
                    .Select(e => new SelectListItem { Value = ((int)e).ToString(), Text = e.ToString() })
            };

            return View(vm);
        }

        // --- GET: Cita/Agendar ---
        [HttpGet]
        [Authorize(Roles = "Odontologo")]
        public async Task<IActionResult> Agendar(int? pacienteId)
        {
            var odontologoId = User.GetOdontologoId();
            if (!odontologoId.HasValue) return RedirectToAction("Index", "Home");

            var vm = new CitaVM();

            vm.Cita.OdontologoId = odontologoId.Value;

            if (pacienteId.HasValue)
            {
                vm.Cita.PacienteId = pacienteId.Value;
            }

            // Cargar Listas
            var filtroPacientes = new PacienteFilterDto { PageSize = 1000 };
            var pacientesResult = await _pacienteService.ObtenerPacientesAsync(filtroPacientes);
            vm.Pacientes = pacientesResult.Items.Select(p => new SelectListItem
            {
                Value = p.Id.ToString(),
                Text = $"{p.Nombre} {p.Apellidos}",
                Selected = pacienteId.HasValue && p.Id == pacienteId.Value
            });

            // Odontólogo
            vm.Odontologos = new List<SelectListItem>();

            // Tratamientos
            var filtroTratamientos = new TratamientoFilterDto { Activo = true, PageSize = 1000 };
            var tratamientosResult = await _tratamientoService.ObtenerTratamientosAdminAsync(filtroTratamientos);
            vm.Tratamientos = tratamientosResult.Items.Select(t => new SelectListItem
            {
                Value = t.Id.ToString(),
                Text = $"{t.Nombre}"
            });

            vm.Duraciones = Enum.GetValues(typeof(DuracionMinutos)).Cast<DuracionMinutos>()
                .Select(d => new SelectListItem { Value = d.ToString(), Text = $"{(int)d} Minutos" });

            vm.Estatus = new List<SelectListItem> { new SelectListItem { Value = "Programada", Text = "Programada", Selected = true } };

            return View(vm);
        }

        // --- POST: Cita/Agendar ---
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Odontologo")]
        public async Task<IActionResult> Agendar(CitaVM model)
        {
            // ID del odontólogo logueado
            var odontologoId = User.GetOdontologoId();
            if (!odontologoId.HasValue) return RedirectToAction("Index", "Home");

            model.Cita.OdontologoId = odontologoId.Value;
            model.Cita.TratamientosIds = model.TratamientosSeleccionados;

            // Validaciones
            if (model.Cita.PacienteId <= 0) ModelState.AddModelError("Cita.PacienteId", "Seleccione un paciente.");

            if (!ModelState.IsValid)
            {
                return await Agendar(model.Cita.PacienteId);
            }

            try
            {
                var id = await _citaService.CrearAsync(model.Cita);
                TempData["MensajeExito"] = "Cita de seguimiento agendada correctamente.";
                return RedirectToAction("MisCitas");
            }
            catch (Exception ex)
            {
                TempData["MensajeError"] = ex.Message;
                return await Agendar(model.Cita.PacienteId);
            }
        }


        // GET: Cita/GetMisEventos (Para el calendario de "Mis Citas")
        [HttpGet]
        [Authorize(Roles = "Odontologo")]
        public async Task<IActionResult> GetMisEventos(DateTime start, DateTime end)
        {
            var odontologoId = User.GetOdontologoId();
            if (!odontologoId.HasValue) return Json(new List<object>());
            return await GetEventosOdontologo(odontologoId.Value, start, end);
        }








    }
}
