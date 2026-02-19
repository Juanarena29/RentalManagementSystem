using CT.Domain.Enums;

namespace CT.Application.DTOs;

/// <summary>
/// DTO de SALIDA para ObtenerCalendarioOcupacionUseCase.
/// 
/// ¿POR QUÉ EXISTE?
/// El calendario de ocupación muestra bloques de reserva por departamento.
/// No necesita TODA la info de la reserva (14 campos), solo lo mínimo para
/// pintar un bloque en el calendario:
///   - Qué departamento
///   - De cuándo a cuándo
///   - Nombre del huésped (para mostrar al hacer hover/click)
///   - Estado (para colorear: verde=confirmada, amarillo=pendiente, rojo=cancelada)
/// 
/// Es una vista aplanada y simplificada de Reserva + Cliente + Departamento.
/// </summary>
public class CalendarioOcupacionDto
{
    public int ReservaId { get; set; }
    public int DepartamentoId { get; set; }
    public string DepartamentoNombre { get; set; } = string.Empty;
    public DateTime FechaInicio { get; set; }
    public DateTime FechaFin { get; set; }
    public string ClienteNombreCompleto { get; set; } = string.Empty;
    public string? ClienteTelefono { get; set; }
    public EstadoReserva Estado { get; set; }
    public int CantidadHuespedes { get; set; }
}
