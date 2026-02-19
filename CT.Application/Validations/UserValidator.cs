using System.Text.RegularExpressions;
using CT.Domain.Exceptions;

namespace CT.Application.Validations;

public partial class UserValidator
{
    public void ValidarLogin(string email, string password)
    {
        if (string.IsNullOrWhiteSpace(email))
            throw new ValidationException("El email es obligatorio");

        if (string.IsNullOrWhiteSpace(password))
            throw new ValidationException("La contraseña es obligatoria");
    }

    public void ValidarCreacion(string nombre, string apellido, string email, string password)
    {
        // Validar nombre y apellido
        if (string.IsNullOrWhiteSpace(nombre) || nombre.Length < 2)
            throw new ValidationException("El nombre debe tener al menos 2 caracteres");

        if (string.IsNullOrWhiteSpace(apellido) || apellido.Length < 2)
            throw new ValidationException("El apellido debe tener al menos 2 caracteres");

        // Validar email
        if (string.IsNullOrWhiteSpace(email))
            throw new ValidationException("El email es obligatorio");

        if (!EsEmailValido(email))
            throw new ValidationException("El formato del email no es válido");

        // Validar contraseña
        ValidarFortalezaPassword(password);
    }

    public void ValidarModificacion(string nombre, string apellido, string email)
    {
        if (string.IsNullOrWhiteSpace(nombre) || nombre.Length < 2)
            throw new ValidationException("El nombre debe tener al menos 2 caracteres");

        if (string.IsNullOrWhiteSpace(apellido) || apellido.Length < 2)
            throw new ValidationException("El apellido debe tener al menos 2 caracteres");

        if (string.IsNullOrWhiteSpace(email))
            throw new ValidationException("El email es obligatorio");

        if (!EsEmailValido(email))
            throw new ValidationException("El formato del email no es válido");
    }

    public void ValidarCambioPassword(string passwordActual, string passwordNueva)
    {
        if (string.IsNullOrWhiteSpace(passwordActual))
            throw new ValidationException("Debe ingresar su contraseña actual");

        if (string.IsNullOrWhiteSpace(passwordNueva))
            throw new ValidationException("Debe ingresar la nueva contraseña");

        if (passwordActual == passwordNueva)
            throw new ValidationException("La nueva contraseña debe ser diferente a la actual");

        ValidarFortalezaPassword(passwordNueva);
    }

    private void ValidarFortalezaPassword(string password)
    {
        if (string.IsNullOrWhiteSpace(password))
            throw new ValidationException("La contraseña es obligatoria");

        if (password.Length < 8)
            throw new ValidationException("La contraseña debe tener al menos 8 caracteres");

        if (!MayusculaRegex().IsMatch(password))
            throw new ValidationException("La contraseña debe contener al menos una letra mayúscula");

        if (!NumeroRegex().IsMatch(password))
            throw new ValidationException("La contraseña debe contener al menos un número");
    }

    private static bool EsEmailValido(string email) => EmailRegex().IsMatch(email);

    [GeneratedRegex(@"^[\w\.\-]+@[\w\.\-]+\.\w+$")]
    private static partial Regex EmailRegex();

    [GeneratedRegex(@"[A-Z]")]
    private static partial Regex MayusculaRegex();

    [GeneratedRegex(@"[0-9]")]
    private static partial Regex NumeroRegex();
}
