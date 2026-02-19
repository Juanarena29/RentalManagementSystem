using CT.Domain.Entities;

namespace CT.Domain.Interfaces;

/// <summary>
/// Repositorio para gestión de Users (administradores/empleados)
/// </summary>
public interface IRepositorioUser
{
    // ========== CRUD ==========
    Task AgregarAsync(User user);
    Task ModificarAsync(User user);
    Task EliminarAsync(int id);
    Task<User?> ObtenerPorIdAsync(int id);
    Task<List<User>> ListarAsync();
    Task<bool> ExisteAsync(int id);

    // ========== Autenticación ==========
    /// <summary>
    /// Busca un usuario por email (usado en login)
    /// </summary>
    Task<User?> ObtenerPorEmailAsync(string email);

    /// <summary>
    /// Verifica si ya existe un usuario con ese email
    /// </summary>
    Task<bool> ExisteEmailAsync(string email);
}
