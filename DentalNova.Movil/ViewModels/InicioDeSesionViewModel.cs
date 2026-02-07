using DentalNova.Movil.Services;
using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Input;

namespace DentalNova.Movil.ViewModels
{
    public class InicioDeSesionViewModel : BaseViewModel
    {
        private readonly UnitOfWork _unitOfWork;

        // Propiedades para el Binding (con patrón clásico)
        private string correo;
        public string Correo
        {
            get => correo;
            set { correo = value; OnPropertyChanged(); }
        }

        private string password;
        public string Password
        {
            get => password;
            set { password = value; OnPropertyChanged(); }
        }

        private string mensajeError;
        public string MensajeError
        {
            get => mensajeError;
            set { mensajeError = value; OnPropertyChanged(); }
        }

        public ICommand LoginCommand { get; }
        public Command IrARegistroCommand { get; }

        // Constructor con Inyección de Dependencias
        public InicioDeSesionViewModel(UnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
            LoginCommand = new Command(OnLogin);
            IrARegistroCommand = new Command(async () => await IrARegistro());
        }

        private async void OnLogin()
        {
            if (IsBusy) return; // Evita doble clic

            // Validaciones
            if (string.IsNullOrWhiteSpace(Correo) || string.IsNullOrWhiteSpace(Password))
            {
                MensajeError = "Por favor ingrese correo y contraseña.";
                return;
            }

            IsBusy = true;
            MensajeError = string.Empty;

            // Llamada al servicio
            var resultado = await _unitOfWork.AuthService.IniciarSesionAsync(Correo, Password);

            if (resultado != null && !string.IsNullOrEmpty(resultado.Token))
            {
                _unitOfWork.Config.Token = resultado.Token;
                _unitOfWork.Config.PacienteId = resultado.PacienteId ?? 0;
                _unitOfWork.Config.UsuarioId = resultado.UsuarioId;
                Application.Current.MainPage = new AppShell();
            }
            else
            {
                MensajeError = "Credenciales incorrectas";
            }

            IsBusy = false;

        }
        private async Task IrARegistro()
        {
            await Application.Current.MainPage.Navigation.PushAsync(new Pages.RegistroPage(new ViewModels.RegistroViewModel(_unitOfWork.AuthService)));
        }
    }
}
