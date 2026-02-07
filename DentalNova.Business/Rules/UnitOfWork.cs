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
        public IOdontologoBL Odontologo { get; }
        public IHorarioOdontologoBL HorarioOdontologo { get; }
        public IPagoBL Pago { get; }
        public IRecordatorioBL Recordatorio { get; }
        public IDashboardBL Dashboard { get; }
        public IAuthBL Auth { get; }

        public UnitOfWork(
            ITratamientoBL tratamientoBL, 
            IUsuarioBL usuarioBL,
            IArticuloBL articuloBL,
            IPacienteBL pacienteBL,
            ICitaBL citaBL,
            IOdontologoBL odontologoBL,
            IHorarioOdontologoBL horarioOdontologoBL,
            IPagoBL pagoBL,
            IRecordatorioBL recordatorioBL,
            IDashboardBL dashboard,
            IAuthBL auth)
        {
            Tratamiento = tratamientoBL;
            Usuario = usuarioBL;
            Articulo = articuloBL;
            Paciente = pacienteBL;
            Cita = citaBL;
            Odontologo = odontologoBL;
            HorarioOdontologo = horarioOdontologoBL;
            Pago = pagoBL;
            Recordatorio = recordatorioBL;
            Dashboard = dashboard;
            Auth = auth;
        }
    }
}
