using CT.Domain.Enums;

namespace CT.Application.DTOs;

/// <summary>
/// DTO de SALIDA para operaciones de Pago.
/// Incluye ReservaId (a diferencia de PagoResumenDto que se usa
/// dentro de ReservaDetalleDto donde el contexto ya es la reserva).
/// </summary>
public class PagoDto
{
    public int PagoId { get; set; }
    public int ReservaId { get; set; }
    public DateTime FechaPago { get; set; }
    public decimal Monto { get; set; }
    public TipoPago TipoPago { get; set; }
    public MedioPago MedioPago { get; set; }
    public EstadoPago EstadoPago { get; set; }
    public string? Observaciones { get; set; }
}
