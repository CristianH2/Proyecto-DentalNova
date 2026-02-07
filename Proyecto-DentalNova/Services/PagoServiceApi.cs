using DentalNova.Core.Dtos;
using DentalNova.Core.Interfaces;
using Microsoft.AspNetCore.WebUtilities;
using System.Net.Http.Headers;

namespace Proyecto_DentalNova.Services
{
    public class PagoServiceApi : IPagoService
    {
        private readonly HttpClient _httpClient;

        public PagoServiceApi(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }

        public async Task<PagedResultDto<PagoDto>> ObtenerListaPaginadaAsync(PagoFilterDto filtro)
        {
            var queryParams = new Dictionary<string, string?>
            {
                ["Page"] = filtro.Page.ToString(),
                ["PageSize"] = filtro.PageSize.ToString()
            };

            if (filtro.PacienteId.HasValue)
                queryParams.Add("PacienteId", filtro.PacienteId.ToString());

            if (filtro.FechaInicio.HasValue)
                queryParams.Add("FechaInicio", filtro.FechaInicio.Value.ToString("yyyy-MM-dd"));

            if (filtro.FechaFin.HasValue)
                queryParams.Add("FechaFin", filtro.FechaFin.Value.ToString("yyyy-MM-dd"));

            var url = QueryHelpers.AddQueryString("api/Pagos", queryParams);


            var resultado = await _httpClient.GetFromJsonAsync<PagedResultDto<PagoDto>>(url);
            return resultado ?? new PagedResultDto<PagoDto>();
        }

        public async Task<EstadoCuentaCitaDto> ObtenerEstadoCuentaCitaAsync(int citaId)
        {
            var resultado = await _httpClient.GetFromJsonAsync<EstadoCuentaCitaDto>($"api/Pagos/estado-cuenta/{citaId}");

            if (resultado == null) throw new Exception("No se pudo obtener el estado de cuenta.");
            return resultado;
        }

        public async Task RegistrarPagoAsync(PagoDtoIn dto)
        {
            var response = await _httpClient.PostAsJsonAsync("api/Pagos", dto);

            if (!response.IsSuccessStatusCode)
            {
                var error = await response.Content.ReadAsStringAsync();
                throw new HttpRequestException(error);
            }
        }
    }
}
