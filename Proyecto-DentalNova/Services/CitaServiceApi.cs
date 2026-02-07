using DentalNova.Core.Dtos;
using DentalNova.Core.Interfaces;
using Microsoft.AspNetCore.WebUtilities;
using System.Net.Http.Headers;
using static DentalNova.Core.Repository.Entities.Enumerables;

namespace Proyecto_DentalNova.Services
{
    public class CitaServiceApi : ICitaService
    {
        private readonly HttpClient _httpClient;

        public CitaServiceApi(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }

        public async Task<PagedResultDto<CitaDto>> ObtenerListaPaginadaAsync(CitaFilterDto filtro)
        {
            // Construcción manual de parámetros
            var queryParams = new Dictionary<string, string?>
            {
                ["Page"] = filtro.Page.ToString(),
                ["PageSize"] = filtro.PageSize.ToString()
            };

            // Agregar filtros opcionales solo si tienen valor
            if (filtro.FechaInicio.HasValue)
                queryParams.Add("FechaInicio", filtro.FechaInicio.Value.ToString("yyyy-MM-dd"));

            if (filtro.FechaFin.HasValue)
                queryParams.Add("FechaFin", filtro.FechaFin.Value.ToString("yyyy-MM-dd"));

            if (filtro.PacienteId.HasValue)
                queryParams.Add("PacienteId", filtro.PacienteId.ToString());

            if (filtro.OdontologoId.HasValue)
                queryParams.Add("OdontologoId", filtro.OdontologoId.ToString());

            // Convertimos el Enum a int para enviarlo
            if (filtro.Estatus.HasValue)
                queryParams.Add("Estatus", ((int)filtro.Estatus.Value).ToString());

            var url = QueryHelpers.AddQueryString("api/Citas", queryParams);


            // Llamada
            var resultado = await _httpClient.GetFromJsonAsync<PagedResultDto<CitaDto>>(url);
            return resultado ?? new PagedResultDto<CitaDto>();
        }

        public async Task<CitaDto> ObtenerPorIdAsync(int id)
        {
            return await _httpClient.GetFromJsonAsync<CitaDto>($"api/Citas/{id}");
        }

        public async Task<int> CrearAsync(CitaDtoIn dto)
        {
            var response = await _httpClient.PostAsJsonAsync("api/Citas", dto);

            if (!response.IsSuccessStatusCode)
            {
                await ManejarErrorApi(response);
            }

            try
            {
                var result = await response.Content.ReadFromJsonAsync<Dictionary<string, object>>();
                if (result != null && result.ContainsKey("id"))
                {
                    return int.Parse(result["id"].ToString());
                }
            }
            catch { }

            return 0;
        }

        public async Task ActualizarAsync(int id, CitaDtoIn dto)
        {
            var response = await _httpClient.PutAsJsonAsync($"api/Citas/{id}", dto);

            if (!response.IsSuccessStatusCode)
            {
                await ManejarErrorApi(response);
            }
        }

        public async Task CambiarEstatusAsync(int id, EstatusCita nuevoEstatus)
        {
            // Patch requiere un cuerpo, enviamos el estatus
            var response = await _httpClient.PatchAsJsonAsync($"api/Citas/{id}/estatus", nuevoEstatus);

            if (!response.IsSuccessStatusCode)
            {
                await ManejarErrorApi(response);
            }
        }

        // --- Helpers Privados ---

        private async Task ManejarErrorApi(HttpResponseMessage response)
        {
            var mensaje = "Error en la operación";
            try
            {
                var errorDict = await response.Content.ReadFromJsonAsync<Dictionary<string, string>>();
                // Buscamos 'mensaje' o 'Mensaje'
                if (errorDict != null)
                {
                    if (errorDict.TryGetValue("mensaje", out var msg)) mensaje = msg;
                    else if (errorDict.TryGetValue("Mensaje", out var msgCap)) mensaje = msgCap;
                }
            }
            catch
            {
                mensaje = response.ReasonPhrase ?? "Error desconocido";
            }
            throw new HttpRequestException(mensaje);
        }

        public async Task EliminarAsync(int id)
        {
            // Llamamos al endpoint DELETE: api/Citas/{id}
            var response = await _httpClient.DeleteAsync($"api/Citas/{id}");

            if (!response.IsSuccessStatusCode)
            {
                await ManejarErrorApi(response);
            }
        }
    }
}
