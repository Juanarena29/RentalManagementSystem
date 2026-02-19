using CT.Application.DTOs;
using CT.Application.Validations;
using CT.Domain.Entities;
using CT.Domain.Exceptions;
using CT.Domain.Interfaces;

namespace CT.Application.UseCases.Users;

/// <summary>
/// UseCase para crear un nuevo usuario del sistema.
/// Solo debe ser ejecutado por un administrador (validación de autorización se hará en la capa Web).
/// </summary>
public class UserCrearUseCase
{
    private readonly IRepositorioUser _repositorioUser;
    private readonly IPasswordHasher _passwordHasher;
    private readonly UserValidator _validador;

    public UserCrearUseCase(IRepositorioUser repositorioUser, IPasswordHasher passwordHasher, UserValidator validador)
    {
        _repositorioUser = repositorioUser;
        _passwordHasher = passwordHasher;
        _validador = validador;
    }

    public async Task<int> EjecutarAsync(CrearUserDto dto)
    {
        // 1. Validar datos de entrada
        _validador.ValidarCreacion(dto.Nombre, dto.Apellido, dto.Email, dto.Password);

        // 2. Verificar que el email no esté registrado
        bool emailExiste = await _repositorioUser.ExisteEmailAsync(dto.Email);
        if (emailExiste)
            throw new ValidationException($"Ya existe un usuario con el email '{dto.Email}'");

        // 3. Hashear la contraseña
        string passwordHash = _passwordHasher.Hash(dto.Password);

        // 4. Crear entidad User
        var user = new User(dto.Nombre.Trim(), dto.Apellido.Trim(), dto.Email.Trim().ToLowerInvariant())
        {
            PasswordHash = passwordHash
        };

        // 5. Persistir
        await _repositorioUser.AgregarAsync(user);

        return user.Id;
    }
}
