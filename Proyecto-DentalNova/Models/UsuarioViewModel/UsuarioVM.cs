using DentalNova.Core.Dtos;
using Microsoft.AspNetCore.Mvc.Rendering;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace Proyecto_DentalNova.Models.UsuarioViewModel
{
    public class UsuarioVM
    {
        //public Usuario Usuario { get; set; } = new();
        public UsuarioAdminDtoIn Usuario { get; set; } = new();
        public IEnumerable<SelectListItem> Generos { get; set; } = new List<SelectListItem>();

        [DataType(DataType.Password)] // Oculta los caracteres en los formularios
        [StringLength(100, MinimumLength = 8, ErrorMessage = "La contraseña debe tener al menos 8 caracteres.")]
        [DisplayName("Contraseña")]
        public string? NewPassword { get; set; }

        [DataType(DataType.Password)]
        [Display(Name = "Confirmar Contraseña")]
        [Compare("NewPassword", ErrorMessage = "Las contraseñas no coinciden.")]
        public string? ConfirmPassword { get; set; }

        public List<string> RolesSeleccionados { get; set; } = new List<string>();
        public List<SelectListItem> RolesDisponibles { get; set; } = new List<SelectListItem>();
    }

}
