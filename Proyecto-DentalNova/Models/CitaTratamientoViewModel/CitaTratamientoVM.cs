using DentalNova.Core.Repository.Entities;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace Proyecto_DentalNova.Models.CitaTratamientoViewModel
{
    public record TratamientoDisponible(int Id, string Nombre, decimal Costo);

    public class CitaTratamientoVM
    {
        public CitaTratamiento CitaTratamiento { get; set; } = new();

        // Se llenará solo con tratamientos activos.
        public IEnumerable<TratamientoDisponible> TratamientosDisponibles { get; set; } = new List<TratamientoDisponible>();

        // Lista para poblar el DropDownList de Estatus.
        public IEnumerable<SelectListItem> EstatusDisponibles { get; set; } = new List<SelectListItem>();
    }
}
