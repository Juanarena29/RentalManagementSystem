using CT.Domain.Enums;

namespace CT.Application.DTOs;

/// <summary>
/// DTO de SALIDA para operaciones de Departamento.
/// Evita exponer la entidad de dominio directamente.
/// </summary>
public class DepartamentoDto
{
    public int DepartamentoId { get; set; }
    public string Nombre { get; set; } = string.Empty;
    public string? Descripcion { get; set; }
    public int CapacidadMaxima { get; set; }
    public decimal PrecioPorNoche { get; set; }
    public EstadoDepartamento Estado { get; set; }
    public string? Observaciones { get; set; }
}
