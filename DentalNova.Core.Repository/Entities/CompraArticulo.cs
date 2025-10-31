using static DentalNova.Core.Repository.Entities.Enumerables;

namespace DentalNova.Core.Repository.Entities
{
    public class CompraArticulo
    {
        public int Id { get; set; }
        public int Cantidad { get; set; }
        public decimal PrecioUnitario { get; set; }
        public decimal Subtotal { get; set; }
        public DateTime FechaCompra { get; set; }
        public MetodoPago MetodoPago { get; set; }
        public string Proveedor { get; set; }
        public Articulo Articulo { get; set; }
    }
}
