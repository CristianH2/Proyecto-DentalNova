using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DentalNova.Core.Interfaces
{
    public interface IUnitOfWork
    {
        IUsuarioBL Usuario { get; }
        ITratamientoBL Tratamiento { get; }
        IArticuloBL Articulo { get; }
    }
}
