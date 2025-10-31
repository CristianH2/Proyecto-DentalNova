namespace DentalNova.Core.Repository.Entities
{
    public class Enumerables
    {
        public enum Categoria
        {
            Medicamento,
            EquipoMedico,
            Suministro,
            Otro
        }

        public enum DiaSemana
        {
            Lunes = 1,
            Martes = 2,
            Miercoles = 3,
            Jueves = 4,
            Viernes = 5,
            Sabado = 6,
            Domingo = 7
        }

        public enum EstatusCita
        {
            Programada = 1,
            Completada = 2,
            Cancelada = 3,
            NoAsistida = 4
        }

        public enum EstatusTratamiento
        {
            Pendiente = 1,
            EnProgreso = 2,
            Completado = 3,
            Cancelado = 4
        }

        public enum MetodoPago
        {
            Efectivo = 0,
            TarjetaDebito = 1,
            TarjetaCredito = 2
        }

        public enum DuracionMinutos
        {
            Veinte = 20,
            Treinta = 30,
            Cuarenta = 40,
            Cincuenta = 50,
            Sesenta = 60
        }
    }
}
