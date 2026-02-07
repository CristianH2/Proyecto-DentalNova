using DentalNova.Business.Helpers;
using DentalNova.Core.Dtos;
using DentalNova.Core.Interfaces;
using DentalNova.Core.Repository.Entities;
using DentalNova.Core.Repository.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace DentalNova.Business.Rules
{
    public class ArticuloBL : IArticuloBL
    {
        private readonly IArticuloRepository _articuloRepository;

        public ArticuloBL(IArticuloRepository articuloRepository)
        {
            _articuloRepository = articuloRepository;
        }

        public async Task<PagedResultDto<ArticuloDto>> ObtenerListaPaginadaAsync(ArticuloFilterDto filtro)
        {
            var query = _articuloRepository.ObtenerQueryable();

            if (!string.IsNullOrWhiteSpace(filtro.Busqueda))
            {
                var termino = filtro.Busqueda.Trim();
                query = query.Where(x => x.Nombre.Contains(termino) || x.Codigo.Contains(termino));
            }

            if (filtro.Categoria.HasValue)
                query = query.Where(x => x.Categoria == filtro.Categoria.Value);

            if (filtro.Activo.HasValue)
                query = query.Where(x => x.Activo == filtro.Activo.Value);

            var totalCount = await query.CountAsync();
            var pageSize = filtro.PageSize > 0 ? filtro.PageSize : 10;
            var totalPages = (int)Math.Ceiling((double)totalCount / pageSize);
            var pageIndex = filtro.Page < 1 ? 1 : filtro.Page;


            var items = await query
                .OrderBy(x => x.Nombre)
                .Skip((pageIndex - 1) * pageSize)
                .Take(pageSize)
                .Select(x => new ArticuloDto
                {
                    Id = x.Id,
                    Nombre = x.Nombre,
                    Codigo = x.Codigo,
                    Descripcion = x.Descripcion,
                    Stock = x.Stock,
                    Reutilizable = x.Reutilizable,
                    Activo = x.Activo,
                    Categoria = x.Categoria,
                    CategoriaTexto = x.Categoria.ToString()
                })
                .ToListAsync();

            return new PagedResultDto<ArticuloDto>
            {
                Items = items,
                TotalCount = totalCount,
                TotalPages = totalPages,
                PageIndex = pageIndex,
                HasPreviousPage = pageIndex > 1,
                HasNextPage = pageIndex < totalPages
            };
        }

        public async Task<ArticuloDtoIn> ObtenerParaEditarAsync(int id)
        {
            var entidad = await _articuloRepository.ObtenerPorIdAsync(id);
            if (entidad == null) throw new Exception("El artículo solicitado no existe.");

            return entidad.ToDtoIn();
        }

        public async Task<int> CrearAsync(ArticuloDtoIn dto)
        {
            if (await _articuloRepository.ExisteCodigoAsync(dto.Codigo))
            {
                throw new Exception($"El código '{dto.Codigo}' ya está asignado a otro producto.");
            }

            var entidad = new Articulo();

            entidad.MapFromDto(dto);
            entidad.Activo = true;
            await _articuloRepository.AgregarAsync(entidad);

            return entidad.Id;
        }

        public async Task ActualizarAsync(ArticuloDtoIn dto)
        {
            var entidad = await _articuloRepository.ObtenerPorIdAsync(dto.Id);
            if (entidad == null) throw new Exception("El artículo que intenta editar no existe.");

            if (await _articuloRepository.ExisteCodigoAsync(dto.Codigo, dto.Id))
            {
                throw new Exception($"El código '{dto.Codigo}' ya pertenece a otro producto.");
            }

            entidad.MapFromDto(dto);

            await _articuloRepository.ActualizarAsync(entidad);
        }

        public async Task EliminarAsync(int id)
        {
            await _articuloRepository.EliminarAsync(id);
        }

        public async Task CambiarEstatusAsync(int id)
        {
            var entidad = await _articuloRepository.ObtenerPorIdAsync(id);
            if (entidad == null) throw new Exception("Artículo no encontrado.");

            entidad.Activo = !entidad.Activo;
            await _articuloRepository.ActualizarAsync(entidad);
        }
    }
}