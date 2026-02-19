
using CT.Domain.Enums;

namespace CT.Domain.Entities;

public class Reserva
{
    public int ReservaId { get; set; }
    public int DepartamentoId { get; set; }
    public int ClienteId { get; set; }
    public DateTime FechaInicio { get; set; }
    public DateTime FechaFin { get; set; }
    public int CantidadHuespedes { get; set; }
    public EstadoReserva Estado { get; set; }
    public decimal PrecioPorNoche { get; set; }
    public decimal MontoTotal { get; set; }
    public decimal MontoSenia { get; set; }
    public decimal SaldoPendiente { get; set; }
    public OrigenReserva OrigenReserva { get; set; }
    public string? Observaciones { get; set; }
    public DateTime FechaCreacion { get; set; }

    // Propiedades de navegación
    public Departamento? Departamento { get; set; }
    public Cliente? Cliente { get; set; }

    public Reserva() { }

}
