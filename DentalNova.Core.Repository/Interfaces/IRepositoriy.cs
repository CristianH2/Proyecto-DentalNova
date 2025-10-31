using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DentalNova.Core.Repository.Interfaces
{
    public interface IRepositoriy
    {
        ITratamientoRepository Tratamiento { get; }
        IUsuarioRepository Usuario { get; }
        IArticuloRepository Articulo { get; }
    }
}
