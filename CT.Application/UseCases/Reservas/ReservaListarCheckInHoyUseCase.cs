using CT.Application.DTOs;
using CT.Domain.Interfaces;

namespace CT.Application.UseCases.Reservas;

public class ReservaListarCheckInHoyUseCase
{
    private readonly IRepositorioReserva _repositorio;

    public ReservaListarCheckInHoyUseCase(IRepositorioReserva repositorio)
    {
        _repositorio = repositorio;
    }

    public async Task<List<ReservaResumenDto>> EjecutarAsync()
    {
        var reservas = await _repositorio.ListarProximosCheckInsAsync(DateTime.UtcNow.Date);

        return reservas.MapToResumenDto();
    }
}
