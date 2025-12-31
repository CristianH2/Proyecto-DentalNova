using DentalNova.Core.Repository.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DentalNova.Core.Repository.Interfaces
{
    public interface ITratamientoRepository
    {
        // Existentes
        Task<IEnumerable<Tratamiento>> ObtenerTodosActivosAsync();
        Task<Tratamiento> ObtenerPorIdAsync(int id);

        // --- CRUD Admin ---
        IQueryable<Tratamiento> ObtenerQueryableParaFiltro();
        Task AgregarAsync(Tratamiento tratamiento);
        Task ActualizarAsync(Tratamiento tratamiento);
        Task EliminarAsync(int id);

        // Validación
        Task<bool> ExisteNombreAsync(string nombre, int? idExcluir = null);
    }
}
