using CT.Application.DTOs;
using CT.Domain.Interfaces;

namespace CT.Application.UseCases.Users;

/// <summary>
/// UseCase para listar todos los usuarios del sistema.
/// Solo debe estar disponible para administradores.
/// </summary>
public class UserListarUseCase
{
    private readonly IRepositorioUser _repositorioUser;

    public UserListarUseCase(IRepositorioUser repositorioUser)
    {
        _repositorioUser = repositorioUser;
    }

    public async Task<List<UserDto>> EjecutarAsync()
    {
        var users = await _repositorioUser.ListarAsync();
        return users.Select(u => new UserDto(u.Id, u.Nombre, u.Apellido, u.Email, u.DebeRestablecerPassword)).ToList();
    }
}
