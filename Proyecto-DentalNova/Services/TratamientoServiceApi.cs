using DentalNova.Core.Dtos;
using DentalNova.Core.Helpers;
using DentalNova.Core.Interfaces;
using Microsoft.AspNetCore.WebUtilities;
using System.Net.Http.Headers;

namespace Proyecto_DentalNova.Services
{
    public class TratamientoServiceApi : ITratamientoService
    {
        private readonly HttpClient _httpClient;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public TratamientoServiceApi(HttpClient httpClient, IHttpContextAccessor httpContextAccessor)
        {
            _httpClient = httpClient;
            _httpContextAccessor = httpContextAccessor;
        }

        public async Task<PagedResultDto<TratamientoDto>> ObtenerTratamientosAdminAsync(TratamientoFilterDto filtro)
        {
            var queryParams = new Dictionary<string, string?>
            {
                ["Page"] = filtro.Page.ToString(),
                ["PageSize"] = filtro.PageSize.ToString()
            };

            if (filtro.Id.HasValue) queryParams.Add("Id", filtro.Id.ToString());
            if (!string.IsNullOrWhiteSpace(filtro.NombreLike)) queryParams.Add("NombreLike", filtro.NombreLike);
            if (filtro.CostoMin.HasValue) queryParams.Add("CostoMin", filtro.CostoMin.ToString());
            if (filtro.CostoMax.HasValue) queryParams.Add("CostoMax", filtro.CostoMax.ToString());
            if (filtro.DuracionMin.HasValue) queryParams.Add("DuracionMin", filtro.DuracionMin.ToString());
            if (filtro.DuracionMax.HasValue) queryParams.Add("DuracionMax", filtro.DuracionMax.ToString());
            if (filtro.Activo.HasValue) queryParams.Add("Activo", filtro.Activo.ToString());

            var url = QueryHelpers.AddQueryString("api/Tratamientos/admin", queryParams);

            await AddAuthorizationHeader();
            return await _httpClient.GetFromJsonAsync<PagedResultDto<TratamientoDto>>(url);
        }

        public async Task<TratamientoDto> ObtenerTratamientoPorIdAsync(int id)
        {
            await AddAuthorizationHeader();
            // Nota: El endpoint es 'admin/{id}' según definimos en el controlador API
            return await _httpClient.GetFromJsonAsync<TratamientoDto>($"api/Tratamientos/admin/{id}");
        }

        public async Task CrearTratamientoAsync(TratamientoDtoIn dto)
        {
            await AddAuthorizationHeader();
            var response = await _httpClient.PostAsJsonAsync("api/Tratamientos", dto);

            if (!response.IsSuccessStatusCode)
            {
                var error = await response.Content.ReadFromJsonAsync<Dictionary<string, string>>();
                var mensaje = error != null && error.ContainsKey("mensaje") ? error["mensaje"] : "Error al crear tratamiento";
                throw new HttpRequestException(mensaje);
            }
        }

        public async Task ActualizarTratamientoAsync(int id, TratamientoDtoIn dto)
        {
            await AddAuthorizationHeader();
            var response = await _httpClient.PutAsJsonAsync($"api/Tratamientos/{id}", dto);
            response.EnsureSuccessStatusCode();
        }

        public async Task EliminarTratamientoAsync(int id)
        {
            await AddAuthorizationHeader();
            var response = await _httpClient.DeleteAsync($"api/Tratamientos/{id}");
            response.EnsureSuccessStatusCode();
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
