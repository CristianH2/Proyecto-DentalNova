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
    public class PacienteRepository : IPacienteRepository
    {
        private readonly ApplicationDbContext _context;

        public PacienteRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        /// <summary>
        /// Agrega un nuevo registro de Paciente a la base de datos.
        /// </summary>
        public async Task<Paciente> AgregarAsync(Paciente paciente)
        {
            await _context.Pacientes.AddAsync(paciente);
            await _context.SaveChangesAsync();
            return paciente; // Devuelve la entidad con el nuevo ID
        }

        /// <summary>
        /// Actualiza un registro de Paciente existente en la base de datos.
        /// </summary>
        public async Task<Paciente> ActualizarAsync(Paciente paciente)
        {
            // Le decimos a EF Core que esta entidad ha sido modificada
            _context.Entry(paciente).State = EntityState.Modified;
            await _context.SaveChangesAsync();
            return paciente;
        }

        /// <summary>
        /// Busca un perfil de Paciente usando el ID del Usuario asociado.
        /// </summary>
        public async Task<Paciente> ObtenerPorUsuarioIdAsync(int usuarioId)
        {
            // Busca en el DbSet "Pacientes"
            // usando la llave foránea "UsuarioId"
            return await _context.Pacientes
                                 .FirstOrDefaultAsync(p => p.UsuarioId == usuarioId);
        }
    }
}
