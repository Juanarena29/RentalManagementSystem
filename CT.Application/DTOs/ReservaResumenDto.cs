using CT.Domain.Enums;

namespace CT.Application.DTOs;

/// <summary>
/// DTO de SALIDA ligero para listados de reservas.
/// Incluye datos aplanados de Departamento y Cliente para evitar
/// exponer las entidades de navegación.
/// </summary>
public class ReservaResumenDto
{
    public int ReservaId { get; set; }
    public int DepartamentoId { get; set; }
    public string DepartamentoNombre { get; set; } = string.Empty;
    public int ClienteId { get; set; }
    public string ClienteNombreCompleto { get; set; } = string.Empty;
    public DateTime FechaInicio { get; set; }
    public DateTime FechaFin { get; set; }
    public int CantidadHuespedes { get; set; }
    public EstadoReserva Estado { get; set; }
    public decimal MontoTotal { get; set; }
    public decimal SaldoPendiente { get; set; }
    public OrigenReserva OrigenReserva { get; set; }
}
