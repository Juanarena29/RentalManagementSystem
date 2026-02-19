namespace CT.Application.DTOs;

/// <summary>
/// DTO para cambiar contraseña
/// </summary>
public record CambiarPasswordDto(
    int UserId,
    string PasswordActual,
    string PasswordNueva
);
