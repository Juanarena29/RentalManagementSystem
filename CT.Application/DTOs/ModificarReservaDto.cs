using CT.Domain.Enums;

namespace CT.Application.DTOs;

/// <summary>
/// DTO de entrada para ReservaModificarUseCase.
/// Solo expone los campos que el usuario puede editar en una reserva existente.
/// No incluye: Estado, PrecioPorNoche, MontoTotal, MontoSenia, SaldoPendiente, FechaCreacion
/// (todos calculados o gestionados por otros UseCases).
/// </summary>
public class ModificarReservaDto
{
    public int ReservaId { get; set; }
    public int DepartamentoId { get; set; }
    public int ClienteId { get; set; }
    public DateTime FechaInicio { get; set; }
    public DateTime FechaFin { get; set; }
    public int CantidadHuespedes { get; set; }
    public OrigenReserva OrigenReserva { get; set; }
    public string? Observaciones { get; set; }
}
