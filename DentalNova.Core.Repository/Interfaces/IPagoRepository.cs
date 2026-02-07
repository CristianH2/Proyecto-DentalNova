using DentalNova.Core.Repository.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DentalNova.Core.Repository.Interfaces
{
    public interface IPagoRepository
    {
        Task<Pago> ObtenerPorIdAsync(int id);
        IQueryable<Pago> ObtenerQueryable();
        Task<List<Pago>> ObtenerPorCitaAsync(int citaId);
        Task<decimal> ObtenerTotalPagadoPorCitaAsync(int citaId);
        Task AgregarAsync(Pago pago);
    }
}
