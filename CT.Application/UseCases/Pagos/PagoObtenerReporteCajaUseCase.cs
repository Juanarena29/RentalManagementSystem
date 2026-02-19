using CT.Application.DTOs;
using CT.Domain.Interfaces;

namespace CT.Application.UseCases.Pagos;

public class PagoObtenerReporteCajaUseCase
{
    private readonly IRepositorioPago _repositorio;

    public PagoObtenerReporteCajaUseCase(IRepositorioPago repositorio)
    {
        _repositorio = repositorio;
    }

    public async Task<List<PagoDto>> EjecutarAsync(DateTime desde, DateTime hasta)
    {
        var pagos = await _repositorio.ListarPorRangoFechasAsync(desde, hasta);

        return pagos.MapToDto();
    }
}
