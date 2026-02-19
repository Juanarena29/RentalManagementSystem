using System.Text.RegularExpressions;
using CT.Application.DTOs;
using CT.Domain.Exceptions;

namespace CT.Application.Validations;

/// <summary>
/// Validaciones de formato para Cliente.
/// Se usan en ClienteAltaUseCase y ClienteModificarUseCase.
/// Las validaciones de negocio (DNI duplicado) van en el UseCase.
/// </summary>
public partial class ClienteValidator
{
    /// <summary>
    /// Valida los datos para crear un cliente nuevo
    /// </summary>
    public void Validar(CrearClienteDto dto)
    {
        ValidarNombre(dto.Nombre);
        ValidarApellido(dto.Apellido);
        ValidarDni(dto.DNI);
        ValidarEmail(dto.Email);
        ValidarTelefono(dto.Telefono);
    }

    /// <summary>
    /// Valida los datos al modificar un cliente existente
    /// </summary>
    public void Validar(ModificarClienteDto dto)
    {
        ValidarNombre(dto.Nombre);
        ValidarApellido(dto.Apellido);
        ValidarDni(dto.DNI);
        ValidarEmail(dto.Email);
        ValidarTelefono(dto.Telefono);
    }

    private void ValidarNombre(string nombre)
    {
        if (string.IsNullOrWhiteSpace(nombre))
            throw new ValidationException("El nombre es requerido.");
    }

    private void ValidarApellido(string apellido)
    {
        if (string.IsNullOrWhiteSpace(apellido))
            throw new ValidationException("El apellido es requerido.");
    }

    private void ValidarDni(string dni)
    {
        if (string.IsNullOrWhiteSpace(dni))
            throw new ValidationException("El DNI es requerido.");

        if (dni.Length < 7 || dni.Length > 8)
            throw new ValidationException("El DNI debe tener entre 7 y 8 caracteres.");

        if (!DniRegex().IsMatch(dni))
            throw new ValidationException("El DNI debe contener solo números.");
    }

    private void ValidarEmail(string? email)
    {
        if (string.IsNullOrWhiteSpace(email))
            return; // Email es opcional

        if (!EmailRegex().IsMatch(email))
            throw new ValidationException("El formato del email es inválido.");
    }

    private void ValidarTelefono(string? telefono)
    {
        if (string.IsNullOrWhiteSpace(telefono))
            return; // Teléfono es opcional

        // Permitir +, números, espacios y guiones
        if (!TelefonoRegex().IsMatch(telefono))
            throw new ValidationException("El formato del teléfono es inválido.");

        if (telefono.Length < 8)
            throw new ValidationException("El teléfono debe tener al menos 8 caracteres.");
    }

    [GeneratedRegex(@"^\d+$")]
    private static partial Regex DniRegex();

    [GeneratedRegex(@"^[^@\s]+@[^@\s]+\.[^@\s]+$")]
    private static partial Regex EmailRegex();

    [GeneratedRegex(@"^[\d\s\-\+\(\)]+$")]
    private static partial Regex TelefonoRegex();
}
