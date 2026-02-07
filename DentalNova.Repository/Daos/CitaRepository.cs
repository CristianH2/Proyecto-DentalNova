using DentalNova.Core.Repository.Entities;
using DentalNova.Core.Repository.Interfaces;
using DentalNova.Repository.DataContext;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static DentalNova.Core.Repository.Entities.Enumerables;

namespace DentalNova.Repository.Daos
{
    public class CitaRepository : ICitaRepository
    {
        private readonly ApplicationDbContext _context;

        public CitaRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<Cita> ObtenerPorIdAsync(int id)
        {
            return await _context.Citas
                .Include(c => c.Paciente).ThenInclude(p => p.Usuario)
                .Include(c => c.Odontologo).ThenInclude(o => o.Usuario)
                .Include(c => c.CitasTratamientos).ThenInclude(ct => ct.Tratamiento)
                .FirstOrDefaultAsync(c => c.Id == id);
        }

        public IQueryable<Cita> ObtenerQueryable()
        {
            // Retornamos el queryable base con includes necesarios para listados
            return _context.Citas
                .Include(c => c.Paciente).ThenInclude(p => p.Usuario)
                .Include(c => c.Odontologo).ThenInclude(o => o.Usuario)
                .AsNoTracking();
        }

        public async Task AgregarAsync(Cita cita)
        {
            await _context.Citas.AddAsync(cita);
            await _context.SaveChangesAsync();
        }

        public async Task ActualizarAsync(Cita cita)
        {
            _context.Citas.Update(cita);
            await _context.SaveChangesAsync();
        }

        public async Task EliminarAsync(int id)
        {
            var cita = await _context.Citas.FindAsync(id);
            if (cita != null)
            {
                // En lugar de borrar físico, cancelar citas
                cita.EstatusCita = EstatusCita.Cancelada;
                await _context.SaveChangesAsync();
            }
        }

        // --- VALIDACIONES ---

        public async Task<bool> ExisteConflictoHorarioAsync(int odontologoId, DateTime inicio, DateTime fin, int? citaIdExcluir = null)
        {
            // Conflicto: Una cita existente se solapa con el rango [inicio, fin]
            // Fórmula: (StartA < EndB) and (EndA > StartB)

            var query = _context.Citas
                .Where(c => c.OdontologoId == odontologoId &&
                            c.EstatusCita != EstatusCita.Cancelada && // Ignorar canceladas
                            c.EstatusCita != EstatusCita.NoAsistida);

            if (citaIdExcluir.HasValue)
            {
                query = query.Where(c => c.Id != citaIdExcluir.Value);
            }

            var citasDelDia = await query
                .Where(c => c.FechaHora.Date == inicio.Date)
                .ToListAsync();

            return citasDelDia.Any(c =>
            {
                var cInicio = c.FechaHora;
                var cFin = c.FechaHora.AddMinutes((int)c.DuracionMinutos);
                return inicio < cFin && fin > cInicio;
            });
        }

        public async Task<bool> EsHorarioLaboralValidoAsync(int odontologoId, DateTime inicio, DateTime fin)
        {
            var diaSemana = (DiaSemana)(int)inicio.DayOfWeek;
            // DayOfWeek.Sunday es 0 :: Enum Domingo es 7. Ajuste:
            if (diaSemana == 0) diaSemana = DiaSemana.Domingo;

            var horaInicio = inicio.TimeOfDay;
            var horaFin = fin.TimeOfDay;

            // Buscamos si existe al menos un bloque de horario que cubra totalmente el rango solicitado
            return await _context.HorariosOdontologos
                .AnyAsync(h => h.Odontologo.Id == odontologoId &&
                               h.Activo &&
                               h.DiaSemana == diaSemana &&
                               h.HoraInicio <= horaInicio && // Empieza antes o igual a la cita
                               h.HoraFin >= horaFin);        // Termina después o igual a la cita
        }
    }
}
