using DentalNova.Movil.Models;
using System;
using System.Collections.Generic;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Text;

namespace DentalNova.Movil.Services
{
    public class NotificacionService
    {
        private readonly HttpClient _httpClient;
        private readonly ConfiguracionService _config;

        public NotificacionService(HttpClient httpClient, ConfiguracionService config)
        {
            _httpClient = httpClient;
            _config = config;
        }

        public async Task<List<NotificacionDto>> ObtenerMisNotificacionesAsync()
        {
            try
            {
                if (!string.IsNullOrEmpty(_config.Token))
                    _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", _config.Token);

                // Api
                int pacienteId = _config.PacienteId;
                var response = await _httpClient.GetAsync($"api/Recordatorios/mis-mensajes/{pacienteId}");

                if (response.IsSuccessStatusCode)
                {
                    return await response.Content.ReadFromJsonAsync<List<NotificacionDto>>() ?? new List<NotificacionDto>();
                }

                return new List<NotificacionDto>();
            }
            catch (Exception ex)
            {
                return new List<NotificacionDto>();
            }
        }
    }
}
