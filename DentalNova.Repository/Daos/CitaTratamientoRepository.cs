using DentalNova.Core.Repository.Entities;
using DentalNova.Core.Repository.Interfaces;
using DentalNova.Repository.DataContext;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DentalNova.Repository.Daos
{
    public class CitaTratamientoRepository : ICitaTratamientoRepository
    {
        private readonly ApplicationDbContext _context;

        public CitaTratamientoRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task AgregarAsync(CitaTratamiento citaTratamiento)
        {
            await _context.CitasTratamientos.AddAsync(citaTratamiento);
            await _context.SaveChangesAsync();
        }
    }
}
