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
    public class RecordatorioRepository : IRecordatorioRepository
    {
        private readonly ApplicationDbContext _context;

        public RecordatorioRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task AgregarAsync(Recordatorio recordatorio)
        {
            await _context.Recordatorios.AddAsync(recordatorio);
            await _context.SaveChangesAsync();
        }

        public async Task<List<Recordatorio>> ObtenerPorPacienteAsync(int pacienteId)
        {
            return await _context.Recordatorios
                .Include(r => r.Cita)
                    .ThenInclude(c => c.Odontologo)
                        .ThenInclude(o => o.Usuario) // Mostrar nombre del doctor
                .Where(r => r.Cita.PacienteId == pacienteId)
                .OrderByDescending(r => r.FechaEnvio) // Mensajes recientes primero
                .ToListAsync();
        }

        public async Task<bool> ExisteRecordatorioParaCitaAsync(int citaId)
        {
            return await _context.Recordatorios
                .AnyAsync(r => r.CitaId == citaId);
        }
    }
}
