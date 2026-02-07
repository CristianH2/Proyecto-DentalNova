using System;
using System.Collections.Generic;
using System.Text;

namespace DentalNova.Movil.Models
{
    public class CitaCreateDto
    {
        public int Id { get; set; } = 0;
        public int PacienteId { get; set; }
        public int OdontologoId { get; set; }
        public DateTime FechaHora { get; set; }
        public int DuracionMinutos { get; set; } = 30; // Regla de negocio
        public string MotivoConsulta { get; set; }
        public int EstatusCita { get; set; } = 1; // 1 = Programada
        public List<int> TratamientosIds { get; set; } = new List<int> { 1 }; // 1 = Diagnóstico
    }
}
