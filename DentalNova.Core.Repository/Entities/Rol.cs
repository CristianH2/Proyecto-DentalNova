namespace DentalNova.Core.Repository.Entities
{
    public class Rol
    {
        public int Id { get; set; }
        public string Nombre { get; set; }
        public string Descripcion { get; set; }
        public Usuario Usuario { get; set; }
    }
}
