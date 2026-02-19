using CT.Application.DTOs;
using CT.Domain.Exceptions;
using CT.Domain.Interfaces;

namespace CT.Application.UseCases.Reservas;

public class ReservaObtenerDetalleUseCase
{
    private readonly IRepositorioReserva _repositorio;
    private readonly IRepositorioPago _repositorioPago;

    public ReservaObtenerDetalleUseCase(IRepositorioReserva repositorio, IRepositorioPago repositorioPago)
    {
        _repositorio = repositorio;
        _repositorioPago = repositorioPago;
    }

    public async Task<ReservaDetalleDto> EjecutarAsync(int reservaId)
    {
        // 1. Obtener reserva con Departamento y Cliente (Include)
        var reserva = await _repositorio.ObtenerDetalleAsync(reservaId);

        if (reserva == null)
            throw new EntityNotFoundException("Reserva no encontrada.");

        // 2. Obtener pagos de la reserva
        var pagos = await _repositorioPago.ListarPorReservaAsync(reservaId);

        // 3. Calcular datos derivados
        var totalPagado = pagos
            .Where(p => p.EstadoPago == Domain.Enums.EstadoPago.Confirmado)
            .Sum(p => p.Monto);

        // 4. Mapear a DTO
        return new ReservaDetalleDto
        {
            ReservaId = reserva.ReservaId,
            FechaInicio = reserva.FechaInicio,
            FechaFin = reserva.FechaFin,
            CantidadHuespedes = reserva.CantidadHuespedes,
            Estado = reserva.Estado,
            PrecioPorNoche = reserva.PrecioPorNoche,
            MontoTotal = reserva.MontoTotal,
            MontoSenia = reserva.MontoSenia,
            OrigenReserva = reserva.OrigenReserva,
            Observaciones = reserva.Observaciones,
            FechaCreacion = reserva.FechaCreacion,

            // Calculados
            CantidadNoches = (reserva.FechaFin - reserva.FechaInicio).Days,
            TotalPagado = totalPagado,
            SaldoPendiente = reserva.MontoTotal - totalPagado,

            // Departamento
            DepartamentoId = reserva.DepartamentoId,
            DepartamentoNombre = reserva.Departamento?.Nombre ?? string.Empty,

            // Cliente
            ClienteId = reserva.ClienteId,
            ClienteNombreCompleto = $"{reserva.Cliente?.Nombre} {reserva.Cliente?.Apellido}".Trim(),
            ClienteTelefono = reserva.Cliente?.Telefono,

            // Pagos
            Pagos = pagos.Select(p => new PagoResumenDto
            {
                PagoId = p.PagoId,
                FechaPago = p.FechaPago,
                Monto = p.Monto,
                TipoPago = p.TipoPago,
                MedioPago = p.MedioPago,
                EstadoPago = p.EstadoPago,
                Observaciones = p.Observaciones
            }).ToList()
        };
    }
}
