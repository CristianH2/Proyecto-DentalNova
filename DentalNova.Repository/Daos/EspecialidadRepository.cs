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
    public class EspecialidadRepository : IEspecialidadRepository
    {
        private readonly ApplicationDbContext _context;

        public EspecialidadRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<List<Especialidad>> ObtenerTodasAsync()
        {
            return await _context.Especialidades
                                 .OrderBy(e => e.Nombre)
                                 .ToListAsync();
        }

        public async Task<List<Especialidad>> ObtenerPorIdsAsync(List<int> ids)
        {
            // Busca todas las especialidades cuyo ID esté en la lista proporcionada
            return await _context.Especialidades
                                 .Where(e => ids.Contains(e.Id))
                                 .ToListAsync();
        }
    }
}
