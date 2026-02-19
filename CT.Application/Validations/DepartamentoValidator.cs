using CT.Application.DTOs;
using CT.Domain.Exceptions;

namespace CT.Application.Validations;

/// <summary>
/// Validaciones de formato para Departamento.
/// Se usan en DepartamentoAltaUseCase y DepartamentoModificacionUseCase.
/// Las validaciones de negocio (nombre duplicado, reservas activas) van en el UseCase.
/// </summary>
public class DepartamentoValidator
{
    /// <summary>
    /// Valida los datos para crear un departamento nuevo
    /// </summary>
    public void Validar(CrearDepartamentoDto dto)
    {
        ValidarNombre(dto.Nombre);
        ValidarCapacidad(dto.CapacidadMaxima);
        ValidarPrecio(dto.PrecioPorNoche);
    }

    /// <summary>
    /// Valida los datos al modificar un departamento existente
    /// </summary>
    public void Validar(ModificarDepartamentoDto dto)
    {
        ValidarNombre(dto.Nombre);
        ValidarCapacidad(dto.CapacidadMaxima);
        ValidarPrecio(dto.PrecioPorNoche);
    }

    private void ValidarNombre(string nombre)
    {
        if (string.IsNullOrWhiteSpace(nombre))
            throw new ValidationException("El nombre del departamento es requerido.");
    }

    private void ValidarCapacidad(int capacidad)
    {
        if (capacidad <= 0)
            throw new ValidationException("La capacidad máxima debe ser mayor a 0.");
    }

    private void ValidarPrecio(decimal precio)
    {
        if (precio <= 0)
            throw new ValidationException("El precio por noche debe ser mayor a 0.");
    }
}
