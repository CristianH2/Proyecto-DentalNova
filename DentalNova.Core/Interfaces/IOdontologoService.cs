using DentalNova.Core.Dtos;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DentalNova.Core.Interfaces
{
    public interface IOdontologoService
    {
        // CRUD Principal
        Task<PagedResultDto<OdontologoDto>> ObtenerOdontologosAsync(OdontologoFilterDto filtro);
        Task<OdontologoDto> ObtenerOdontologoPorIdAsync(int id);
        Task CrearOdontologoAsync(OdontologoDtoIn dto);
        Task ActualizarOdontologoAsync(int id, OdontologoDtoIn dto);
        Task EliminarOdontologoAsync(int id);

        // Auxiliares para DropDowns
        Task<List<UsuarioDisponibleDto>> ObtenerUsuariosDisponiblesAsync(int? odontologoIdEdicion = null);
        Task<List<EspecialidadDto>> ObtenerEspecialidadesAsync();
    }
}
