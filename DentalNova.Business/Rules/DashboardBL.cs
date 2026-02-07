using DentalNova.Core.Dtos;
using DentalNova.Core.Interfaces;
using DentalNova.Core.Repository.Entities;
using DentalNova.Core.Repository.Interfaces;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DentalNova.Business.Rules
{
    public class DashboardBL : IDashboardBL
    {
        private readonly ICitaRepository _citaRepository;
        private readonly IPagoRepository _pagoRepository;
        private readonly IPacienteRepository _pacienteRepository;
        private readonly IRepository _repository;

        public DashboardBL(
            ICitaRepository citaRepository,
            IPagoRepository pagoRepository,
            IPacienteRepository pacienteRepository,
            IRepository repository)
        {
            _citaRepository = citaRepository;
            _pagoRepository = pagoRepository;
            _pacienteRepository = pacienteRepository;
            _repository = repository;
        }

        public async Task<DashboardDto> ObtenerResumenAsync(int? usuarioId = null)
        {
            int? odontologoId = null;
            var dto = new DashboardDto();
            var cultura = new CultureInfo("es-ES");

            if (usuarioId is not null)
            {
                odontologoId = await _repository.Odontologo
                    .ObtenerQueryableParaFiltro()
                    .Where(o => o.UsuarioId == usuarioId)
                    .Select(o => o.Id)
                    .FirstOrDefaultAsync();
            }

            // --- FECHAS DE CORTE ---
            var hoy = DateTime.Today; // 00:00:00
            var manana = hoy.AddDays(1);
            var inicioMes = new DateTime(hoy.Year, hoy.Month, 1);
            var hace7Dias = hoy.AddDays(-6); // Rango para gráfico


            // CALCULAR KPIs (Consultas Count/Sum directas a BD)

            // Citas de HOY
            var queryCitas = _citaRepository.ObtenerQueryable();

            // Si es odontólogo, filtramos TODO por su ID
            if (odontologoId.HasValue)
            {
                queryCitas = queryCitas.Where(x => x.OdontologoId == odontologoId.Value);
            }

            // -- KPIS ---

            dto.CitasHoy = await queryCitas
                .CountAsync(x => x.FechaHora >= hoy && x.FechaHora < manana);

            dto.CitasPendientesHoy = await queryCitas
                .CountAsync(x => x.FechaHora >= hoy && x.FechaHora < manana
                            && (x.EstatusCita == Enumerables.EstatusCita.Programada
                                || x.EstatusCita == Enumerables.EstatusCita.Completada));

            // Pacientes Nuevos este MES
            //var queryPacientes = _pacienteRepository.ObtenerQueryableParaFiltro();
            //dto.PacientesNuevosMes = await queryPacientes
            //    .CountAsync(x => x.FechaCreacion >= inicioMes);
            if (odontologoId.HasValue)
            {
                // Para el Dr. mostramos sus citas completadas en el mes en lugar de "Pacientes Nuevos"
                dto.PacientesNuevosMes = await queryCitas
                    .CountAsync(x => x.FechaHora >= inicioMes && x.EstatusCita == Enumerables.EstatusCita.Completada);
            }
            else
            {
                dto.PacientesNuevosMes = await _pacienteRepository.ObtenerQueryableParaFiltro()
                    .CountAsync(x => x.FechaCreacion >= inicioMes);
            }

            // Ingresos este MES
            var queryPagos = _pagoRepository.ObtenerQueryable();
            dto.IngresosMes = await queryPagos
                .Where(x => x.FechaPago >= inicioMes)
                .SumAsync(x => x.Monto);


            // OBTENER DATOS PARA GRÁFICO (Últimos 7 días)
            var citasSemana = await queryCitas
                .Where(x => x.FechaHora >= hace7Dias && x.FechaHora < manana)
                .Select(x => x.FechaHora.Date) // Solo traemos la fecha
                .ToListAsync();

            var pagosSemana = await queryPagos
                .Where(x => x.FechaPago >= hace7Dias && x.FechaPago < manana)
                .Select(x => new { x.FechaPago.Date, x.Monto }) // Solo fecha y monto
                .ToListAsync();

            // Rellenar el gráfico asegurando que existan los 7 días aunque no haya datos
            for (int i = 0; i < 7; i++)
            {
                var fechaDia = hace7Dias.AddDays(i);
                dto.DatosGrafico.Add(new DashboardGraficoDto
                {
                    DiaSemana = $"{fechaDia.ToString("ddd", cultura)} {fechaDia.Day}",
                    CantidadCitas = citasSemana.Count(c => c == fechaDia),
                    TotalIngresos = pagosSemana.Where(p => p.Date == fechaDia).Sum(p => p.Monto)
                });
            }

            // PRÓXIMAS CITAS 


            
            dto.ProximasCitas = await queryCitas
            .Where(x => x.FechaHora >= DateTime.Now) // Desde este instante en adelante
            .OrderBy(x => x.FechaHora)
            .Take(5)
            .Select(x => new DashboardCitaDto
            {
                Id = x.Id,
                Hora = x.FechaHora.ToString("HH:mm"),
                Paciente = x.Paciente.Usuario.Nombre + " " + x.Paciente.Usuario.Apellidos,
                Odontologo = x.Odontologo.Usuario.Nombre + " " + x.Odontologo.Usuario.Apellidos,
                Estatus = x.EstatusCita.ToString(),
                // Tomamos el nombre del primer tratamiento o un default
                Tratamiento = x.CitasTratamientos.Any()
                                ? x.CitasTratamientos.First().Tratamiento.Nombre
                                : "Consulta General"
            })
            .ToListAsync();

            return dto;
        }
    }
}
