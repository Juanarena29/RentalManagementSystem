using CT.Application.DTOs;
using CT.Domain.Interfaces;

namespace CT.Application.UseCases.Pagos;

public class PagoObtenerReservasConSaldoPendienteUseCase
{
    private readonly IRepositorioReserva _repositorioReserva;

    public PagoObtenerReservasConSaldoPendienteUseCase(
        IRepositorioReserva repositorioReserva)
    {
        _repositorioReserva = repositorioReserva;
    }

    public async Task<List<ReservaConSaldoDto>> EjecutarAsync()
    {
        // 1. Obtener reservas con saldo pendiente (ya filtradas por SaldoPendiente > 0)
        var reservas = await _repositorioReserva.ObtenerReservasConSaldoPendienteAsync();

        // 2. Transformar a DTO — SaldoPendiente y TotalPagado se calculan
        //    desde los campos ya persistidos en la entidad, sin queries adicionales
        return reservas.Select(r => new ReservaConSaldoDto
        {
            ReservaId = r.ReservaId,
            DepartamentoNombre = r.Departamento?.Nombre ?? string.Empty,
            ClienteNombreCompleto = $"{r.Cliente?.Nombre} {r.Cliente?.Apellido}".Trim(),
            ClienteTelefono = r.Cliente?.Telefono,
            FechaInicio = r.FechaInicio,
            FechaFin = r.FechaFin,
            Estado = r.Estado,
            MontoTotal = r.MontoTotal,
            TotalPagado = r.MontoTotal - r.SaldoPendiente,
            SaldoPendiente = r.SaldoPendiente
        }).ToList();
    }
}
