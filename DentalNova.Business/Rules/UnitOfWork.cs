using DentalNova.Core.Interfaces;
using DentalNova.Core.Repository.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DentalNova.Business.Rules
{
    public class UnitOfWork : IUnitOfWork
    {
        public ITratamientoBL Tratamiento { get; }
        public IUsuarioBL Usuario { get; }
        public IArticuloBL Articulo { get; }
        public IPacienteBL Paciente { get; }
        public ICitaBL Cita { get; }

        public UnitOfWork(
            ITratamientoBL tratamientoBL, 
            IUsuarioBL usuarioBL,
            IArticuloBL articuloBL,
            IPacienteBL pacienteBL,
            ICitaBL citaBL
            )
        {
            Tratamiento = tratamientoBL;
            Usuario = usuarioBL;
            Articulo = articuloBL;
            Paciente = pacienteBL;
            Cita = citaBL;
        }
    }
}
