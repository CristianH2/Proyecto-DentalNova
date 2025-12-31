using DentalNova.Core.Dtos;
using DentalNova.Core.Helpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DentalNova.Core.Interfaces
{
    public interface IOdontologoBL
    {
        // CRUD Básico
        Task<PaginatedList<OdontologoDto>> ObtenerListaPaginadaAsync(OdontologoFilterDto filtro);
        Task<OdontologoDto> ObtenerPorIdAdminAsync(int id);
        Task CrearOdontologoAdminAsync(OdontologoDtoIn dto);
        Task ActualizarOdontologoAdminAsync(int id, OdontologoDtoIn dto);
        Task EliminarOdontologoAsync(int id);

        // Auxiliares para UI
        Task<List<UsuarioDisponibleDto>> ObtenerUsuariosDisponiblesAsync(int? odontologoIdEdicion = null);
        Task<List<EspecialidadDto>> ObtenerTodasEspecialidadesAsync();
    }
}
