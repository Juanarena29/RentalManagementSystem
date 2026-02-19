using CT.Domain.Enums;

namespace CT.Application.DTOs;

/// <summary>
/// DTO de SALIDA para ReservaObtenerDetalleUseCase.
/// 
/// ¿POR QUÉ EXISTE?
/// El detalle de una reserva necesita datos de 3 tablas: Reserva + Cliente + Pagos.
/// Si retornaras la entidad Reserva directamente tendrías estos problemas:
///   1. Referencias circulares (Reserva → Cliente → Reservas → ...)
///   2. No tendrías datos calculados como NombreCompleto, CantidadNoches, TotalPagado
///   3. La UI recibiría toda la entidad Cliente cuando solo necesita nombre y teléfono
/// 
/// Este DTO estructura exactamente lo que la pantalla de detalle necesita mostrar.
/// </summary>
public class ReservaDetalleDto
{
    // Datos de la reserva
    public int ReservaId { get; set; }
    public DateTime FechaInicio { get; set; }
    public DateTime FechaFin { get; set; }
    public int CantidadHuespedes { get; set; }
    public EstadoReserva Estado { get; set; }
    public decimal PrecioPorNoche { get; set; }
    public decimal MontoTotal { get; set; }
    public decimal MontoSenia { get; set; }
    public OrigenReserva OrigenReserva { get; set; }
    public string? Observaciones { get; set; }
    public DateTime FechaCreacion { get; set; }

    // Datos calculados (no existen en la entidad)
    public int CantidadNoches { get; set; }
    public decimal TotalPagado { get; set; }
    public decimal SaldoPendiente { get; set; }

    // Datos del departamento (solo lo que la UI necesita)
    public int DepartamentoId { get; set; }
    public string DepartamentoNombre { get; set; } = string.Empty;

    // Datos del cliente (solo lo que la UI necesita)
    public int ClienteId { get; set; }
    public string ClienteNombreCompleto { get; set; } = string.Empty;
    public string? ClienteTelefono { get; set; }

    // Pagos asociados
    public List<PagoResumenDto> Pagos { get; set; } = new();
}

/// <summary>
/// DTO de SALIDA para un pago dentro del detalle de reserva.
/// 
/// ¿POR QUÉ EXISTE?
/// Evita retornar la entidad Pago completa (que tiene navegación a Reserva → referencia circular).
/// Solo incluye lo que la tabla de pagos en la UI necesita mostrar.
/// </summary>
public class PagoResumenDto
{
    public int PagoId { get; set; }
    public DateTime FechaPago { get; set; }
    public decimal Monto { get; set; }
    public TipoPago TipoPago { get; set; }
    public MedioPago MedioPago { get; set; }
    public EstadoPago EstadoPago { get; set; }
    public string? Observaciones { get; set; }
}
