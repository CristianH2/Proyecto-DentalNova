using DentalNova.Core.Dtos;
using DentalNova.Core.Interfaces;
using System.Net.Http.Headers;

namespace Proyecto_DentalNova.Services
{
    public class RecordatorioServiceApi : IRecordatorioService
    {
        private readonly HttpClient _httpClient;

        public RecordatorioServiceApi(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }

        public async Task EnviarRecordatorioAsync(int citaId)
        {
            var response = await _httpClient.PostAsync($"api/Recordatorios/enviar/{citaId}", null);

            if (!response.IsSuccessStatusCode)
            {
                var error = await response.Content.ReadAsStringAsync();
                // Limpiamos el mensaje si viene en formato JSON complejo
                throw new HttpRequestException(error.Replace("{", "").Replace("}", "").Replace("message", "").Replace(":", "").Trim('"'));
            }
        }

        public async Task<List<RecordatorioDto>> ObtenerMisMensajesAsync(int pacienteId)
        {
            var response = await _httpClient.GetFromJsonAsync<List<RecordatorioDto>>($"api/Recordatorios/mis-mensajes/{pacienteId}");

            return response ?? new List<RecordatorioDto>();
        }
    }
}
