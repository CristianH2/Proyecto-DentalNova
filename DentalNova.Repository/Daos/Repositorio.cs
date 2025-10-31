using DentalNova.Core.Repository.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DentalNova.Repository.Daos
{
    public class Repositorio : IRepositoriy
    {
        public ITratamientoRepository Tratamiento { get; }
        public IUsuarioRepository Usuario { get; }
        public IArticuloRepository Articulo { get; }

        public Repositorio( 
            ITratamientoRepository tratamientoRepository, 
            IUsuarioRepository usuarioRepository,
            IArticuloRepository articuloRepository
            )
        {
            Tratamiento = tratamientoRepository;
            Usuario = usuarioRepository;
            Articulo = articuloRepository;
        }
    }
}
