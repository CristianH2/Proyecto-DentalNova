using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DentalNova.Core.Dtos
{
    public class UsuarioAdminDtoIn
    {
        [DisplayName("ID")]
        public int Id { get; set; } // 0 para crear, >0 para editar

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

        [Required(ErrorMessage = "La CURP es obligatoria.")]
        [StringLength(18, MinimumLength = 18, ErrorMessage = "La CURP debe tener 18 caracteres.")]
        [RegularExpression(@"^[A-Z][AEIOUX][A-Z]{2}[0-9]{2}(0[1-9]|1[0-2])(0[1-9]|[12][0-9]|3[01])[HM](AS|BC|BS|CC|CS|CH|CL|CM|DF|DG|GT|GR|HG|JC|MC|MN|MS|NT|NL|OC|PL|QT|QR|SP|SL|SR|TC|TS|TL|VZ|YN|ZS|NE)[B-DF-HJ-NP-TV-Z]{3}[0-9A-Z][0-9]$", ErrorMessage = "El formato de la CURP no es válido.")]
        [DisplayName("CURP")]
        public string CURP { get; set; }

        [Phone(ErrorMessage = "El formato del teléfono no es válido.")]
        [StringLength(15)]
        [DisplayName("Teléfono")]
        public string? Telefono { get; set; }

        [DataType(DataType.Date)] // Ayuda a que la vista muestre un control de solo fecha
        [DisplayName("Fecha de Nacimiento")]
        public DateTime? FechaNacimiento { get; set; }

        [DisplayName("Género")]
        public char? Genero { get; set; }

        [DisplayName("Activo")]
        public bool Activo { get; set; }

        // Opcional (solo si se cambia)
        public string? Password { get; set; }

        // Lista de roles seleccionados (ej. ["Administrador", "Odontologo", "Paciente"])
        public List<string> Roles { get; set; } = new List<string>();
    }

    public class UsuarioAdminDto
    {
        public int Id { get; set; }
        public string Nombre { get; set; }
        public string Apellidos { get; set; }
        public string NombreCompleto => $"{Nombre} {Apellidos}";
        public string CorreoElectronico { get; set; }
        public string CURP { get; set; }
        public string? Telefono { get; set; }
        public DateTime? FechaNacimiento { get; set; }
        public char? Genero { get; set; }
        public bool Activo { get; set; }
        public List<string> Roles { get; set; } = new List<string>();
    }
}
