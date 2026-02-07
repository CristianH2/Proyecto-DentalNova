using System.ComponentModel.DataAnnotations;

namespace Proyecto_DentalNova.Models.PerfilViewModel
{
    public class PerfilViewModel
    {
        // --- INFORMACIÓN DE LECTURA ---
        public int Id { get; set; }

        [Display(Name = "Nombre Completo")]
        public string NombreCompleto { get; set; }

        [Display(Name = "Correo Electrónico")]
        public string Correo { get; set; }

        [Display(Name = "Rol")]
        public string Rol { get; set; }

        // Datos extra (opcionales)
        public string Telefono { get; set; }
        public string FechaNacimiento { get; set; }

        // --- CAMBIO DE CONTRASEÑA ---

        [DataType(DataType.Password)]
        [Display(Name = "Contraseña Actual")]
        public string? PasswordActual { get; set; }

        [DataType(DataType.Password)]
        [Display(Name = "Nueva Contraseña")]
        [StringLength(100, ErrorMessage = "La {0} debe tener al menos {2} caracteres.", MinimumLength = 6)]
        public string? PasswordNuevo { get; set; }

        [DataType(DataType.Password)]
        [Display(Name = "Confirmar Nueva Contraseña")]
        [Compare("PasswordNuevo", ErrorMessage = "Las contraseñas no coinciden.")]
        public string? PasswordConfirmacion { get; set; }
    }
}
