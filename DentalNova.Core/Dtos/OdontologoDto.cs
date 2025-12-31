using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DentalNova.Core.Dtos
{
    public class OdontologoDto
    {
        public int Id { get; set; }
        public int UsuarioId { get; set; }

        // Datos del Usuario
        public string Nombre { get; set; }
        public string Apellidos { get; set; }
        public string NombreCompleto => $"{Nombre} {Apellidos}";
        public string CorreoElectronico { get; set; }
        public string Telefono { get; set; }

        // Datos del Odontólogo
        public string CedulaProfesional { get; set; }
        public int? AnioGraduacion { get; set; }
        public string? Institucion { get; set; }
        public DateTime FechaIngreso { get; set; }

        // Lista de nombres de especialidades (para mostrar en tabla)
        public List<string> Especialidades { get; set; } = new List<string>();

        // Lista de IDs de especialidades (útil para pre-llenar checkboxes en Edit)
        public List<int> EspecialidadesIds { get; set; } = new List<int>();
    }

    public class OdontologoDtoIn
    {
        [DisplayName("ID")]
        public int Id { get; set; }

        [Required(ErrorMessage = "Debe seleccionar un usuario para asociarlo como odontólogo.")]
        [DisplayName("Usuario")]
        public int UsuarioId { get; set; }

        [Required(ErrorMessage = "La cédula profesional es obligatoria.")]
        [StringLength(50)]
        [DisplayName("Cédula Profesional")]
        public string CedulaProfesional { get; set; }

        [Range(1950, 2100, ErrorMessage = "El año de graduación no es válido.")]
        [DisplayName("Año de Graduación")]
        public int? AnioGraduacion { get; set; }

        [StringLength(150)]
        [DisplayName("Institución")]
        public string? Institucion { get; set; }

        [Required(ErrorMessage = "La fecha de ingreso es obligatoria.")]
        [DataType(DataType.Date)]
        [DisplayName("Fecha de Ingreso")]
        public DateTime FechaIngreso { get; set; }

        // Lista de IDs de especialidades seleccionadas
        public List<int> EspecialidadesIds { get; set; } = new List<int>();
    }
}
