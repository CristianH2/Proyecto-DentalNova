using DentalNova.Core.Dtos;
using DentalNova.Core.Helpers;
using DentalNova.Core.Repository.Entities;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace Proyecto_DentalNova.Models.CitaViewModel
{
    public class CitaIndexViewModel
    {
        // Filtros aplicados
        public CitaFilterDto Filtro { get; set; } = new CitaFilterDto();

        // Resultados paginados de la API
        //public PagedResultDto<CitaDto> Resultados { get; set; } = new PagedResultDto<CitaDto>();
        public PaginatedList<CitaDto> Resultados { get; set; }

        // Listas para los filtros (Dropdowns)
        public IEnumerable<SelectListItem> Odontologos { get; set; } = new List<SelectListItem>();
        public IEnumerable<SelectListItem> Pacientes { get; set; } = new List<SelectListItem>();
        public IEnumerable<SelectListItem> Estatus { get; set; } = new List<SelectListItem>();
    }
    //public class CitaIndexViewModel
    //{
    //    public CitaFilterViewModel Filtro { get; set; } = new();
    //    public PaginatedList<Cita>? Resultados { get; set; }
    //}
}
