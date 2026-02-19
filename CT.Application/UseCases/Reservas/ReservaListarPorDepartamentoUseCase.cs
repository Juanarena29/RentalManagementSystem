using CT.Application.DTOs;
using CT.Domain.Interfaces;

namespace CT.Application.UseCases.Reservas;

public class ReservaListarPorDepartamentoUseCase
{
    private readonly IRepositorioReserva _repositorio;

    public ReservaListarPorDepartamentoUseCase(IRepositorioReserva repositorio)
    {
        _repositorio = repositorio;
    }

    public async Task<List<ReservaResumenDto>> EjecutarAsync(int departamentoId)
    {
        var reservas = await _repositorio.ListarPorDepartamentoAsync(departamentoId);

        return reservas.MapToResumenDto();
    }
}
