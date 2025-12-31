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

        // --- MÉTODOS DE LECTURA ---

        public async Task<Odontologo> ObtenerPorIdAsync(int id)
        {
            // Carga al odontólogo y su entidad Usuario para obtener el nombre
            return await _context.Odontologos
                                 .Include(o => o.Usuario)
                                 .FirstOrDefaultAsync(o => o.Id == id);
        }

        public IQueryable<Odontologo> ObtenerQueryableParaFiltro()
        {
            // Incluimos Usuario (para filtrar por nombre) y Especialidades (para filtrar por esp.)
            return _context.Odontologos
                           .Include(o => o.Usuario)
                           .Include(o => o.Especialidades)
                           .AsNoTracking();
        }

        public async Task<Odontologo> ObtenerDetalleCompletoAsync(int id)
        {
            // Carga completa para Edición/Detalles
            return await _context.Odontologos
                                 .Include(o => o.Usuario)
                                 .Include(o => o.Especialidades)
                                 .FirstOrDefaultAsync(m => m.Id == id);
        }

        public async Task<List<int>> ObtenerIdsUsuariosOcupadosAsync()
        {
            // Obtenemos IDs de usuarios que YA son odontólogos
            return await _context.Odontologos
                                 .Select(o => o.UsuarioId)
                                 .ToListAsync();
        }

        public async Task<bool> ExisteOdontologoParaUsuarioAsync(int usuarioId)
        {
            return await _context.Odontologos.AnyAsync(o => o.UsuarioId == usuarioId);
        }

        // --- MÉTODOS DE ESCRITURA ---

        public async Task AgregarAsync(Odontologo odontologo)
        {
            await _context.Odontologos.AddAsync(odontologo);
            await _context.SaveChangesAsync();
        }

        public async Task ActualizarAsync(Odontologo odontologo)
        {
            _context.Odontologos.Update(odontologo);
            await _context.SaveChangesAsync();
        }

        public async Task EliminarAsync(int id)
        {
            var odontologo = await _context.Odontologos.FindAsync(id);
            if (odontologo != null)
            {
                _context.Odontologos.Remove(odontologo);
                await _context.SaveChangesAsync();
            }
        }
    }
}
