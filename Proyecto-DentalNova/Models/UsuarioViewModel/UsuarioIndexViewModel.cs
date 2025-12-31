using DentalNova.Core.Dtos;
using DentalNova.Core.Helpers;

namespace Proyecto_DentalNova.Models.UsuarioViewModel
{
    public class UsuarioIndexViewModel
    {
        public UsuarioFilterViewModel Filtro { get; set; } = new();
        public PaginatedList<UsuarioAdminDto>? Resultados { get; set; }
    }
}
