using DentalNova.Core.Repository.Entities;
using DentalNova.Core.Repository.Interfaces;
using DentalNova.Repository.DataContext;
using Microsoft.EntityFrameworkCore;
using static DentalNova.Core.Repository.Entities.Enumerables;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DentalNova.Repository.Daos
{
    public class HorarioOdontologoRepository : IHorarioOdontologoRepository
    {
        private readonly ApplicationDbContext _context;

        public HorarioOdontologoRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<HorarioOdontologo>> ObtenerPorOdontologoIdAsync(int odontologoId)
        {
            return await _context.HorariosOdontologos
                .Include(h => h.Odontologo)
                .ThenInclude(o => o.Usuario)
                .Where(h => h.Odontologo.Id == odontologoId)
                .OrderBy(h => h.DiaSemana)
                .ThenBy(h => h.HoraInicio)
                .ToListAsync();
        }

        public async Task<HorarioOdontologo> ObtenerPorIdAsync(int id)
        {
            return await _context.HorariosOdontologos
                .Include(h => h.Odontologo)
                .ThenInclude(o => o.Usuario)
                .FirstOrDefaultAsync(h => h.Id == id);
        }
        public async Task<List<HorarioOdontologo>> ObtenerHorariosDisponiblesAsync(DiaSemana dia, TimeSpan horaInicio, TimeSpan horaFin)
        {
            // Busca horarios activos del día X que cubran el rango solicitado
            return await _context.HorariosOdontologos
                .Include(h => h.Odontologo)
                .Where(h => h.Activo &&
                            h.DiaSemana == dia &&
                            h.HoraInicio <= horaInicio &&
                            h.HoraFin >= horaFin)
                .ToListAsync();
        }

        public IQueryable<HorarioOdontologo> ObtenerQueryableParaFiltro()
        {
            return _context.HorariosOdontologos.AsNoTracking();
        }

        public async Task AgregarAsync(HorarioOdontologo horario)
        {
            await _context.HorariosOdontologos.AddAsync(horario);
            await _context.SaveChangesAsync();
        }

        public async Task ActualizarAsync(HorarioOdontologo horario)
        {
            _context.HorariosOdontologos.Update(horario);
            await _context.SaveChangesAsync();
        }

        public async Task EliminarAsync(int id)
        {
            // Eliminación física (Hard Delete)
            var horario = await _context.HorariosOdontologos.FindAsync(id);
            if (horario != null)
            {
                _context.HorariosOdontologos.Remove(horario);
                await _context.SaveChangesAsync();
            }
        }

        public async Task<bool> ExisteSolapamientoAsync(int odontologoId, DiaSemana dia, TimeSpan horaInicio, TimeSpan horaFin, int? idExcluir = null)
        {
            var query = _context.HorariosOdontologos.AsNoTracking()
                .Where(h => h.Odontologo.Id == odontologoId &&
                            h.DiaSemana == dia &&
                            h.Activo); // Solo comparamos contra horarios activos

            if (idExcluir.HasValue)
            {
                query = query.Where(h => h.Id != idExcluir.Value);
            }

            // Lógica de solapamiento: (StartA < EndB) y (EndA > StartB)
            return await query.AnyAsync(h => horaInicio < h.HoraFin && horaFin > h.HoraInicio);
        }
    }
}
