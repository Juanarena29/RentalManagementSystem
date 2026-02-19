using CT.Domain.Enums;

namespace CT.Application.DTOs;

/// <summary>
/// DTO de entrada para PagoRegistrarUseCase.
/// 
/// ¿POR QUÉ EXISTE?
/// La entidad Pago tiene PagoId (lo genera la BD), FechaPago (la setea el sistema),
/// y EstadoPago (el sistema lo pone en Confirmado o PendienteVerificacion según la lógica).
/// El usuario solo elige a qué reserva paga, cuánto, por qué medio y de qué tipo.
/// </summary>
public class RegistrarPagoDto
{
    public int ReservaId { get; set; }
    public decimal Monto { get; set; }
    public TipoPago TipoPago { get; set; }
    public MedioPago MedioPago { get; set; }
    public string? Observaciones { get; set; }

    // NO incluye: PagoId (BD), FechaPago (sistema), EstadoPago (lógica del UseCase)
}
