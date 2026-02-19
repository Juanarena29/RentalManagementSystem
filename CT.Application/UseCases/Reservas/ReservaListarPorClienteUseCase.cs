using CT.Application.DTOs;
using CT.Domain.Interfaces;

namespace CT.Application.UseCases.Reservas;

/// <summary>
/// Lista todas las reservas de un cliente específico, mapeadas a ReservaResumenDto.
/// Útil para el historial de reservas en la ficha de cliente.
/// </summary>
public class ReservaListarPorClienteUseCase
{
    private readonly IRepositorioReserva _repositorio;

    public ReservaListarPorClienteUseCase(IRepositorioReserva repositorio)
    {
        _repositorio = repositorio;
    }

    public async Task<List<ReservaResumenDto>> EjecutarAsync(int clienteId)
    {
        var reservas = await _repositorio.ListarPorClienteAsync(clienteId);
        return reservas.MapToResumenDto();
    }
}
