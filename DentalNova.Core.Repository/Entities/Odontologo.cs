using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace DentalNova.Core.Repository.Entities
{
    public class Odontologo
    {
        [DisplayName("ID")]
        public int Id { get; set; }

        [Required(ErrorMessage = "La cédula profesional es obligatoria.")]
        [StringLength(50)]
        [DisplayName("Cédula Profesional")]
        public string CedulaProfesional { get; set; }

        //[Required(ErrorMessage = "La especialidad es obligatoria.")]
        //[StringLength(100)]
        //[DisplayName("Especialidad")]
        //public string Especialidad { get; set; }

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

        // --- Llaves Foráneas (Foreign Keys) ---
        [Required(ErrorMessage = "Debe seleccionar un usuario para asociarlo como odontólogo.")]
        [DisplayName("Usuario")]
        public int UsuarioId { get; set; }

        // --- Propiedades de Navegación ---
        public virtual Usuario? Usuario { get; set; }

        // --- Colecciones (Relaciones de uno a muchos) ---
        public virtual List<HorarioOdontologo>? Horarios { get; set; }
        public virtual List<Cita>? Citas { get; set; }

        // --- Colecciones (Muchos a Muchos) ---
        public virtual List<Especialidad> Especialidades { get; set; } = new();
    }
}
