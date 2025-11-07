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
    public class HorarioOdontologoRepository : IHorarioOdontologoRepository
    {
        private readonly ApplicationDbContext _context;

        public HorarioOdontologoRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<List<HorarioOdontologo>> ObtenerHorariosDisponiblesAsync(DiaSemana dia, TimeSpan horaInicio, TimeSpan horaFin)
        {
            // Busca en la entidad HorarioOdontologo
            return await _context.HorariosOdontologos
                .Include(h => h.Odontologo)
                .Where(h => h.Activo &&
                            h.DiaSemana == dia &&
                            h.HoraInicio <= horaInicio &&
                            h.HoraFin >= horaFin)
                .ToListAsync();
        }
    }
}
