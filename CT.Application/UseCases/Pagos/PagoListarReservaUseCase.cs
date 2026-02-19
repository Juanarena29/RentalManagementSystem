using CT.Application.DTOs;
using CT.Domain.Interfaces;

namespace CT.Application.UseCases.Pagos;

public class PagoListarReservaUseCase
{
    private readonly IRepositorioPago _repositorio;

    public PagoListarReservaUseCase(IRepositorioPago repositorio)
    {
        _repositorio = repositorio;
    }

    public async Task<List<PagoDto>> EjecutarAsync(int reservaId)
    {
        var pagos = await _repositorio.ListarPorReservaAsync(reservaId);

        return pagos.MapToDto();
    }
}
