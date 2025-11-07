using DentalNova.Core.Dtos;
using DentalNova.Core.Interfaces;
using DentalNova.Core.Repository.Interfaces;

namespace DentalNova.Business.Rules
{
    public class TratamientoBL : ITratamientoBL
    {
        private readonly IRepository _repository;

        public TratamientoBL(IRepository repository)
        {
            _repository = repository;
        }

        public async Task<IEnumerable<TratamientoDto>> ObtenerCatalogoAsync()
        {
            var tratamientos = await _repository.Tratamiento.ObtenerTodosActivosAsync();

            return tratamientos.Select(t => new TratamientoDto
            {
                Id = t.Id,
                Nombre = t.Nombre,
                Descripcion = t.Descripcion,
                Costo = t.Costo
            });
        }
    }
}
