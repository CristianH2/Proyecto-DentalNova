using DentalNova.Core.Repository.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DentalNova.Core.Repository.Interfaces
{
    public interface IArticuloRepository
    {
        // --- CRUD ---
        Task<Articulo> ObtenerPorIdAsync(int id);
        IQueryable<Articulo> ObtenerQueryable();
        Task AgregarAsync(Articulo articulo);
        Task ActualizarAsync(Articulo articulo);
        Task EliminarAsync(int id);

        // --- Validaciones ---
        Task<bool> ExisteCodigoAsync(string codigo, int? idExcluir = null);
    }
}
