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

        /// <summary>
        /// Guarda los cambios en una entidad Usuario existente.
        /// </summary>
        public async Task<bool> ActualizarAsync(Usuario usuario)
        {
            _context.Usuarios.Update(usuario);
            // Devuelve 'true' si al menos una fila fue afectada
            return await _context.SaveChangesAsync() > 0;
        }

        /// <summary>
        /// Actualiza la contraseña utilizando un ID y la nueva contraseña
        /// </summary>
        public async Task<bool> ActualizarPasswordAsync(int id, string nuevoPasswordHash)
        {
            var usuario = await _context.Usuarios.FindAsync(id);
            if (usuario == null)
            {
                return false; // El usuario no existe
            }

            // Actualiza solo la propiedad de la contraseña
            usuario.Password = nuevoPasswordHash;
            _context.Usuarios.Update(usuario);

            // Guarda los cambios y devuelve 'true' si se afectó al menos 1 fila
            return await _context.SaveChangesAsync() > 0;
        }

        /// <summary>
        /// Agrega una nueva entidad Usuario a la base de datos.
        /// </summary>
        public async Task<Usuario> AgregarAsync(Usuario usuario)
        {
            await _context.Usuarios.AddAsync(usuario);
            await _context.SaveChangesAsync();
            return usuario; // Devuelve la entidad (con el 'Id' generado por la BD)
        }

        /// <summary>
        /// Busca un usuario por su email, sin incluir roles.
        /// </summary>
        public async Task<Usuario> ObtenerPorEmailAsync(string email)
        {
            return await _context.Usuarios
                         .FirstOrDefaultAsync(u => u.CorreoElectronico.ToLower() == email.ToLower());
        }

        /// <summary>
        /// Busca un usuario por su id.
        /// </summary>
        public async Task<Usuario> ObtenerPorIdAsync(int id)
        {
            // _context.Usuarios.FindAsync(id);
            return await _context.Usuarios
                        .Include(u => u.Roles)
                        .FirstOrDefaultAsync(u => u.Id == id);
        }
    }
}
