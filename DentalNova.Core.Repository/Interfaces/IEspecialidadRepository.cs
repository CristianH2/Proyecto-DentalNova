using DentalNova.Core.Repository.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DentalNova.Core.Repository.Interfaces
{
    public interface IEspecialidadRepository
    {
        Task<List<Especialidad>> ObtenerTodasAsync();

        // Obtiene una lista de entidades dado una lista de IDs (para asignar)
        Task<List<Especialidad>> ObtenerPorIdsAsync(List<int> ids);
    }
}
