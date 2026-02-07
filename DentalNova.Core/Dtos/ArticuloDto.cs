using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static DentalNova.Core.Repository.Entities.Enumerables;

namespace DentalNova.Core.Dtos
{
    public class ArticuloDto
    {
        public int Id { get; set; }
        public string CategoriaTexto { get; set; } // Para mostrar el nombre del Enum
        public Categoria Categoria { get; set; }
        public string Nombre { get; set; }
        public string Descripcion { get; set; }
        public string Codigo { get; set; }
        public bool Reutilizable { get; set; }
        public int Stock { get; set; }
        public bool Activo { get; set; }
        public string EstatusTexto => Activo ? "Activo" : "Inactivo";
    }

    public class ArticuloDtoIn
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "La categoría es obligatoria.")]
        public Categoria Categoria { get; set; }

        [Required(ErrorMessage = "El nombre es obligatorio.")]
        [StringLength(100)]
        public string Nombre { get; set; }

        [StringLength(500)]
        public string? Descripcion { get; set; }

        [Required(ErrorMessage = "El código es obligatorio.")]
        [StringLength(50)]
        public string Codigo { get; set; }

        public bool Reutilizable { get; set; }

        // El stock inicial se puede definir al crear, pero usualmente se modifica por compras
        public int Stock { get; set; }
        public bool Activo { get; set; } = true;
    }

    public class ArticuloFilterDto
    {
        public string? Busqueda { get; set; } // Buscará por Nombre o Código
        public Categoria? Categoria { get; set; }
        public bool? Activo { get; set; }

        public int Page { get; set; } = 1;
        public int PageSize { get; set; } = 10;
    }
}
