using DentalNova.Movil.Models;
using DentalNova.Movil.Services;
using System;
using System.Collections.Generic;
using System.Text;

namespace DentalNova.Movil.ViewModels
{
    public class PerfilViewModel : BaseViewModel
    {
        private readonly UnitOfWork _unitOfWork;

        // DATOS
        private PerfilUsuarioDto usuario;
        public PerfilUsuarioDto Usuario
        {
            get => usuario;
            set { usuario = value; OnPropertyChanged(); }
        }

        private PerfilPacienteDto paciente;
        public PerfilPacienteDto Paciente
        {
            get => paciente;
            set { paciente = value; OnPropertyChanged(); }
        }

        // FORMULARIO PASSWORD
        private string passActual;
        public string PassActual
        {
            get => passActual;
            set { passActual = value; OnPropertyChanged(); }
        }

        private string passNuevo;
        public string PassNuevo
        {
            get => passNuevo;
            set { passNuevo = value; OnPropertyChanged(); }
        }

        private string passConfirmar;
        public string PassConfirmar
        {
            get => passConfirmar;
            set { passConfirmar = value; OnPropertyChanged(); }
        }

        public Command CambiarPassCommand { get; }
        public Command CerrarSesionCommand { get; }

        public PerfilViewModel(UnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
            Title = "Mi Perfil";
            CambiarPassCommand = new Command(OnCambiarPassword);
            CerrarSesionCommand = new Command(OnCerrarSesion);
        }

        public async Task OnAppearing()
        {
            if (IsBusy) return;
            IsBusy = true;

            var taskUsuario = _unitOfWork.PerfilService.ObtenerUsuarioAsync();
            var taskPaciente = _unitOfWork.PerfilService.ObtenerPacienteAsync();

            await Task.WhenAll(taskUsuario, taskPaciente);

            Usuario = await taskUsuario;
            Paciente = await taskPaciente;

            IsBusy = false;
        }

        private async void OnCambiarPassword()
        {
            if (string.IsNullOrWhiteSpace(PassActual) || string.IsNullOrWhiteSpace(PassNuevo))
            {
                await Application.Current.MainPage.DisplayAlert("Error", "Llena todos los campos", "OK");
                return;
            }

            if (PassNuevo != PassConfirmar)
            {
                await Application.Current.MainPage.DisplayAlert("Error", "Las contraseñas nuevas no coinciden", "OK");
                return;
            }

            IsBusy = true;
            bool exito = await _unitOfWork.PerfilService.CambiarPasswordAsync(PassActual, PassNuevo);
            IsBusy = false;

            if (exito)
            {
                await Application.Current.MainPage.DisplayAlert("Éxito", "Contraseña actualizada", "OK");
                PassActual = PassNuevo = PassConfirmar = string.Empty; // Limpiar campos
            }
            else
            {
                await Application.Current.MainPage.DisplayAlert("Error", "Contraseña actual incorrecta o error de red", "OK");
            }
        }

        private void OnCerrarSesion()
        {
            // Borrar datos de sesión
            _unitOfWork.Config.Token = string.Empty;
            _unitOfWork.Config.PacienteId = 0;
            _unitOfWork.Config.UsuarioId = 0;

            // Redirigir al Login
            Application.Current.MainPage = new NavigationPage(new Pages.InicioDeSesionPage(new InicioDeSesionViewModel(_unitOfWork)));
        }
    }
}
