using CT.Application.DTOs;
using CT.Domain.Exceptions;

namespace CT.Application.Validations;

/// <summary>
/// Validaciones de formato para Reserva.
/// Se usan en ReservaAltaUseCase y ReservaModificarUseCase.
/// Las validaciones de negocio (solapamiento, existencia depto/cliente, capacidad) van en el UseCase.
/// </summary>
public class ReservaValidator
{
    /// <summary>
    /// Valida los datos para crear una reserva nueva
    /// </summary>
    public void Validar(CrearReservaDto dto)
    {
        ValidarFechas(dto.FechaInicio, dto.FechaFin);
        ValidarFechaNoEnPasado(dto.FechaInicio);
        ValidarHuespedes(dto.CantidadHuespedes);
    }

    /// <summary>
    /// Valida los datos al modificar una reserva existente
    /// </summary>
    public void Validar(ModificarReservaDto dto)
    {
        ValidarFechas(dto.FechaInicio, dto.FechaFin);
        ValidarHuespedes(dto.CantidadHuespedes);
    }

    /// <summary>
    /// Valida las fechas para consulta de disponibilidad
    /// </summary>
    public void ValidarRangoFechas(DateTime fechaInicio, DateTime fechaFin)
    {
        ValidarFechas(fechaInicio, fechaFin);
    }

    private void ValidarFechas(DateTime fechaInicio, DateTime fechaFin)
    {
        if (fechaInicio == default)
            throw new FechasInvalidasException("La fecha de inicio es requerida.");

        if (fechaFin == default)
            throw new FechasInvalidasException("La fecha de fin es requerida.");

        if (fechaInicio.Date >= fechaFin.Date)
            throw new FechasInvalidasException("La fecha de inicio debe ser anterior a la fecha de fin.");
    }

    private void ValidarFechaNoEnPasado(DateTime fechaInicio)
    {
        if (fechaInicio.Date < DateTime.UtcNow.Date)
            throw new FechasInvalidasException("No se pueden crear reservas con fecha de inicio en el pasado.");
    }

    private void ValidarHuespedes(int cantidad)
    {
        if (cantidad <= 0)
            throw new ValidationException("La cantidad de huéspedes debe ser mayor a 0.");
    }

}
