using System.ComponentModel.DataAnnotations;
using static DentalNova.Core.Repository.Entities.Enumerables;

namespace DentalNova.Core.Repository.Entities
{
    public class Articulo
    {
        [Display(Name = "Id")]
        public int Id { get; set; }

        [Display(Name = "Categoría")]
        public Categoria Categoria { get; set; }

        [Display(Name = "Nombre")]
        public string Nombre { get; set; }

        [Display(Name = "Descripcion")]
        public string? Descripcion { get; set; }

        [Display(Name = "Codigo")]
        public string Codigo { get; set; }

        [Display(Name = "Reutilizable")]
        public bool Reutilizable { get; set; }

        [Display(Name = "Stock")]
        public int Stock { get; set; }

        [Display(Name = "Activo")]
        public bool Activo { get; set; }
        public List<CompraArticulo> compraArticulos { get; set; } = new List<CompraArticulo>();
    }
}
