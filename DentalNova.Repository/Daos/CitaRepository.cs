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
    public class CitaRepository : ICitaRepository
    {
        private readonly ApplicationDbContext _context;

        public CitaRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<bool> ActualizarAsync(Cita cita)
        {
            _context.Citas.Update(cita);
            return await _context.SaveChangesAsync() > 0; // Devuelve true si se guardaron cambios
        }

        public async Task<Cita> AgregarAsync(Cita cita)
        {
            await _context.Citas.AddAsync(cita);
            await _context.SaveChangesAsync();
            return cita;
        }

        public async Task<List<Cita>> ObtenerCitasEnRangoAsync(List<int> odontologoIds, DateTime inicio, DateTime fin)
        {
            // Busca citas que se superpongan con el rango de tiempo solicitado para los odontólogos especificados.
            return await _context.Citas
                .Where(c => odontologoIds.Contains(c.OdontologoId) && // Solo odontólogos relevantes
                            c.EstatusCita != Enumerables.EstatusCita.Cancelada && // Ignora citas canceladas
                            c.FechaHora < fin && // La cita empieza ANTES de que termine el nuevo espacio
                            (c.FechaHora.AddMinutes((double)c.DuracionMinutos)) > inicio) // La cita termina DESPUÉS de que empiece el nuevo espacio
                .ToListAsync();
        }

        public async Task<List<Cita>> ObtenerHistorialPorPacienteIdAsync(int pacienteId)
        {
            return await _context.Citas
                .Where(c => c.PacienteId == pacienteId) // Filtra por el PacienteId
                .Include(c => c.Odontologo.Usuario)     // Carga el Odontólogo y su Usuario (para el nombre)            
                .Include(c => c.CitasTratamientos)      // Carga la lista de CitaTratamiento
                .ThenInclude(ct => ct.Tratamiento)      // Carga el Tratamiento para cada CitaTratamiento
                .OrderByDescending(c => c.FechaHora)    // Ordena las citas por fecha de la más reciente
                .ToListAsync();
        }

        public async Task<Cita> ObtenerPorIdAsync(int citaId)
        {
            return await _context.Citas.FindAsync(citaId);
        }
    }
}
