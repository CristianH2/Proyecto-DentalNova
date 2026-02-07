using DentalNova.Business.Helpers;
using DentalNova.Core.Dtos;
using DentalNova.Core.Interfaces;
using DentalNova.Core.Repository.Entities;
using DentalNova.Core.Repository.Interfaces;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DentalNova.Business.Rules
{
    public class PagoBL : IPagoBL
    {
        private readonly IRepository _repositorio;

        public PagoBL(IRepository repository)
        {
            _repositorio = repository;
        }

        public async Task<PagedResultDto<PagoDto>> ObtenerListaPaginadaAsync(PagoFilterDto filtro)
        {
            var query = _repositorio.Pago.ObtenerQueryable();

            if (filtro.PacienteId.HasValue) query = query.Where(p => p.PacienteId == filtro.PacienteId.Value);

            if (filtro.FechaInicio.HasValue) query = query.Where(p => p.FechaPago >= filtro.FechaInicio.Value); 

            if (filtro.FechaFin.HasValue)
            {
                var fechaFinAjustada = filtro.FechaFin.Value.Date.AddDays(1).AddTicks(-1);
                query = query.Where(p => p.FechaPago <= fechaFinAjustada);
            }

            var totalCount = await query.CountAsync();
            var pageSize = filtro.PageSize > 0 ? filtro.PageSize : 10;
            var totalPages = (int)Math.Ceiling((double)totalCount / pageSize);
            var pageIndex = filtro.Page < 1 ? 1 : filtro.Page;

            var items = await query
                .OrderByDescending(p => p.FechaPago)
                .Skip((pageIndex - 1) * pageSize)
                .Take(pageSize)
                .Select(p => new PagoDto
                {
                    Id = p.Id,
                    Monto = p.Monto,
                    FechaPago = p.FechaPago,
                    MetodoPago = p.MetodoPago,
                    CitaId = p.CitaId,
                    PacienteNombre = p.Paciente.Usuario.Nombre + " " + p.Paciente.Usuario.Apellidos,
                    OdontologoNombre = p.Cita.Odontologo.Usuario.Nombre + " " + p.Cita.Odontologo.Usuario.Apellidos
                })
                .ToListAsync();

            return new PagedResultDto<PagoDto>
            {
                Items = items,
                TotalCount = totalCount,
                TotalPages = totalPages,
                PageIndex = pageIndex,
                HasPreviousPage = pageIndex > 1,
                HasNextPage = pageIndex < totalPages
            };
        }

        public async Task<EstadoCuentaCitaDto> ObtenerEstadoCuentaCitaAsync(int citaId)
        {
            var cita = await _repositorio.Cita.ObtenerPorIdAsync(citaId);
            if (cita == null)
                throw new Exception("La cita solicitada no existe.");

            var totalPagado = await _repositorio.Pago.ObtenerTotalPagadoPorCitaAsync(citaId);

            return new EstadoCuentaCitaDto
            {
                CitaId = cita.Id,
                PacienteNombre = $"{cita.Paciente.Usuario.Nombre} {cita.Paciente.Usuario.Apellidos}",
                CostoTotal = cita.CostoTotalTratamientos,
                TotalPagado = totalPagado
            };
        }

        public async Task<int> RegistrarPagoAsync(PagoDtoIn dto)
        {
            var estadoCuenta = await ObtenerEstadoCuentaCitaAsync(dto.CitaId);

            if (estadoCuenta.Pendiente <= 0)
            {
                throw new Exception("Esta cita ya ha sido pagada en su totalidad.");
            }

            if (dto.Monto > estadoCuenta.Pendiente)
            {
                throw new Exception($"El monto ingresado ({dto.Monto:C}) excede el saldo pendiente ({estadoCuenta.Pendiente:C}).");
            }

            var cita = await _repositorio.Cita.ObtenerPorIdAsync(dto.CitaId);

            var pago = new Pago();
            pago.MapFromDto(dto);
            pago.PacienteId = cita.PacienteId;

            await _repositorio.Pago.AgregarAsync(pago);

            return pago.Id;
        }
    }
}
