using DentalNova.Business.Helpers;
using DentalNova.Core.Dtos;
using DentalNova.Core.Interfaces;
using DentalNova.Core.Repository.Entities;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace DentalNova.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    // [Authorize(Roles = "Administrador")]
    public class UsuariosController : ControllerBase
    {
        private readonly IUnitOfWork _unitOfWork;

        public UsuariosController(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        /// <summary>
        /// Obtiene una lista paginada de usuarios con filtros.
        /// </summary>
        [HttpGet]
        public async Task<ActionResult<PagedResultDto<UsuarioAdminDto>>> GetUsuarios([FromQuery] UsuarioFilterDto filtro)
        {
            // Lógica de negocio (que devuelve PaginatedList<Usuario>)
            var pagedList = await _unitOfWork.Usuario.ObtenerListaPaginadaAsync(filtro);

            // Lista de Entidades a lista de DTOs
            var dtos = pagedList.Select(u => u.ToAdminDto()).ToList();

            // Envuelve todo en el PagedResultDto
            var response = new PagedResultDto<UsuarioAdminDto>
            {
                Items = dtos,
                TotalCount = pagedList.TotalCount,
                TotalPages = pagedList.TotalPages,
                PageIndex = pagedList.PageIndex,
                HasNextPage = pagedList.HasNextPage,
                HasPreviousPage = pagedList.HasPreviousPage
            };

            return Ok(response);
        }

        /// <summary>
        /// Obtiene un usuario por su ID (para editar/detalles).
        /// </summary>
        [HttpGet("{id}")]
        public async Task<ActionResult<UsuarioAdminDto>> GetUsuario(int id)
        {
            var usuario = await _unitOfWork.Usuario.ObtenerPorIdAdminAsync(id);
            if (usuario == null) return NotFound();

            return Ok(usuario.ToAdminDto());
        }

        /// <summary>
        /// Crea un nuevo usuario (Admin).
        /// </summary>
        [HttpPost]
        public async Task<ActionResult> CreateUsuario(UsuarioAdminDtoIn dto)
        {
            // Validaciones de negocio (Email/CURP)
            if (await _unitOfWork.Usuario.EmailYaExisteAsync(dto.CorreoElectronico))
                return BadRequest("El correo electrónico ya está registrado.");

            if (await _unitOfWork.Usuario.CurpYaExisteAsync(dto.CURP))
                return BadRequest("La CURP ya está registrada.");

            if (string.IsNullOrEmpty(dto.Password))
                return BadRequest("La contraseña es obligatoria para nuevos usuarios.");

            // Crear entidad base
            var nuevoUsuario = new Usuario();
            nuevoUsuario.MapFromAdminDto(dto); // Usamos el mapeador

            // Llamar a la BL para crear (hashea pass y asigna roles)
            await _unitOfWork.Usuario.CrearUsuarioAdminAsync(nuevoUsuario, dto.Password, dto.Roles);

            return CreatedAtAction(nameof(GetUsuario), new { id = nuevoUsuario.Id }, nuevoUsuario.ToAdminDto());
        }

        /// <summary>
        /// Actualiza un usuario existente.
        /// </summary>
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateUsuario(int id, UsuarioAdminDtoIn dto)
        {
            if (id != dto.Id) return BadRequest("El ID no coincide.");

            // Validaciones de negocio (excluyendo al usuario actual)
            if (await _unitOfWork.Usuario.EmailYaExisteAsync(dto.CorreoElectronico, id))
                return BadRequest("El correo electrónico ya está en uso.");

            if (await _unitOfWork.Usuario.CurpYaExisteAsync(dto.CURP, id))
                return BadRequest("La CURP ya está en uso.");

            // Obtener usuario existente
            var usuarioExistente = await _unitOfWork.Usuario.ObtenerPorIdAdminAsync(id);
            if (usuarioExistente == null) return NotFound();

            // Actualizar propiedades
            usuarioExistente.MapFromAdminDto(dto);

            // Llamar a la BL para guardar (maneja password opcional y roles)
            await _unitOfWork.Usuario.ActualizarUsuarioAdminAsync(usuarioExistente, dto.Password, dto.Roles);

            return NoContent();
        }

        /// <summary>
        /// Elimina un usuario.
        /// </summary>
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteUsuario(int id)
        {
            await _unitOfWork.Usuario.EliminarUsuarioAsync(id);
            return NoContent();
        }

        /// <summary>
        /// Endpoint auxiliar para validar fecha de nacimiento (usado por el JS del MVC).
        /// </summary>
        [HttpGet("check-birthdate/{id}")]
        public async Task<IActionResult> GetFechaNacimiento(int id)
        {
            var fecha = await _unitOfWork.Usuario.ObtenerFechaNacimientoJsonAsync(id);
            return Ok(new { fechaNacimiento = fecha });
        }
    }
}
