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
    internal class RolRepository : IRolRepository
    {
        private readonly ApplicationDbContext _context;

        public RolRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        /// <summary>
        /// Agrega una nueva entidad Rol a la base de datos.
        /// </summary>
        public async Task AgregarAsync(Rol rol)
        {
            await _context.Roles.AddAsync(rol);
            await _context.SaveChangesAsync();
        }

        public async Task EliminarPorUsuarioIdAsync(int usuarioId)
        {
            // Busca todos los roles que pertenecen a este UsuarioId.
            var rolesAEliminar = _context.Roles.Where(r => r.Usuario.Id == usuarioId);

            // Usa ExecuteDeleteAsync para borrarlos todos en una sola consulta
            await rolesAEliminar.ExecuteDeleteAsync();

            // No llamamos a SaveChangesAsync porque ExecuteDeleteAsync lo hace solo.
        }
    }
}
