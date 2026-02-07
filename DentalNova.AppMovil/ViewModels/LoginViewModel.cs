using DentalNova.AppMovil.Helpers;
using DentalNova.AppMovil.Services;
using DentalNova.AppMovil.Views;
using DentalNova.Core.Dtos;
using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Input;

namespace DentalNova.AppMovil.ViewModels
{
    public class LoginViewModel : BaseViewModel
    {
        private readonly ApiService _apiService;
        private readonly IServiceProvider _serviceProvider;

        private string _email;
        private string _password;

        public LoginViewModel(ApiService apiService, IServiceProvider serviceProvider)
        {
            _apiService = apiService;
            _serviceProvider = serviceProvider;

            LoginCommand = new Command(OnLoginClicked, ValidateLogin);
            RegisterCommand = new Command(OnRegisterClicked);

        }

        // --- Propiedades Binding ---
        public string Email
        {
            get => _email;
            set
            {
                if (SetProperty(ref _email, value))
                    ((Command)LoginCommand).ChangeCanExecute();
            }
        }

        public string Password
        {
            get => _password;
            set
            {
                if (SetProperty(ref _password, value))
                    ((Command)LoginCommand).ChangeCanExecute();
            }
        }

        public ICommand LoginCommand { get; }
        public ICommand RegisterCommand { get; }

        private bool ValidateLogin()
        {
            return !string.IsNullOrWhiteSpace(Email) && !string.IsNullOrWhiteSpace(Password);
        }

        private async void OnRegisterClicked()
        {
            // Obtenemos la página desde el contenedor de dependencias
            var registerPage = _serviceProvider.GetRequiredService<RegisterPage>();
            await Application.Current.MainPage.Navigation.PushAsync(registerPage);
        }

        private async void OnLoginClicked()
        {
            if (IsBusy) return;
            IsBusy = true;

            try
            {
                // Preparar DTO de envío
                var loginDto = new LoginDto
                {
                    Correo = this.Email,
                    Password = this.Password
                };

                // Llamar a la API
                var response = await _apiService.PostAsync<LoginDto, LoginResponseDto>("Auth/login", loginDto);

                if (response != null && !string.IsNullOrEmpty(response.Token))
                {
                    // Datos de autenticación
                    await SecureStorage.SetAsync(Constants.AuthTokenKey, response.Token);
                    await SecureStorage.SetAsync(Constants.UserNameKey, response.NombreCompleto);
                    await SecureStorage.SetAsync(Constants.UserIdKey, response.UsuarioId.ToString());

                    // Guardar el ID de Paciente si existe.
                    if (response.PacienteId.HasValue)
                    {
                        await SecureStorage.SetAsync("PacienteId", response.PacienteId.Value.ToString());
                    }

                    // Evita que al dar "Atrás" se regrese al Login
                    Application.Current.MainPage = new AppShell();
                }
                else
                {
                    await Application.Current.MainPage.DisplayAlert("Error", "Usuario o contraseña incorrectos", "Aceptar");
                }
            }
            catch (Exception ex)
            {
                await Application.Current.MainPage.DisplayAlert("Error", $"Ocurrió un error de conexión: {ex.Message}", "Aceptar");
            }
            finally
            {
                IsBusy = false;
            }
        }
    }
}
