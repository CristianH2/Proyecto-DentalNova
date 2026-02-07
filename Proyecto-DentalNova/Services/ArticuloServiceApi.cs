using DentalNova.Core.Dtos;
using DentalNova.Core.Interfaces;
using Microsoft.AspNetCore.WebUtilities;
using System.Net.Http.Headers;

namespace Proyecto_DentalNova.Services
{
    public class ArticuloServiceApi : IArticuloService
    {
        private readonly HttpClient _httpClient;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public ArticuloServiceApi(HttpClient httpClient, IHttpContextAccessor httpContextAccessor)
        {
            _httpClient = httpClient;
            _httpContextAccessor = httpContextAccessor;
        }

        public async Task<PagedResultDto<ArticuloDto>> ObtenerListaPaginadaAsync(ArticuloFilterDto filtro)
        {
            // Construcción manual de parámetros QueryString
            var queryParams = new Dictionary<string, string?>
            {
                ["Page"] = filtro.Page.ToString(),
                ["PageSize"] = filtro.PageSize.ToString()
            };

            if (!string.IsNullOrWhiteSpace(filtro.Busqueda))
            {
                queryParams.Add("Busqueda", filtro.Busqueda);
            }

            if (filtro.Categoria.HasValue)
            {
                // Convertimos el Enum a int para enviarlo
                queryParams.Add("Categoria", ((int)filtro.Categoria.Value).ToString());
            }

            if (filtro.Activo.HasValue)
            {
                queryParams.Add("Activo", filtro.Activo.Value.ToString());
            }

            // Construir URL final
            var url = QueryHelpers.AddQueryString("api/Articulos", queryParams);

            //await AddAuthorizationHeader();

            // Usamos un PagedResultDto vacío en caso de null para evitar NullReferenceException
            var resultado = await _httpClient.GetFromJsonAsync<PagedResultDto<ArticuloDto>>(url);
            return resultado ?? new PagedResultDto<ArticuloDto>();
        }

        public async Task<ArticuloDtoIn> ObtenerParaEditarAsync(int id)
        {
            var resultado = await _httpClient.GetFromJsonAsync<ArticuloDtoIn>($"api/Articulos/{id}");

            if (resultado == null) throw new Exception("No se pudieron cargar los datos del artículo.");
            return resultado;
        }

        public async Task CrearAsync(ArticuloDtoIn dto)
        {
            var response = await _httpClient.PostAsJsonAsync("api/Articulos", dto);

            if (!response.IsSuccessStatusCode)
            {
                // Leemos el mensaje de error que envía el API
                var error = await response.Content.ReadAsStringAsync();
                throw new HttpRequestException(error);
            }
        }

        public async Task ActualizarAsync(ArticuloDtoIn dto)
        {
            var response = await _httpClient.PutAsJsonAsync($"api/Articulos/{dto.Id}", dto);

            if (!response.IsSuccessStatusCode)
            {
                var error = await response.Content.ReadAsStringAsync();
                throw new HttpRequestException(error);
            }
        }

        public async Task EliminarAsync(int id)
        {
            var response = await _httpClient.DeleteAsync($"api/Articulos/{id}");

            if (!response.IsSuccessStatusCode)
            {
                var error = await response.Content.ReadAsStringAsync();
                throw new HttpRequestException(error);
            }
        }

        public async Task CambiarEstatusAsync(int id)
        {
            var response = await _httpClient.PostAsync($"api/Articulos/{id}/estatus", null);

            if (!response.IsSuccessStatusCode)
            {
                var error = await response.Content.ReadAsStringAsync();
                throw new HttpRequestException(error);
            }
        }
    }
}
