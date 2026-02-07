using DentalNova.AppMovil.Helpers;
using System;
using System.Collections.Generic;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Text;

namespace DentalNova.AppMovil.Services
{
    public class ApiService
    {
        private readonly HttpClient _httpClient;

        public ApiService()
        {
            var handler = new HttpClientHandler();
            handler.ServerCertificateCustomValidationCallback = (message, cert, chain, errors) => true;

            _httpClient = new HttpClient
            {
                BaseAddress = new Uri(Constants.BaseUrl)
            };
        }

        // Método genérico para POST (Login, Registro, Agendar)
        public async Task<TResponse?> PostAsync<TRequest, TResponse>(string endpoint, TRequest data)
        {
            try
            {
                await AddAuthorizationHeader();
                var response = await _httpClient.PostAsJsonAsync(endpoint, data);

                if (response.IsSuccessStatusCode)
                {
                    return await response.Content.ReadFromJsonAsync<TResponse>();
                }
                else
                {
                    // Esto te ayudará a ver qué error devuelve la API
                    var errorContent = await response.Content.ReadAsStringAsync();
                    Console.WriteLine($"[API ERROR] {response.StatusCode}: {errorContent}");

                    // Opcional: Mostrar alerta rápida si falla (solo para debug)
                    await Application.Current.MainPage.DisplayAlert("Error API", errorContent, "OK");

                    return default;
                }

                var error = await response.Content.ReadAsStringAsync();
                Console.WriteLine($"Error API: {error}");
                return default;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Excepción: {ex.Message}");
                await Application.Current.MainPage.DisplayAlert("Error Técnico", ex.Message, "OK");
                return default;
            }
        }

        // Método genérico para GET (Listas, Historial, Perfil)
        public async Task<T?> GetAsync<T>(string endpoint)
        {
            await AddAuthorizationHeader();
            try
            {
                var response = await _httpClient.GetAsync(endpoint);
                if (response.IsSuccessStatusCode)
                {
                    return await response.Content.ReadFromJsonAsync<T>();
                }
                return default;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Excepción: {ex.Message}");
                return default;
            }
        }

        // Agrega el Token guardado a cada petición
        private async Task AddAuthorizationHeader()
        {
            var token = await SecureStorage.GetAsync(Constants.AuthTokenKey);
            if (!string.IsNullOrEmpty(token))
            {
                _httpClient.DefaultRequestHeaders.Authorization =
                    new AuthenticationHeaderValue("Bearer", token);
            }
        }

        // Método para cerrar sesión (Limpiar token)
        public void Logout()
        {
            SecureStorage.Remove(Constants.AuthTokenKey);
            SecureStorage.Remove(Constants.UserIdKey);
            SecureStorage.Remove(Constants.UserNameKey);
        }
    }
}
