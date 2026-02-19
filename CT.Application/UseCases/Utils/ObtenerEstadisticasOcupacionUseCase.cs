using CT.Application.DTOs;
using CT.Domain.Enums;
using CT.Domain.Interfaces;

namespace CT.Application.UseCases.Utils;

public class ObtenerEstadisticasOcupacionUseCase
{
    private readonly IRepositorioReserva _repositorioReserva;
    private readonly IRepositorioPago _repositorioPago;
    private readonly IRepositorioDepartamento _repositorioDepartamento;

    public ObtenerEstadisticasOcupacionUseCase(
        IRepositorioReserva repositorioReserva,
        IRepositorioPago repositorioPago,
        IRepositorioDepartamento repositorioDepartamento)
    {
        _repositorioReserva = repositorioReserva;
        _repositorioPago = repositorioPago;
        _repositorioDepartamento = repositorioDepartamento;
    }

    public async Task<EstadisticasOcupacionDto> EjecutarAsync(DateTime desde, DateTime hasta)
    {
        // 1. Obtener todas las reservas del período
        var reservas = await _repositorioReserva.ListarPorRangoFechasAsync(desde, hasta);
        var reservasConfirmadas = reservas.Where(r =>
            r.Estado == EstadoReserva.Confirmada ||
            r.Estado == EstadoReserva.Finalizada).ToList();

        // 2. Obtener todos los departamentos
        var departamentos = await _repositorioDepartamento.ListarAsync();
        var cantidadDepartamentos = departamentos.Count;

        // 3. Calcular noches totales posibles y ocupadas
        int diasPeriodo = (hasta.Date - desde.Date).Days;
        int nochesTotalesPosibles = cantidadDepartamentos * diasPeriodo;

        int nochesOcupadas = 0;
        foreach (var reserva in reservasConfirmadas)
        {
            var inicioCalculo = reserva.FechaInicio < desde ? desde : reserva.FechaInicio;
            var finCalculo = reserva.FechaFin > hasta ? hasta : reserva.FechaFin;
            nochesOcupadas += (finCalculo.Date - inicioCalculo.Date).Days;
        }

        decimal tasaOcupacion = nochesTotalesPosibles > 0
            ? (decimal)nochesOcupadas / nochesTotalesPosibles * 100
            : 0;

        // 4. Calcular ingresos
        var ingresosTotales = await _repositorioPago.ObtenerIngresosPorPeriodoAsync(desde, hasta);

        // 5. Estadísticas por estado
        var reservasPorEstado = reservas
            .GroupBy(r => r.Estado)
            .ToDictionary(g => g.Key, g => g.Count());

        // 6. Calcular ingreso promedio por reserva
        var cantidadReservasFinalizadas = reservasPorEstado.GetValueOrDefault(EstadoReserva.Finalizada, 0);
        decimal ingresoPromedioPorReserva = cantidadReservasFinalizadas > 0
            ? ingresosTotales / cantidadReservasFinalizadas
            : 0;

        return new EstadisticasOcupacionDto
        {
            FechaDesde = desde,
            FechaHasta = hasta,
            TasaOcupacion = Math.Round(tasaOcupacion, 2),
            NochesOcupadas = nochesOcupadas,
            NochesTotalesPosibles = nochesTotalesPosibles,
            TotalReservas = reservas.Count,
            ReservasPendientes = reservasPorEstado.GetValueOrDefault(EstadoReserva.Pendiente, 0),
            ReservasConfirmadas = reservasPorEstado.GetValueOrDefault(EstadoReserva.Confirmada, 0),
            ReservasFinalizadas = reservasPorEstado.GetValueOrDefault(EstadoReserva.Finalizada, 0),
            ReservasCanceladas = reservasPorEstado.GetValueOrDefault(EstadoReserva.Cancelada, 0),
            IngresosTotales = ingresosTotales,
            IngresoPromedioPorReserva = Math.Round(ingresoPromedioPorReserva, 2)
        };
    }
}
