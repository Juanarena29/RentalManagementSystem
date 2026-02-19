namespace CT.Application.DTOs;

/// <summary>
/// DTO para crear un nuevo usuario
/// </summary>
public record CrearUserDto(
    string Nombre,
    string Apellido,
    string Email,
    string Password
);
