namespace CT.Domain.Interfaces;

/// <summary>
/// Interface para hashear y verificar contraseñas.
/// Abstrae el algoritmo de hashing para mantener Clean Architecture.
/// </summary>
public interface IPasswordHasher
{
    /// <summary>
    /// Genera un hash seguro de la contraseña
    /// </summary>
    string Hash(string password);

    /// <summary>
    /// Verifica si una contraseña coincide con el hash almacenado
    /// </summary>
    bool Verify(string password, string passwordHash);
}
