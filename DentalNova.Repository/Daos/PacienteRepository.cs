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


        // Agrega un nuevo registro de Paciente a la base de datos.
        public async Task<Paciente> AgregarAsync(Paciente paciente)
        {
            await _context.Pacientes.AddAsync(paciente);
            await _context.SaveChangesAsync();
            return paciente; // Devuelve la entidad con el nuevo ID
        }


        // Actualiza un registro de Paciente existente en la base de datos.
        public async Task<Paciente> ActualizarAsync(Paciente paciente)
        {
            // Le decimos a EF Core que esta entidad ha sido modificada
            _context.Entry(paciente).State = EntityState.Modified;
            await _context.SaveChangesAsync();
            return paciente;
        }


        // Busca un perfil de Paciente usando el ID del Usuario asociado.
        public async Task<Paciente> ObtenerPorUsuarioIdAsync(int usuarioId)
        {
            // Busca en el DbSet "Pacientes"
            // usando la llave foránea "UsuarioId"
            return await _context.Pacientes
                                 .FirstOrDefaultAsync(p => p.UsuarioId == usuarioId);
        }


        // --- MÉTODOS NUEVOS (Gestión Admin) ---

        // Retorna la consulta base para el filtrado.
        // Incluye la relación con Usuario para poder filtrar por Nombre/Apellido.
        public IQueryable<Paciente> ObtenerQueryableParaFiltro()
        {
            return _context.Pacientes
                           .Include(p => p.Usuario) // ¡Crucial! El filtro usa p.Usuario.Nombre
                           .AsNoTracking(); // Optimización de lectura
        }


        // Obtiene un paciente por su ID incluyendo los datos del usuario asociado.
        // (Para Details, Edit, Delete).
        public async Task<Paciente> ObtenerPorIdConUsuarioAsync(int id)
        {
            return await _context.Pacientes
                                 .Include(p => p.Usuario)
                                 .FirstOrDefaultAsync(m => m.Id == id);
        }


        // Elimina un paciente por su ID.
        public async Task EliminarAsync(int id)
        {
            var paciente = await _context.Pacientes.FindAsync(id);
            if (paciente != null)
            {
                _context.Pacientes.Remove(paciente);
                await _context.SaveChangesAsync();
            }
        }


        // Verifica si ya existe un registro de paciente para un usuario específico.
        // (Para evitar duplicados al crear).
        public async Task<bool> ExistePacienteParaUsuarioAsync(int usuarioId)
        {
            return await _context.Pacientes.AnyAsync(p => p.UsuarioId == usuarioId);
        }


        // Obtiene una lista de todos los IDs de usuarios que ya tienen un paciente asignado.
        // (Se usa para filtrar el DropDownList y no mostrar usuarios ocupados).
        public async Task<List<int>> ObtenerIdsUsuariosOcupadosAsync()
        {
            return await _context.Pacientes
                                 .Select(p => p.UsuarioId)
                                 .ToListAsync();
        }
    }
}
