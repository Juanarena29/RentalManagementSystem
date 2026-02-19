using System.Security.Cryptography;
using CT.Domain.Interfaces;

namespace CT.Infraestructure.Services;

/// <summary>
/// Implementación de hashing de contraseñas usando PBKDF2.
/// Registrar como Singleton en el DI container.
/// </summary>
public class PasswordHasher : IPasswordHasher
{
    private const int SaltSize = 16;
    private const int KeySize = 32;
    private const int Iterations = 100_000;
    private static readonly HashAlgorithmName Algorithm = HashAlgorithmName.SHA256;

    /// <summary>
    /// Genera un hash seguro de la contraseña usando PBKDF2
    /// </summary>
    public string Hash(string password)
    {
        var salt = RandomNumberGenerator.GetBytes(SaltSize);
        var key = Rfc2898DeriveBytes.Pbkdf2(
            password, salt, Iterations, Algorithm, KeySize);

        return $"{Convert.ToBase64String(salt)}.{Convert.ToBase64String(key)}";
    }

    /// <summary>
    /// Verifica si una contraseña coincide con el hash almacenado
    /// </summary>
    public bool Verify(string password, string passwordHash)
    {
        var parts = passwordHash.Split('.');
        if (parts.Length != 2)
            return false;

        var salt = Convert.FromBase64String(parts[0]);
        var storedKey = Convert.FromBase64String(parts[1]);

        var key = Rfc2898DeriveBytes.Pbkdf2(
            password, salt, Iterations, Algorithm, KeySize);

        return CryptographicOperations.FixedTimeEquals(key, storedKey);
    }
}
