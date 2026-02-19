using CT.Application.DTOs;
using CT.Application.Validations;
using CT.Domain.Exceptions;
using CT.Domain.Interfaces;

namespace CT.Application.UseCases.Users;

/// <summary>
/// UseCase para modificar los datos de un usuario (nombre, apellido, email).
/// Para cambiar contraseña usar UserCambiarPasswordUseCase.
/// </summary>
public class UserModificarUseCase
{
    private readonly IRepositorioUser _repositorioUser;
    private readonly UserValidator _validador;

    public UserModificarUseCase(IRepositorioUser repositorioUser, UserValidator validador)
    {
        _repositorioUser = repositorioUser;
        _validador = validador;
    }

    public async Task EjecutarAsync(ModificarUserDto dto)
    {
        // 1. Validar datos de entrada
        _validador.ValidarModificacion(dto.Nombre, dto.Apellido, dto.Email);

        // 2. Obtener usuario existente
        var user = await _repositorioUser.ObtenerPorIdAsync(dto.UserId);
        if (user == null)
            throw new EntityNotFoundException("Usuario no encontrado.");

        // 3. Si cambió el email, validar que no esté en uso por otro usuario
        if (!string.Equals(user.Email, dto.Email, StringComparison.OrdinalIgnoreCase))
        {
            if (await _repositorioUser.ExisteEmailAsync(dto.Email))
                throw new ValidationException($"Ya existe un usuario con el email '{dto.Email}'.");
        }

        // 4. Actualizar campos editables
        user.Nombre = dto.Nombre.Trim();
        user.Apellido = dto.Apellido.Trim();
        user.Email = dto.Email.Trim().ToLowerInvariant();

        // 5. Persistir
        await _repositorioUser.ModificarAsync(user);
    }
}
