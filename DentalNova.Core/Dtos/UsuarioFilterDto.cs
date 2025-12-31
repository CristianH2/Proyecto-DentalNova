using DentalNova.Core.Helpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DentalNova.Core.Dtos
{
    public class UsuarioFilterDto : PaginacionDto // Reutilizamos el DTO de paginación
    {
        public int? Id { get; set; }
        public string? NombreLike { get; set; }
        public string? ApellidosLike { get; set; }
        public string? CorreoLike { get; set; }
        public string? TelefonoLike { get; set; }
        public char? Genero { get; set; }
        public bool? Activo { get; set; }
    }

    // DTO base para paginación
    public class PaginacionDto
    {
        public int Page { get; set; } = 1;
        public int PageSize { get; set; } = 10;
    }
}
