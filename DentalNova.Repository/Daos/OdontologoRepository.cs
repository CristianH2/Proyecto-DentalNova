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
    public class OdontologoRepository : IOdontologoRepository
    {
        private readonly ApplicationDbContext _context;

        public OdontologoRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<Odontologo> ObtenerPorIdAsync(int id)
        {
            // Carga al odontólogo y su entidad Usuario para obtener el nombre
            return await _context.Odontologos
                                 .Include(o => o.Usuario)
                                 .FirstOrDefaultAsync(o => o.Id == id);
        }
    }
}
