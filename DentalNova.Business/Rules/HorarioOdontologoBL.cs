using DentalNova.Business.Helpers;
using DentalNova.Core.Dtos;
using DentalNova.Core.Interfaces;
using DentalNova.Core.Repository.Entities;
using DentalNova.Core.Repository.Interfaces;
using DentalNova.Repository.Daos;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DentalNova.Business.Rules
{
    internal class HorarioOdontologoBL : IHorarioOdontologoBL
    {
        private readonly IRepository _repositorio;

        public HorarioOdontologoBL(IRepository repositorio)
        {
            _repositorio = repositorio;
        }
        public async Task<HorarioOdontologoDto> ObtenerPorIdAsync(int id)
        {
            var entidad = await _repositorio.HorarioOdontologo.ObtenerPorIdAsync(id);
            return Mapeador.ToDto(entidad);
        }

        public async Task<List<HorarioOdontologoDto>> ObtenerPorOdontologoAsync(int odontologoId)
        {
            var entidades = await _repositorio.HorarioOdontologo.ObtenerPorOdontologoIdAsync(odontologoId);
            // Usamos LINQ para mapear la lista
            return entidades.Select(e => Mapeador.ToDto(e)).ToList();
        }

        public async Task<int> GuardarAsync(HorarioOdontologoDtoIn dto)
        {
            if (dto.HoraInicio >= dto.HoraFin)
                throw new Exception("La hora de inicio debe ser anterior a la hora de fin.");

            // Validar Solapamiento (Regla Crítica)
            // Pasamos dto.Id para excluirse a sí mismo si es edición
            var haySolapamiento = await _repositorio.HorarioOdontologo
                .ExisteSolapamientoAsync(dto.OdontologoId, dto.DiaSemana, dto.HoraInicio, dto.HoraFin, dto.Id == 0 ? null : dto.Id);

            if (haySolapamiento)
                throw new Exception($"El horario {dto.HoraInicio:hh\\:mm}-{dto.HoraFin:hh\\:mm} se superpone con otro horario existente para este odontólogo.");

            HorarioOdontologo entidad;

            if (dto.Id == 0) // CREATE
            {
                entidad = Mapeador.MapFromDto(dto);

                // Cargar relación obligatoria
                var odontologo = await _repositorio.Odontologo.ObtenerPorIdAsync(dto.OdontologoId);
                if (odontologo == null) throw new Exception("El odontólogo especificado no existe.");
                entidad.Odontologo = odontologo;

                await _repositorio.HorarioOdontologo.AgregarAsync(entidad);
            }
            else // UPDATE
            {
                entidad = await _repositorio.HorarioOdontologo.ObtenerPorIdAsync(dto.Id);
                if (entidad == null) throw new Exception("El horario a editar no existe.");

                Mapeador.MapFromDto(dto, entidad);

                // Si cambió el odontólogo
                if (entidad.Odontologo?.Id != dto.OdontologoId)
                {
                    var nuevoOdontologo = await _repositorio.Odontologo.ObtenerPorIdAsync(dto.OdontologoId);
                    if (nuevoOdontologo != null) entidad.Odontologo = nuevoOdontologo;
                }

                await _repositorio.HorarioOdontologo.ActualizarAsync(entidad);
            }

            return entidad.Id;
        }

        public async Task EliminarAsync(int id)
        {
            await _repositorio.HorarioOdontologo.EliminarAsync(id);
        }

        public async Task<bool> ValidarSolapamientoAsync(HorarioOdontologoDtoIn dto)
        {
            return await _repositorio.HorarioOdontologo
                .ExisteSolapamientoAsync(dto.OdontologoId, dto.DiaSemana, dto.HoraInicio, dto.HoraFin, dto.Id == 0 ? null : dto.Id);
        }
    }
}
