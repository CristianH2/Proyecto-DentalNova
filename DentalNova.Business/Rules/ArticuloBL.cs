using DentalNova.Core.Dtos;
using DentalNova.Core.Interfaces;
using DentalNova.Core.Repository.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DentalNova.Business.Rules
{
    public class ArticuloBL : IArticuloBL
    {
        private readonly IRepository _repositorio;

        public ArticuloBL(IRepository repositorio)
        {
            _repositorio = repositorio;
        }

        public async Task<IEnumerable<ArticuloDto>> ObtenerCatalogoAsync()
        {
            // 1. Llama a la capa de repositorio
            var articulos = await _repositorio.Articulo.ObtenerTodosActivosAsync();

            // 2. Mapea las Entidades a DTOs
            //    (Usando las propiedades de ArticulosConfig)
            return articulos.Select(a => new ArticuloDto
            {
                Id = a.Id,
                Nombre = a.Nombre,
                Descripcion = a.Descripcion,
                Codigo = a.Codigo,
                Stock = a.Stock
            });
        }
    }
}
