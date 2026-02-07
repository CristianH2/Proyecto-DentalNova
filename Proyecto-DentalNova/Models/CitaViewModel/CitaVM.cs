using DentalNova.Core.Dtos;
using DentalNova.Core.Repository.Entities;
using Microsoft.AspNetCore.Mvc.Rendering;
using System.ComponentModel.DataAnnotations;

namespace Proyecto_DentalNova.Models.CitaViewModel
{
    public class CitaVM
    {
        // Datos de entrada para la API
        public CitaDtoIn Cita { get; set; } = new();

        // --- Listas para llenar los Selects en la Vista ---
        public IEnumerable<SelectListItem> Pacientes { get; set; } = new List<SelectListItem>();
        public IEnumerable<SelectListItem> Odontologos { get; set; } = new List<SelectListItem>();
        public IEnumerable<SelectListItem> Tratamientos { get; set; } = new List<SelectListItem>();

        // Listas de Enums (se pueden llenar en el controlador o usar helpers)
        public IEnumerable<SelectListItem> Duraciones { get; set; } = new List<SelectListItem>();
        public IEnumerable<SelectListItem> Estatus { get; set; } = new List<SelectListItem>();

        // Propiedad auxiliar para recibir los tratamientos seleccionados (Multi-select)
        [Display(Name = "Tratamientos a realizar")]
        public List<int> TratamientosSeleccionados { get; set; } = new List<int>();
    }
}
