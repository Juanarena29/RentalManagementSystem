using CT.Domain.Enums;

namespace CT.Application.DTOs;

/// <summary>
/// DTO de SALIDA para PagoObtenerReservasConSaldoPendienteUseCase.
/// 
/// ¿POR QUÉ EXISTE?
/// La pantalla de "cobros pendientes" necesita mostrar una lista de reservas que deben plata.
/// No necesita la entidad Reserva completa, solo:
///   - Qué reserva es y en qué departamento
///   - Quién es el huésped (nombre + teléfono para contactar por WhatsApp)
///   - Cuánto debe
///   - Cuándo es el check-in (para priorizar cobros)
/// 
/// El SaldoPendiente se CALCULA cruzando MontoTotal con la suma de pagos.
/// No se puede obtener de la entidad Reserva sola.
/// </summary>
public class ReservaConSaldoDto
{
    public int ReservaId { get; set; }
    public string DepartamentoNombre { get; set; } = string.Empty;
    public string ClienteNombreCompleto { get; set; } = string.Empty;
    public string? ClienteTelefono { get; set; }
    public DateTime FechaInicio { get; set; }
    public DateTime FechaFin { get; set; }
    public EstadoReserva Estado { get; set; }
    public decimal MontoTotal { get; set; }
    public decimal TotalPagado { get; set; }
    public decimal SaldoPendiente { get; set; }
}
