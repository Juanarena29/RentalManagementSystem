using CT.Application.DTOs;
using CT.Domain.Interfaces;

namespace CT.Application.UseCases.Reservas;

public class ReservaListarCheckOutUseCase
{
    private readonly IRepositorioReserva _repositorio;

    public ReservaListarCheckOutUseCase(IRepositorioReserva repositorio)
    {
        _repositorio = repositorio;
    }

    public async Task<List<ReservaResumenDto>> EjecutarAsync(DateTime? fecha = null)
    {
        var fechaCheckOut = fecha ?? DateTime.UtcNow.Date;
        var reservas = await _repositorio.ListarProximosCheckOutsAsync(fechaCheckOut);

        return reservas.MapToResumenDto();
    }
}
