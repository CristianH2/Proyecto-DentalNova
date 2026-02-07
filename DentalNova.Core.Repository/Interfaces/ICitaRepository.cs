using DentalNova.Core.Repository.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DentalNova.Core.Repository.Interfaces
{
    public interface ICitaRepository
    {
        // CRUD Estándar
        Task<Cita> ObtenerPorIdAsync(int id);
        IQueryable<Cita> ObtenerQueryable(); // Para filtros y paginación
        Task AgregarAsync(Cita cita);
        Task ActualizarAsync(Cita cita);
        Task EliminarAsync(int id); // Soft Delete (Cambiar estatus a Cancelada)

        // --- Validaciones Críticas ---

        /// <summary>
        /// Verifica si un odontólogo tiene conflicto de horario con otras citas.
        /// </summary>
        /// <param name="odontologoId">ID del doctor</param>
        /// <param name="inicio">Fecha y hora inicio</param>
        /// <param name="fin">Fecha y hora fin (calculada)</param>
        /// <param name="citaIdExcluir">ID de cita a ignorar (en caso de edición)</param>
        /// <returns>True si hay conflicto (ya está ocupado), False si está libre.</returns>
        Task<bool> ExisteConflictoHorarioAsync(int odontologoId, DateTime inicio, DateTime fin, int? citaIdExcluir = null);

        /// <summary>
        /// Verifica si la fecha solicitada cae dentro del horario laboral configurado del odontólogo.
        /// </summary>
        Task<bool> EsHorarioLaboralValidoAsync(int odontologoId, DateTime inicio, DateTime fin);
    }
}
