using CT.Domain.Enums;

namespace CT.Application.DTOs;

/// <summary>
/// DTO de entrada para DepartamentoModificacionUseCase.
/// Solo expone los campos editables.
/// No incluye: Estado (se cambia por UseCases específicos de baja/activar).
/// </summary>
public class ModificarDepartamentoDto
{
    public int DepartamentoId { get; set; }
    public string Nombre { get; set; } = string.Empty;
    public string? Descripcion { get; set; }
    public int CapacidadMaxima { get; set; }
    public decimal PrecioPorNoche { get; set; }
    public string? Observaciones { get; set; }
}
