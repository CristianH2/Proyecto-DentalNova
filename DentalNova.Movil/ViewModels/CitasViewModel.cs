using DentalNova.Movil.Models;
using DentalNova.Movil.Services;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Text;

namespace DentalNova.Movil.ViewModels
{
    public class CitasViewModel : BaseViewModel
    {
        private readonly UnitOfWork _unitOfWork;

        public ObservableCollection<CitaDto> Citas { get; } = new(); // La lista que se verá en pantalla

        public Command CargarCitasCommand { get; }

        public CitasViewModel(UnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
            Title = "Mis Citas";

            CargarCitasCommand = new Command(async () => await CargarCitas());
        }

        private async Task CargarCitas()
        {
            // Si ya está cargando, no hacemos nada para evitar duplicados
            if (IsBusy) return;

            try
            {
                IsBusy = true; // Activa el spinner

                await Task.Delay(200);

                // Llamada a la API
                var lista = await _unitOfWork.CitaService.ObtenerMisCitasAsync();

                // Actualizar la lista en pantalla
                Citas.Clear();

                if (lista != null && lista.Count > 0)
                {
                    // la más reciente arriba
                    var listaOrdenada = lista.OrderByDescending(c => c.FechaHora);

                    foreach (var cita in listaOrdenada)
                    {
                        Citas.Add(cita);
                    }
                }
            }
            catch (Exception ex)
            {
                await Application.Current.MainPage.DisplayAlert("Error", "No se pudieron actualizar las citas", "OK");
            }
            finally
            {
                IsBusy = false;
            }
        }


        public async Task OnAppearing()
        {
                await CargarCitas();
        }
    }
}
