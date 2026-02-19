
using CT.Domain.Enums;

namespace CT.Domain.Entities;

public class Pago
{
    public int PagoId { get; set; }
    public int ReservaId { get; set; }
    public DateTime FechaPago { get; set; }
    public decimal Monto { get; set; }
    public TipoPago TipoPago { get; set; }
    public MedioPago MedioPago { get; set; }
    public EstadoPago EstadoPago { get; set; }
    public string? Observaciones { get; set; }

    // Propiedad de navegación
    public Reserva? Reserva { get; set; }

    public Pago() { }

}
