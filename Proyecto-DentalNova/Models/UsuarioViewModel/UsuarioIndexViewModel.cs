using DentalNova.Core.Helpers;
using DentalNova.Core.Repository.Entities;

namespace Proyecto_DentalNova.Models.UsuarioViewModel
{
    public class UsuarioIndexViewModel
    {
        public UsuarioFilterViewModel Filtro { get; set; } = new();
        public PaginatedList<Usuario>? Resultados { get; set; }
    }
}
