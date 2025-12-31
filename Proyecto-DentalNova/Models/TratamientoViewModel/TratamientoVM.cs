using DentalNova.Core.Dtos;

namespace Proyecto_DentalNova.Models.TratamientoViewModel
{
    public class TratamientoVM
    {
        // CAMBIO: Usamos el DTO de entrada
        public TratamientoDtoIn Tratamiento { get; set; } = new();
    }
}
