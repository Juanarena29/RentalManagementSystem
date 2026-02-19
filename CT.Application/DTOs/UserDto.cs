namespace CT.Application.DTOs;

/// <summary>
/// DTO de salida para User. Excluye PasswordHash por seguridad.
/// </summary>
public record UserDto(
    int Id,
    string Nombre,
    string Apellido,
    string Email,
    bool DebeRestablecerPassword
);
