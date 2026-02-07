using DentalNova.Core.Dtos;
using DentalNova.Core.Interfaces;
using System.Net.Http.Headers;

namespace Proyecto_DentalNova.Services
{
    public class HorarioOdontologoServiceApi : IHorarioOdontologoService
    {
        private readonly HttpClient _httpClient;

        public HorarioOdontologoServiceApi(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }

        public async Task<List<HorarioOdontologoDto>> ObtenerPorOdontologoAsync(int odontologoId)
        {
            return await _httpClient.GetFromJsonAsync<List<HorarioOdontologoDto>>($"api/HorariosOdontologos/odontologo/{odontologoId}");
        }

        public async Task<HorarioOdontologoDto> ObtenerPorIdAsync(int id)
        {
            return await _httpClient.GetFromJsonAsync<HorarioOdontologoDto>($"api/HorariosOdontologos/{id}");
        }

        public async Task CrearAsync(HorarioOdontologoDtoIn dto)
        {
            var response = await _httpClient.PostAsJsonAsync("api/HorariosOdontologos", dto);

            if (!response.IsSuccessStatusCode)
            {
                await ManejarErrorApi(response);
            }
        }

        public async Task ActualizarAsync(int id, HorarioOdontologoDtoIn dto)
        {
            var response = await _httpClient.PutAsJsonAsync($"api/HorariosOdontologos/{id}", dto);

            if (!response.IsSuccessStatusCode)
            {
                await ManejarErrorApi(response);
            }
        }

        public async Task EliminarAsync(int id)
        {
            // DELETE api/HorariosOdontologos/{id}
            var response = await _httpClient.DeleteAsync($"api/HorariosOdontologos/{id}");

            if (!response.IsSuccessStatusCode)
            {
                await ManejarErrorApi(response);
            }
        }

        // --- Helpers Privados ---


        // Método auxiliar para no repetir la lógica de lectura de errores
        private async Task ManejarErrorApi(HttpResponseMessage response)
        {
            var mensaje = "Error en la operación";
            try
            {
                // Intentamos leer el JSON de error estandarizado: { "Mensaje": "..." }
                var errorDict = await response.Content.ReadFromJsonAsync<Dictionary<string, string>>();
                if (errorDict != null && errorDict.TryGetValue("mensaje", out var msg))
                {
                    mensaje = msg;
                }
                else if (errorDict != null && errorDict.TryGetValue("Mensaje", out var msgMayus)) // Por si acaso viene en mayúscula
                {
                    mensaje = msgMayus;
                }
            }
            catch
            {
                mensaje = response.ReasonPhrase ?? "Error desconocido";
            }

            throw new HttpRequestException(mensaje);
        }
    }
}
