using DentalNova.Core.Dtos;
using DentalNova.Core.Interfaces;

namespace Proyecto_DentalNova.Services
{
    public class DashboardServiceApi : IDashboardService
    {
        private readonly HttpClient _httpClient;

        public DashboardServiceApi(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }

        public async Task<DashboardDto> ObtenerResumenAsync()
        {
            try
            {
                var resultado = await _httpClient.GetFromJsonAsync<DashboardDto>("api/Dashboard");
                return resultado ?? new DashboardDto();
            }
            catch
            {
                return new DashboardDto();
            }
        }
    }
}
