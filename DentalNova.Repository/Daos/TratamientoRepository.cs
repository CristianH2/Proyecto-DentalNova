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
    }
}
