using CT.Application.DTOs;
using CT.Application.Validations;
using CT.Domain.Exceptions;
using CT.Domain.Interfaces;

namespace CT.Application.UseCases.Users;

/// <summary>
/// UseCase para cambiar la contraseña de un usuario.
/// Requiere la contraseña actual para mayor seguridad.
/// </summary>
public class UserCambiarPasswordUseCase
{
    private readonly IRepositorioUser _repositorioUser;
    private readonly IPasswordHasher _passwordHasher;
    private readonly UserValidator _validador;

    public UserCambiarPasswordUseCase(IRepositorioUser repositorioUser, IPasswordHasher passwordHasher, UserValidator validador)
    {
        _repositorioUser = repositorioUser;
        _passwordHasher = passwordHasher;
        _validador = validador;
    }

    public async Task EjecutarAsync(CambiarPasswordDto dto)
    {
        // 1. Validar datos de entrada
        _validador.ValidarCambioPassword(dto.PasswordActual, dto.PasswordNueva);

        // 2. Obtener usuario
        var user = await _repositorioUser.ObtenerPorIdAsync(dto.UserId);
        if (user == null)
            throw new EntityNotFoundException("Usuario no encontrado");

        // 3. Verificar que la contraseña actual sea correcta
        bool esPasswordActualValida = _passwordHasher.Verify(
            dto.PasswordActual, user.PasswordHash);

        if (!esPasswordActualValida)
            throw new WrongCredentialsException("La contraseña actual es incorrecta");

        // 4. Generar nuevo hash de la contraseña nueva
        user.PasswordHash = _passwordHasher.Hash(dto.PasswordNueva);

        // 5. Marcar que ya no debe restablecer la contraseña
        user.DebeRestablecerPassword = false;

        // 6. Persistir cambio
        await _repositorioUser.ModificarAsync(user);
    }
}
