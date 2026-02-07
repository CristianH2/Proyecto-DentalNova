using System;
using System.Collections.Generic;
using System.Text;

namespace DentalNova.Movil.Services
{
    public class UnitOfWork
    {
        public InicioDeSesionService AuthService { get; }
        public ConfiguracionService Config { get; }
        public CitaService CitaService { get; }
        public OdontologoService OdontologoService { get; set; }
        public NotificacionService NotificacionService { get; set; }
        public PerfilService PerfilService { get; set; }

        // Constructor con Inyección de Dependencias
        public UnitOfWork(InicioDeSesionService authService, ConfiguracionService configService, CitaService citaService, OdontologoService odontologoService, NotificacionService notificacionService, PerfilService perfilService)
        {
            AuthService = authService;
            Config = configService;
            CitaService = citaService;
            OdontologoService = odontologoService;
            NotificacionService = notificacionService;
            PerfilService = perfilService;
        }
    }
}
