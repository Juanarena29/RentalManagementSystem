namespace CT.Application.DTOs;

/// <summary>
/// DTO de entrada para ClienteModificarUseCase.
/// Solo expone los campos que el usuario puede editar.
/// No incluye: FechaCreacion (lo asignó el sistema al crear).
/// </summary>
public class ModificarClienteDto
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
}
