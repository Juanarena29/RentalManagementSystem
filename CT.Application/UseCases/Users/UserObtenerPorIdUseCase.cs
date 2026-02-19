using CT.Application.DTOs;
using CT.Domain.Exceptions;
using CT.Domain.Interfaces;

namespace CT.Application.UseCases.Users;

/// <summary>
/// UseCase para obtener un usuario por su ID.
/// </summary>
public class UserObtenerPorIdUseCase
{
    private readonly IRepositorioUser _repositorioUser;

    public UserObtenerPorIdUseCase(IRepositorioUser repositorioUser)
    {
        _repositorioUser = repositorioUser;
    }

    public async Task<UserDto> EjecutarAsync(int userId)
    {
        var user = await _repositorioUser.ObtenerPorIdAsync(userId);

        if (user == null)
            throw new EntityNotFoundException($"Usuario con ID {userId} no encontrado");

        return new UserDto(user.Id, user.Nombre, user.Apellido, user.Email, user.DebeRestablecerPassword);
    }
}
