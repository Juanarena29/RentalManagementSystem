namespace CT.Application.DTOs;

/// <summary>
/// DTO de entrada para UserModificarUseCase.
/// Solo permite editar nombre, apellido y email (no password).
/// </summary>
public class ModificarUserDto
{
    public int UserId { get; set; }
    public string Nombre { get; set; } = string.Empty;
    public string Apellido { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
}
