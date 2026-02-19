using CT.Application.DTOs;
using CT.Domain.Interfaces;

namespace CT.Application.UseCases.Reservas;

public class ObtenerCalendarioOcupacionUseCase
{
    private readonly IRepositorioReserva _repositorio;

    public ObtenerCalendarioOcupacionUseCase(IRepositorioReserva repositorio)
    {
        _repositorio = repositorio;
    }

    public async Task<List<CalendarioOcupacionDto>> EjecutarAsync(DateTime desde, DateTime hasta)
    {
        // 1. Obtener reservas en el rango (con relaciones cargadas)
        var reservas = await _repositorio.ListarPorRangoFechasAsync(desde, hasta);

        // 2. Transformar a DTO de calendario
        var resultado = new List<CalendarioOcupacionDto>();

        foreach (var reserva in reservas)
        {
            resultado.Add(new CalendarioOcupacionDto
            {
                ReservaId = reserva.ReservaId,
                DepartamentoId = reserva.DepartamentoId,
                DepartamentoNombre = reserva.Departamento?.Nombre ?? string.Empty,
                FechaInicio = reserva.FechaInicio,
                FechaFin = reserva.FechaFin,
                ClienteNombreCompleto = $"{reserva.Cliente?.Nombre} {reserva.Cliente?.Apellido}".Trim(),
                ClienteTelefono = reserva.Cliente?.Telefono,
                Estado = reserva.Estado,
                CantidadHuespedes = reserva.CantidadHuespedes
            });
        }

        return resultado;
    }
}
