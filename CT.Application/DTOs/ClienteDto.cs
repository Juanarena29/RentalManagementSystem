namespace CT.Application.DTOs;

/// <summary>
/// DTO de SALIDA para operaciones de Cliente.
/// Evita exponer la entidad de dominio directamente.
/// </summary>
public class ClienteDto
{
    public int ClienteId { get; set; }
    public string Nombre { get; set; } = string.Empty;
    public string Apellido { get; set; } = string.Empty;
    public string DNI { get; set; } = string.Empty;
    public string? Telefono { get; set; }
    public string? Email { get; set; }
    public string? Ciudad { get; set; }
    public string? Provincia { get; set; }
    public string? Pais { get; set; }
    public string? Observaciones { get; set; }
    public DateTime FechaCreacion { get; set; }
}
