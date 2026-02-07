using DentalNova.Core.Repository.Entities;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static DentalNova.Core.Repository.Entities.Enumerables;

namespace DentalNova.Core.Dtos
{
    public class PagoDto
    {
        public int Id { get; set; }
        public decimal Monto { get; set; }
        public DateTime FechaPago { get; set; }
        public MetodoPago MetodoPago { get; set; }
        public string MetodoPagoTexto => MetodoPago.ToString();

        public int CitaId { get; set; }
        public string PacienteNombre { get; set; }
        public string OdontologoNombre { get; set; } // Para referencia
    }

    // Para Crear
    public class PagoDtoIn
    {
        [Required]
        public int CitaId { get; set; }

        // El PacienteId lo sacamos de la Cita en el Backend para seguridad, 
        // o lo enviamos si es necesario, pero mejor inferirlo.

        [Required]
        [Range(0.01, double.MaxValue, ErrorMessage = "El monto debe ser mayor a 0")]
        public decimal Monto { get; set; }

        [Required]
        public MetodoPago MetodoPago { get; set; }
    }

    // Para visualizar el estado de cuenta de una Cita antes de pagar
    public class EstadoCuentaCitaDto
    {
        public int CitaId { get; set; }
        public decimal CostoTotal { get; set; }
        public decimal TotalPagado { get; set; }
        public decimal Pendiente => CostoTotal - TotalPagado;
        public string PacienteNombre { get; set; }
    }

    // Filtros
    public class PagoFilterDto
    {
        public int Page { get; set; } = 1;
        public int PageSize { get; set; } = 10;
        public int? PacienteId { get; set; }
        public DateTime? FechaInicio { get; set; }
        public DateTime? FechaFin { get; set; }
    }
}
