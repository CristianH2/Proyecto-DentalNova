using DentalNova.Core.Repository.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DentalNova.Core.Repository.Interfaces
{
    public interface IOdontologoRepository
    {
        Task<Odontologo> ObtenerPorIdAsync(int id); // Para obtener los datos del odontólogo asignado
    }
}
