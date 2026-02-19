namespace CT.Application.DTOs;

/// <summary>
/// DTO para login de usuario
/// </summary>
public record LoginDto(
    string Email,
    string Password
);
