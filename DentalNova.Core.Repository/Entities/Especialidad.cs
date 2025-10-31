using System.ComponentModel.DataAnnotations;

namespace DentalNova.Core.Repository.Entities
{
    public class Especialidad
    {
        public int Id { get; set; }

        [Required]
        [StringLength(100)]
        public string Nombre { get; set; }

        // Propiedad de navegación: Una especialidad la pueden tener muchos odontólogos
        public virtual List<Odontologo> Odontologos { get; set; } = new();
    }
}
