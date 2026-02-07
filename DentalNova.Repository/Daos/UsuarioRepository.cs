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
    public class UsuarioRepository : IUsuarioRepository
    {
        private readonly ApplicationDbContext _context;

        public UsuarioRepository(ApplicationDbContext context)
        {
            _context = context;
        }
        public async Task<bool> ActualizarAsync(Usuario usuario)
        {
            _context.Usuarios.Update(usuario);
            // Devuelve 'true' si al menos una fila fue afectada
            return await _context.SaveChangesAsync() > 0;
        }

        public async Task ActualizarUsuarioAdminAsync(Usuario usuario, bool actualizarPassword)
        {
            if (actualizarPassword)
            {
                // Actualiza la entidad completa, incluyendo la contraseña hasheada
                _context.Usuarios.Update(usuario);
            }
            else
            {
                // Actualiza la entidad pero le dice a EF que ignore el campo 'Password'
                _context.Entry(usuario).State = EntityState.Modified;
                _context.Entry(usuario).Property(u => u.Password).IsModified = false;
            }

            await _context.SaveChangesAsync();
        }
        public async Task<bool> ActualizarPasswordAsync(int id, string nuevoPasswordHash)
        {
            var usuario = await _context.Usuarios.FindAsync(id);
            if (usuario == null)
            {
                return false; 
            }

            usuario.Password = nuevoPasswordHash;
            _context.Usuarios.Update(usuario);

            // Guarda los cambios y devuelve 'true' si se afectó al menos 1 fila
            return await _context.SaveChangesAsync() > 0;
        }

        public async Task<Usuario> AgregarAsync(Usuario usuario)
        {
            await _context.Usuarios.AddAsync(usuario);
            await _context.SaveChangesAsync();
            return usuario; // Devuelve la entidad (con el 'Id' generado por la BD)
        }

        public async Task<Usuario> ObtenerPorEmailAsync(string email)
        {
            return await _context.Usuarios
                         .Include(u => u.Roles)
                         .FirstOrDefaultAsync(u => u.CorreoElectronico.ToLower() == email.ToLower());
        }

        public async Task<Usuario> ObtenerPorIdAsync(int id)
        {
            // _context.Usuarios.FindAsync(id);
            return await _context.Usuarios
                        .Include(u => u.Roles)
                        .FirstOrDefaultAsync(u => u.Id == id);
        }

        // --- Métodos para Admin MVC ---
        public IQueryable<Usuario> ObtenerQueryableParaFiltro()
        {
            return _context.Usuarios.AsNoTracking();
        }


        public async Task EliminarAsync(int id)
        {
            var usuario = await _context.Usuarios.FindAsync(id);
            if (usuario != null)
            {
                _context.Usuarios.Remove(usuario);
                await _context.SaveChangesAsync();
            }
        }

        public async Task<bool> EmailYaExisteAsync(string email, int? usuarioId = null)
        {
            var query = _context.Usuarios.AsNoTracking();

            if (usuarioId.HasValue)
            {
                // Para el caso de "Edit", excluye al usuario actual de la búsqueda
                query = query.Where(u => u.Id != usuarioId.Value);
            }

            return await query.AnyAsync(u => u.CorreoElectronico == email);
        }

        public async Task<bool> CurpYaExisteAsync(string curp, int? usuarioId = null)
        {
            var query = _context.Usuarios.AsNoTracking();

            if (usuarioId.HasValue)
            {
                // Para el caso de "Edit", excluye al usuario actual de la búsqueda
                query = query.Where(u => u.Id != usuarioId.Value);
            }

            return await query.AnyAsync(u => u.CURP == curp);
        }
    }
}
