using DentalNova.Core.Dtos;
using DentalNova.Core.Interfaces;
using Microsoft.AspNetCore.WebUtilities;
using System.Net.Http.Headers;

namespace Proyecto_DentalNova.Services
{
    public class OdontologoServiceApi : IOdontologoService
    {
        private readonly HttpClient _httpClient;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public OdontologoServiceApi(HttpClient httpClient, IHttpContextAccessor httpContextAccessor)
        {
            _httpClient = httpClient;
            _httpContextAccessor = httpContextAccessor;
        }

        public async Task<PagedResultDto<OdontologoDto>> ObtenerOdontologosAsync(OdontologoFilterDto filtro)
        {
            var queryParams = new Dictionary<string, string?>
            {
                ["Page"] = filtro.Page.ToString(),
                ["PageSize"] = filtro.PageSize.ToString()
            };

            // Mapeo de filtros
            if (filtro.Id.HasValue) queryParams.Add("Id", filtro.Id.ToString());
            if (!string.IsNullOrWhiteSpace(filtro.NombreLike)) queryParams.Add("NombreLike", filtro.NombreLike);
            if (!string.IsNullOrWhiteSpace(filtro.ApellidosLike)) queryParams.Add("ApellidosLike", filtro.ApellidosLike);
            if (!string.IsNullOrWhiteSpace(filtro.CorreoLike)) queryParams.Add("CorreoLike", filtro.CorreoLike);
            if (!string.IsNullOrWhiteSpace(filtro.CedulaLike)) queryParams.Add("CedulaLike", filtro.CedulaLike);
            if (filtro.EspecialidadId.HasValue) queryParams.Add("EspecialidadId", filtro.EspecialidadId.ToString());
            if (filtro.FechaIngresoDesde.HasValue) queryParams.Add("FechaIngresoDesde", filtro.FechaIngresoDesde.Value.ToString("yyyy-MM-dd"));
            if (filtro.FechaIngresoHasta.HasValue) queryParams.Add("FechaIngresoHasta", filtro.FechaIngresoHasta.Value.ToString("yyyy-MM-dd"));

            var url = QueryHelpers.AddQueryString("api/Odontologos", queryParams);

            await AddAuthorizationHeader();
            return await _httpClient.GetFromJsonAsync<PagedResultDto<OdontologoDto>>(url);
        }

        public async Task<OdontologoDto> ObtenerOdontologoPorIdAsync(int id)
        {
            await AddAuthorizationHeader();
            return await _httpClient.GetFromJsonAsync<OdontologoDto>($"api/Odontologos/{id}");
        }

        public async Task CrearOdontologoAsync(OdontologoDtoIn dto)
        {
            await AddAuthorizationHeader();
            var response = await _httpClient.PostAsJsonAsync("api/Odontologos", dto);
            if (!response.IsSuccessStatusCode)
            {
                var error = await response.Content.ReadFromJsonAsync<Dictionary<string, string>>();
                var mensaje = error != null && error.ContainsKey("mensaje") ? error["mensaje"] : "Error al crear odontólogo";
                throw new HttpRequestException(mensaje);
            }
        }

        public async Task ActualizarOdontologoAsync(int id, OdontologoDtoIn dto)
        {
            await AddAuthorizationHeader();
            var response = await _httpClient.PutAsJsonAsync($"api/Odontologos/{id}", dto);
            response.EnsureSuccessStatusCode();
        }

        public async Task EliminarOdontologoAsync(int id)
        {
            await AddAuthorizationHeader();
            var response = await _httpClient.DeleteAsync($"api/Odontologos/{id}");
            response.EnsureSuccessStatusCode();
        }

        public async Task<List<UsuarioDisponibleDto>> ObtenerUsuariosDisponiblesAsync(int? odontologoIdEdicion = null)
        {
            await AddAuthorizationHeader();
            var url = "api/Odontologos/usuarios-disponibles";
            if (odontologoIdEdicion.HasValue) url += $"?odontologoIdEdicion={odontologoIdEdicion}";
            return await _httpClient.GetFromJsonAsync<List<UsuarioDisponibleDto>>(url);
        }

        public async Task<List<EspecialidadDto>> ObtenerEspecialidadesAsync()
        {
            await AddAuthorizationHeader();
            return await _httpClient.GetFromJsonAsync<List<EspecialidadDto>>("api/Odontologos/especialidades");
        }

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
