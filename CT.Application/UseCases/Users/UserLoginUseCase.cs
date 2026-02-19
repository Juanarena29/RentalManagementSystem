using CT.Application.DTOs;
using CT.Application.Validations;
using CT.Domain.Exceptions;
using CT.Domain.Interfaces;

namespace CT.Application.UseCases.Users;

/// <summary>
/// UseCase para autenticar un usuario por email y contraseña.
/// Retorna UserDto (sin PasswordHash) si las credenciales son válidas.
/// </summary>
public class UserLoginUseCase
{
    private readonly IRepositorioUser _repositorioUser;
    private readonly IPasswordHasher _passwordHasher;
    private readonly UserValidator _validador;

    public UserLoginUseCase(IRepositorioUser repositorioUser, IPasswordHasher passwordHasher, UserValidator validador)
    {
        _repositorioUser = repositorioUser;
        _passwordHasher = passwordHasher;
        _validador = validador;
    }

    public async Task<UserDto> EjecutarAsync(LoginDto dto)
    {
        // 1. Validar datos de entrada
        _validador.ValidarLogin(dto.Email, dto.Password);

        // 2. Buscar usuario por email (normalizado a minúsculas)
        var user = await _repositorioUser.ObtenerPorEmailAsync(dto.Email.Trim().ToLowerInvariant());
        if (user == null)
            throw new WrongCredentialsException("Email o contraseña incorrectos");

        // 3. Verificar contraseña usando PasswordHasher
        bool esPasswordValida = _passwordHasher.Verify(dto.Password, user.PasswordHash);

        if (!esPasswordValida)
            throw new WrongCredentialsException("Email o contraseña incorrectos");

        // 4. Retornar usuario autenticado (sin PasswordHash)
        return new UserDto(user.Id, user.Nombre, user.Apellido, user.Email, user.DebeRestablecerPassword);
    }
}
