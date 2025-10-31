namespace DentalNova.Core.Dtos
{
    public class TratamientoDto
    {
        public int Id { get; set; }
        public string Nombre { get; set; }
        public string? Descripcion { get; set; }
        public decimal Costo { get; set; }
    }
}
