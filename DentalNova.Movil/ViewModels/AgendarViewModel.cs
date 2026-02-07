using DentalNova.Movil.Models;
using DentalNova.Movil.Services;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Text;

namespace DentalNova.Movil.ViewModels
{
    public class AgendarViewModel : BaseViewModel
    {
        private readonly UnitOfWork _unitOfWork;

        private bool tieneCitaActiva;
        public bool TieneCitaActiva
        {
            get => tieneCitaActiva;
            set { tieneCitaActiva = value; OnPropertyChanged(); OnPropertyChanged(nameof(MostrarFormulario)); }
        }
        public bool MostrarFormulario => !TieneCitaActiva;

        public CitaDto CitaActual { get; set; } // Para mostrar info si ya tiene una

        // FORMULARIO
        public ObservableCollection<OdontologoDto> Odontologos { get; } = new();
        public ObservableCollection<string> HorasDisponibles { get; } = new();

        private OdontologoDto selectedOdontologo;
        public OdontologoDto SelectedOdontologo
        {
            get => selectedOdontologo;
            set
            {
                selectedOdontologo = value;
                OnPropertyChanged();
                // Si cambia el doctor, recargamos horarios
                _ = CargarHorariosDelDoctor();
            }
        }

        private DateTime fechaSeleccionada = DateTime.Today.AddDays(1);
        public DateTime FechaSeleccionada
        {
            get => fechaSeleccionada;
            set
            {
                fechaSeleccionada = value;
                OnPropertyChanged();
                // Si cambia la fecha, recalculamos las horas disponibles
                CalcularHorasDisponibles();
            }
        }

        private string horaSeleccionada;
        public string HoraSeleccionada
        {
            get => horaSeleccionada;
            set { horaSeleccionada = value; OnPropertyChanged(); }
        }

        private string motivo;
        public string Motivo
        {
            get => motivo;
            set { motivo = value; OnPropertyChanged(); }
        }

        // Para guardar los rangos traídos de la API
        private List<HorarioOdontologoDto> _horariosRaw = new();

        public Command AgendarCommand { get; }
        public Command CancelarCitaActualCommand { get; }

        public AgendarViewModel(UnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
            Title = "Agendar Cita";
            AgendarCommand = new Command(OnAgendar);
            CancelarCitaActualCommand = new Command(OnCancelarActual);
        }

        public async Task OnAppearing()
        {
            IsBusy = true;
            // Verificar si ya tiene cita activa
            var citas = await _unitOfWork.CitaService.ObtenerMisCitasAsync();

            // Buscamos estatus 1 (Programada)
            var activa = citas.FirstOrDefault(c => c.EstatusTexto == "Programada" || c.EstatusTexto == "Confirmada");

            if (activa != null)
            {
                CitaActual = activa;
                TieneCitaActiva = true;
                OnPropertyChanged(nameof(CitaActual));
            }
            else
            {
                TieneCitaActiva = false;
                await CargarOdontologos();
            }
            IsBusy = false;
        }

        private async Task CargarOdontologos()
        {
            Odontologos.Clear();
            var docs = await _unitOfWork.OdontologoService.GetOdontologosAsync();
            foreach (var d in docs) Odontologos.Add(d);
        }

        private async Task CargarHorariosDelDoctor()
        {
            if (SelectedOdontologo == null) return;

            IsBusy = true;
            _horariosRaw = await _unitOfWork.OdontologoService.GetHorariosAsync(SelectedOdontologo.Id);
            CalcularHorasDisponibles();
            IsBusy = false;
        }

        private void CalcularHorasDisponibles()
        {
            HorasDisponibles.Clear();
            if (SelectedOdontologo == null || _horariosRaw.Count == 0) return;

            // Convertir DayOfWeek (Lunes=1... Domingo=7)
            int diaSemanaApi = FechaSeleccionada.DayOfWeek == DayOfWeek.Sunday ? 7 : (int)FechaSeleccionada.DayOfWeek;

            // Buscar si trabaja ese día
            var horarioDia = _horariosRaw.FirstOrDefault(h => h.DiaSemana == diaSemanaApi && h.Activo);

            if (horarioDia != null)
            {
                // Generar slots de 30 mins
                TimeSpan inicio = TimeSpan.Parse(horarioDia.HoraInicio);
                TimeSpan fin = TimeSpan.Parse(horarioDia.HoraFin);

                while (inicio < fin)
                {
                    HorasDisponibles.Add(inicio.ToString(@"hh\:mm"));
                    inicio = inicio.Add(TimeSpan.FromMinutes(30));
                }
            }
        }

        private async void OnAgendar()
        {
            if (string.IsNullOrEmpty(HoraSeleccionada) || string.IsNullOrEmpty(Motivo) || SelectedOdontologo == null)
            {
                await Application.Current.MainPage.DisplayAlert("Error", "Faltan datos", "OK");
                return;
            }

            IsBusy = true;

            // Cambinar Fecha y Hora
            var timeSpan = TimeSpan.Parse(HoraSeleccionada);
            DateTime fechaFinal = new DateTime(FechaSeleccionada.Year, FechaSeleccionada.Month, FechaSeleccionada.Day, timeSpan.Hours, timeSpan.Minutes, 0);

            var nuevaCita = new CitaCreateDto
            {
                PacienteId = _unitOfWork.Config.PacienteId,
                OdontologoId = SelectedOdontologo.Id,
                FechaHora = fechaFinal, // ISO 8601
                MotivoConsulta = Motivo,
                DuracionMinutos = 30,
                EstatusCita = 1,
                TratamientosIds = new List<int> { 1 }
            };

            bool exito = await _unitOfWork.CitaService.AgendarCitaAsync(nuevaCita);

            if (exito)
            {
                await Application.Current.MainPage.DisplayAlert("Éxito", "Cita agendada", "OK");
                // Recargar para mostrar la vista de "Cita Activa"
                await OnAppearing();
            }
            else
            {
                await Application.Current.MainPage.DisplayAlert("Error", "No se pudo agendar. Verifica disponibilidad.", "OK");
            }

            IsBusy = false;
        }

        private async void OnCancelarActual()
        {
            bool confirmar = await Application.Current.MainPage.DisplayAlert("Cancelar", "¿Deseas cancelar tu cita actual?", "Sí", "No");
            if (!confirmar) return;

            IsBusy = true;
            bool exito = await _unitOfWork.CitaService.CancelarCitaAsync(CitaActual.Id);

            if (exito)
            {
                await Application.Current.MainPage.DisplayAlert("Listo", "Cita cancelada", "OK");
                await OnAppearing(); // Recargar estado
            }
            else
            {
                await Application.Current.MainPage.DisplayAlert("Error", "No se pudo cancelar", "OK");
            }
            IsBusy = false;
        }
    }
}
