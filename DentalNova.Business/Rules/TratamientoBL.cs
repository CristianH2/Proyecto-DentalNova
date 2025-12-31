using DentalNova.Business.Helpers;
using DentalNova.Core.Dtos;
using DentalNova.Core.Helpers;
using DentalNova.Core.Interfaces;
using DentalNova.Core.Repository.Entities;
using DentalNova.Core.Repository.Interfaces;
using DentalNova.Repository.Daos;

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

        public async Task<PaginatedList<TratamientoDto>> ObtenerListaPaginadaAsync(TratamientoFilterDto filtro)
        {
            var query = _repository.Tratamiento.ObtenerQueryableParaFiltro();

            if (filtro.Id.HasValue) query = query.Where(t => t.Id == filtro.Id.Value);
            if (!string.IsNullOrWhiteSpace(filtro.NombreLike)) query = query.Where(t => t.Nombre.Contains(filtro.NombreLike));
            if (filtro.CostoMin.HasValue) query = query.Where(t => t.Costo >= filtro.CostoMin.Value);
            if (filtro.CostoMax.HasValue) query = query.Where(t => t.Costo <= filtro.CostoMax.Value);
            if (filtro.DuracionMin.HasValue) query = query.Where(t => t.DuracionDias >= filtro.DuracionMin.Value);
            if (filtro.DuracionMax.HasValue) query = query.Where(t => t.DuracionDias <= filtro.DuracionMax.Value);
            if (filtro.Activo.HasValue) query = query.Where(t => t.Activo == filtro.Activo.Value);

            query = query.OrderBy(t => t.Nombre);

            var pagedEntities = await PaginatedList<Tratamiento>.CreateAsync(query, filtro.Page, filtro.PageSize);
            var dtos = pagedEntities.Select(t => t.ToDto()).ToList();

            return PaginatedList<TratamientoDto>.Create(dtos, pagedEntities.TotalCount, pagedEntities.PageIndex, filtro.PageSize);
        }

        public async Task<TratamientoDto> ObtenerPorIdAdminAsync(int id)
        {
            var tratamiento = await _repository.Tratamiento.ObtenerPorIdAsync(id);
            return tratamiento?.ToDto();
        }

        public async Task CrearTratamientoAdminAsync(TratamientoDtoIn dto)
        {
            if (await _repository.Tratamiento.ExisteNombreAsync(dto.Nombre))
                throw new InvalidOperationException("Ya existe un tratamiento con este nombre.");

            var nuevo = new Tratamiento();
            nuevo.MapFromDto(dto);
            await _repository.Tratamiento.AgregarAsync(nuevo);
        }

        public async Task ActualizarTratamientoAdminAsync(int id, TratamientoDtoIn dto)
        {
            if (await _repository.Tratamiento.ExisteNombreAsync(dto.Nombre, id))
                throw new InvalidOperationException("Ya existe otro tratamiento con este nombre.");

            var existente = await _repository.Tratamiento.ObtenerPorIdAsync(id);
            if (existente == null) return;

            existente.MapFromDto(dto);
            await _repository.Tratamiento.ActualizarAsync(existente);
        }

        public async Task EliminarTratamientoAsync(int id)
        {
            await _repository.Tratamiento.EliminarAsync(id);
        }
    }
}
