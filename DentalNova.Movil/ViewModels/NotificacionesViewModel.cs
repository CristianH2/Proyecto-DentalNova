using DentalNova.Movil.Models;
using DentalNova.Movil.Services;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Text;

namespace DentalNova.Movil.ViewModels
{
    public class NotificacionesViewModel : BaseViewModel
    {
        private readonly UnitOfWork _unitOfWork;
        public ObservableCollection<NotificacionDto> Notificaciones { get; } = new();

        public NotificacionesViewModel(UnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
            Title = "Avisos";
        }

        public async Task OnAppearing()
        {
            if (IsBusy) return;

            try
            {
                IsBusy = true;
                var lista = await _unitOfWork.NotificacionService.ObtenerMisNotificacionesAsync();

                Notificaciones.Clear();
                foreach (var n in lista.OrderByDescending(x => x.FechaEnvio))
                {
                    Notificaciones.Add(n);
                }
            }
            finally
            {
                IsBusy = false;
            }
        }
    }
}
