using DentalNova.Core.Helpers;
using DentalNova.Core.Repository.Entities;

namespace Proyecto_DentalNova.Models.PacienteViewModel
{
    public class PacienteIndexViewModel
    {
        public PacienteFilterViewModel Filtro { get; set; } = new();
        public PaginatedList<Paciente>? Resultados { get; set; }
    }
}
