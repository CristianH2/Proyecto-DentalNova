namespace Proyecto_DentalNova.Models
{
    public class Tratamiento
    {
        public int Id { get; set; }
        public string Nombre { get; set; }
        public string? Descripcion { get; set; }
        public decimal Costo { get; set; }
        public int DuracionDias { get; set; }
        public bool Activo { get; set; }
        public List<CitaTratamiento> CitasTratamientos { get; set; }
    }
}
