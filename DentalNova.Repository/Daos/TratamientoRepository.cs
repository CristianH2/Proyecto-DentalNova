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
    public class TratamientoRepository : ITratamientoRepository
    {
        private readonly ApplicationDbContext _context;

        public TratamientoRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<Tratamiento>> ObtenerTodosActivosAsync()
        {
            return await _context.Tratamientos
                                 .Where(t => t.Activo)
                                 .ToListAsync();
        }

        public async Task<Tratamiento> ObtenerPorIdAsync(int id)
        {
            // Busca la entidad Tratamiento por su ID
            return await _context.Tratamientos.FindAsync(id);
        }

        public IQueryable<Tratamiento> ObtenerQueryableParaFiltro()
        {
            return _context.Tratamientos.AsNoTracking();
        }

        public async Task AgregarAsync(Tratamiento tratamiento)
        {
            await _context.Tratamientos.AddAsync(tratamiento);
            await _context.SaveChangesAsync();
        }

        public async Task ActualizarAsync(Tratamiento tratamiento)
        {
            _context.Tratamientos.Update(tratamiento);
            await _context.SaveChangesAsync();
        }

        public async Task EliminarAsync(int id)
        {
            var tratamiento = await _context.Tratamientos.FindAsync(id);
            if (tratamiento != null)
            {
                _context.Tratamientos.Remove(tratamiento);
                await _context.SaveChangesAsync();
            }
        }

        public async Task<bool> ExisteNombreAsync(string nombre, int? idExcluir = null)
        {
            var query = _context.Tratamientos.AsNoTracking();
            if (idExcluir.HasValue)
            {
                query = query.Where(t => t.Id != idExcluir.Value);
            }
            return await query.AnyAsync(t => t.Nombre == nombre);
        }
    }
}
