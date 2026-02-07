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

        public async Task EliminarAsync(int id)
        {
            var entidad = await _context.CitasTratamientos.FindAsync(id);
            if (entidad != null)
            {
                _context.CitasTratamientos.Remove(entidad);
                await _context.SaveChangesAsync();
            }
        }

        public async Task<int> ContarTratamientosPreviosAsync(int pacienteId, int tratamientoId)
        {
            return await _context.CitasTratamientos
                .Include(ct => ct.Cita)
                .Where(ct => ct.Cita.PacienteId == pacienteId &&
                             ct.TratamientoId == tratamientoId &&
                             ct.Cita.EstatusCita == Enumerables.EstatusCita.Completada) // Solo contamos las completadas
                .CountAsync();
        }
    }
}
