using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace DentalNova.Core.Repository.Entities
{
    public class Usuario
    {
        [Key] // Indica que es la clave primaria
        [DisplayName("ID")]
        public int Id { get; set; }

        [Required(ErrorMessage = "El nombre es obligatorio.")]
        [StringLength(50, ErrorMessage = "El nombre no puede exceder los 50 caracteres.")]
        [DisplayName("Nombre")]
        public string Nombre { get; set; }

        [Required(ErrorMessage = "Los apellidos son obligatorios.")]
        [StringLength(100, ErrorMessage = "Los apellidos no pueden exceder los 100 caracteres.")]
        [DisplayName("Apellidos")]
        public string Apellidos { get; set; }

        [Required(ErrorMessage = "El correo electrónico es obligatorio.")]
        [EmailAddress(ErrorMessage = "El formato del correo electrónico no es válido.")]
        [StringLength(100)]
        [DisplayName("Correo Electrónico")]
        public string CorreoElectronico { get; set; }

        [Phone(ErrorMessage = "El formato del teléfono no es válido.")]
        [StringLength(15)]
        [DisplayName("Teléfono")]
        public string? Telefono { get; set; }

        [DataType(DataType.Date)] // Ayuda a que la vista muestre un control de solo fecha
        [DisplayName("Fecha de Nacimiento")]
        public DateTime? FechaNacimiento { get; set; }

        // * Char (M/F/O) Dropdown list *
        [DisplayName("Género")]
        public char? Genero { get; set; }

        [Required(ErrorMessage = "La CURP es obligatoria.")]
        [StringLength(18, MinimumLength = 18, ErrorMessage = "La CURP debe tener 18 caracteres.")]
        [RegularExpression(@"^[A-Z][AEIOUX][A-Z]{2}[0-9]{2}(0[1-9]|1[0-2])(0[1-9]|[12][0-9]|3[01])[HM](AS|BC|BS|CC|CS|CH|CL|CM|DF|DG|GT|GR|HG|JC|MC|MN|MS|NT|NL|OC|PL|QT|QR|SP|SL|SR|TC|TS|TL|VZ|YN|ZS|NE)[B-DF-HJ-NP-TV-Z]{3}[0-9A-Z][0-9]$", ErrorMessage = "El formato de la CURP no es válido.")]
        [DisplayName("CURP")]
        public string CURP { get; set; }

        [DisplayName("Contraseña")]
        public string Password { get; set; }

        // * Checkbox *
        [DisplayName("Activo")]
        public bool Activo { get; set; } = true; // Valor por defecto

        // --- Colecciones (Relaciones de uno a muchos) ---
        public List<Rol>? Roles { get; set; }
        public List<LogActividad>? LogActividades { get; set; }
    }
}
