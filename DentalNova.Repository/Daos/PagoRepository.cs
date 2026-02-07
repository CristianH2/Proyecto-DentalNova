using DentalNova.Core.Repository.Entities;
using DentalNova.Core.Repository.Interfaces;
using DentalNova.Repository.DataContext;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DentalNova.Repository.Daos
{
    public class PagoRepository : IPagoRepository
    {
        private readonly ApplicationDbContext _context;

        public PagoRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<Pago> ObtenerPorIdAsync(int id)
        {
            return await _context.Pagos
                .Include(p => p.Paciente).ThenInclude(pa => pa.Usuario)
                .Include(p => p.Cita)
                .FirstOrDefaultAsync(p => p.Id == id);
        }

        public IQueryable<Pago> ObtenerQueryable()
        {
            return _context.Pagos
                .Include(p => p.Paciente).ThenInclude(pa => pa.Usuario)
                .Include(p => p.Cita).ThenInclude(c => c.Odontologo).ThenInclude(o => o.Usuario)
                .AsNoTracking(); // Optimización de solo lectura
        }

        public async Task<List<Pago>> ObtenerPorCitaAsync(int citaId)
        {
            return await _context.Pagos
                .Where(p => p.CitaId == citaId)
                .OrderByDescending(p => p.FechaPago)
                .ToListAsync();
        }

        public async Task<decimal> ObtenerTotalPagadoPorCitaAsync(int citaId)
        {
            var total = await _context.Pagos
                .Where(p => p.CitaId == citaId)
                .SumAsync(p => p.Monto);

            return total;
        }

        public async Task AgregarAsync(Pago pago)
        {
            await _context.Pagos.AddAsync(pago);
            await _context.SaveChangesAsync();
        }
    }
}
