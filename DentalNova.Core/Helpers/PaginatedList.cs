using Microsoft.EntityFrameworkCore;

namespace DentalNova.Core.Helpers
{
    public class PaginatedList<T> : List<T>, IPaginatedList
    {
        public int PageIndex { get; private set; }
        public int TotalPages { get; private set; }
        public int TotalCount { get; private set; }

        public bool HasPreviousPage => PageIndex > 1;
        public bool HasNextPage => PageIndex < TotalPages;

        private PaginatedList(List<T> items, int count, int pageIndex, int pageSize)
        {
            TotalCount = count;
            PageIndex = pageIndex;
            TotalPages = (int)Math.Ceiling(count / (double)pageSize);
            this.AddRange(items);
        }

        public static async Task<PaginatedList<T>> CreateAsync(IQueryable<T> source, int pageIndex, int pageSize)
        {
            // Cuenta el total de registros en la consulta antes de paginar.
            var count = await source.CountAsync();

            // Se salta los registros de las páginas anteriores y toma solo los de la página actual.
            var items = await source.Skip((pageIndex - 1) * pageSize).Take(pageSize).ToListAsync();

            // Devuelve una nueva instancia de sí misma con los resultados y los datos de paginación.
            return new PaginatedList<T>(items, count, pageIndex, pageSize);
        }


        // Crea una PaginatedList a partir de una lista de objetos y metadatos ya existentes (API).
        public static PaginatedList<T> Create(IEnumerable<T> source, int count, int pageIndex, int pageSize)
        {
            return new PaginatedList<T>(source.ToList(), count, pageIndex, pageSize);
        }
    }
}
