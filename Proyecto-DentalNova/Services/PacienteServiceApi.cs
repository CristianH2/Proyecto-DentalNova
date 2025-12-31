using DentalNova.Core.Dtos;
using DentalNova.Core.Interfaces;
using Microsoft.AspNetCore.WebUtilities;
using System.Net.Http.Headers;

namespace Proyecto_DentalNova.Services
{
    public class PacienteServiceApi : IPacienteService
    {
        private readonly HttpClient _httpClient;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public PacienteServiceApi(HttpClient httpClient, IHttpContextAccessor httpContextAccessor)
        {
            _httpClient = httpClient;
            _httpContextAccessor = httpContextAccessor;
        }

        public async Task<PagedResultDto<PacienteAdminDto>> ObtenerPacientesAsync(PacienteFilterDto filtro)
        {
            // Convertir filtro a Query String
            var queryParams = new Dictionary<string, string?>
            {
                ["Page"] = filtro.Page.ToString(),
                ["PageSize"] = filtro.PageSize.ToString()
            };

            if (filtro.Id.HasValue) queryParams.Add("Id", filtro.Id.ToString());
            if (!string.IsNullOrWhiteSpace(filtro.NombreLike)) queryParams.Add("NombreLike", filtro.NombreLike);
            if (!string.IsNullOrWhiteSpace(filtro.ApellidosLike)) queryParams.Add("ApellidosLike", filtro.ApellidosLike);
            if (!string.IsNullOrWhiteSpace(filtro.CorreoLike)) queryParams.Add("CorreoLike", filtro.CorreoLike);
            if (!string.IsNullOrWhiteSpace(filtro.TelefonoLike)) queryParams.Add("TelefonoLike", filtro.TelefonoLike);
            if (filtro.EdadMin.HasValue) queryParams.Add("EdadMin", filtro.EdadMin.ToString());
            if (filtro.EdadMax.HasValue) queryParams.Add("EdadMax", filtro.EdadMax.ToString());
            if (filtro.FechaDesde.HasValue) queryParams.Add("FechaDesde", filtro.FechaDesde.Value.ToString("o"));
            if (filtro.FechaHasta.HasValue) queryParams.Add("FechaHasta", filtro.FechaHasta.Value.ToString("o"));
            if (filtro.ConAlergias) queryParams.Add("ConAlergias", filtro.ConAlergias.ToString());
            if (filtro.ConEnfermedadesCronicas) queryParams.Add("ConEnfermedadesCronicas", filtro.ConEnfermedadesCronicas.ToString());
            if (filtro.ConMedicamentosActuales) queryParams.Add("ConMedicamentosActuales", filtro.ConMedicamentosActuales.ToString());
            if (filtro.ConAntecedentesFamiliares) queryParams.Add("ConAntecedentesFamiliares", filtro.ConAntecedentesFamiliares.ToString());

            var url = QueryHelpers.AddQueryString("api/Pacientes", queryParams);

            await AddAuthorizationHeader();
            return await _httpClient.GetFromJsonAsync<PagedResultDto<PacienteAdminDto>>(url);
        }

        public async Task<PacienteAdminDto> ObtenerPacientePorIdAsync(int id)
        {
            await AddAuthorizationHeader();
            return await _httpClient.GetFromJsonAsync<PacienteAdminDto>($"api/Pacientes/{id}");
        }

        public async Task CrearPacienteAsync(PacienteAdminDtoIn dto)
        {
            await AddAuthorizationHeader();
            var response = await _httpClient.PostAsJsonAsync("api/Pacientes", dto);

            if (!response.IsSuccessStatusCode)
            {
                var error = await response.Content.ReadFromJsonAsync<Dictionary<string, string>>();
                var mensaje = error != null && error.ContainsKey("mensaje") ? error["mensaje"] : "Error al crear paciente";
                throw new HttpRequestException(mensaje);
            }
        }

        public async Task ActualizarPacienteAsync(int id, PacienteAdminDtoIn dto)
        {
            await AddAuthorizationHeader();
            var response = await _httpClient.PutAsJsonAsync($"api/Pacientes/{id}", dto);
            response.EnsureSuccessStatusCode();
        }

        public async Task EliminarPacienteAsync(int id)
        {
            await AddAuthorizationHeader();
            var response = await _httpClient.DeleteAsync($"api/Pacientes/{id}");
            response.EnsureSuccessStatusCode();
        }

        public async Task<List<UsuarioDisponibleDto>> ObtenerUsuariosDisponiblesAsync(int? pacienteIdEdicion = null)
        {
            await AddAuthorizationHeader();
            var url = "api/Pacientes/usuarios-disponibles";
            if (pacienteIdEdicion.HasValue)
            {
                url += $"?pacienteIdEdicion={pacienteIdEdicion}";
            }
            return await _httpClient.GetFromJsonAsync<List<UsuarioDisponibleDto>>(url);
        }

        // Método privado para Token (Cópialo de UsuarioServiceApi o crea una clase base)
        private async Task AddAuthorizationHeader()
        {
            var user = _httpContextAccessor.HttpContext?.User;
            if (user != null && user.Identity.IsAuthenticated)
            {
                var token = user.FindFirst("JWT_TOKEN")?.Value;
                if (!string.IsNullOrEmpty(token))
                {
                    _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
                }
            }
        }
    }
}
