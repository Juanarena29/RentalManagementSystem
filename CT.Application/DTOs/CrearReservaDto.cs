using CT.Domain.Enums;

namespace CT.Application.DTOs;

/// <summary>
/// DTO de entrada para ReservaAltaUseCase.
/// 
/// ¿POR QUÉ EXISTE?
/// La entidad Reserva tiene 14 propiedades, pero al crear una reserva el usuario solo completa 6.
/// El resto lo calcula el UseCase:
///   - ReservaId → lo genera la BD
///   - Estado → siempre arranca en Pendiente
///   - PrecioPorNoche → se toma del Departamento
///   - MontoTotal → se calcula (noches × precio)
///   - SaldoPendiente → arranca igual al MontoTotal (se actualiza al registrar pagos)
///   - FechaCreacion → DateTime.UtcNow
/// 
/// Los pagos (señas, saldos, extras) se registran por separado con PagoRegistrarUseCase.
/// </summary>
public class CrearReservaDto
{
    public int DepartamentoId { get; set; }
    public int ClienteId { get; set; }
    public DateTime FechaInicio { get; set; }
    public DateTime FechaFin { get; set; }
    public int CantidadHuespedes { get; set; }
    public OrigenReserva OrigenReserva { get; set; }
    public string? Observaciones { get; set; }

    // NO incluye: ReservaId, Estado, PrecioPorNoche, MontoTotal, SaldoPendiente, MontoSenia, FechaCreacion
}
