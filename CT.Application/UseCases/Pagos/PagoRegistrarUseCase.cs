using CT.Application.DTOs;
using CT.Application.Validations;
using CT.Domain.Entities;
using CT.Domain.Enums;
using CT.Domain.Exceptions;
using CT.Domain.Interfaces;

namespace CT.Application.UseCases.Pagos;

public class PagoRegistrarUseCase
{
    private readonly IRepositorioPago _repositorio;
    private readonly IRepositorioReserva _repositorioReserva;
    private readonly IUnitOfWork _unitOfWork;
    private readonly PagoValidator _validador;

    public PagoRegistrarUseCase(
        IRepositorioPago repositorio,
        IRepositorioReserva repositorioReserva,
        IUnitOfWork unitOfWork,
        PagoValidator validador)
    {
        _repositorio = repositorio;
        _repositorioReserva = repositorioReserva;
        _unitOfWork = unitOfWork;
        _validador = validador;
    }

    public async Task EjecutarAsync(RegistrarPagoDto dto)
    {
        // 1. Validaciones de formato
        _validador.Validar(dto);

        // 2. Validar que la reserva existe
        var reserva = await _repositorioReserva.ObtenerPorIdAsync(dto.ReservaId);
        if (reserva == null)
            throw new EntityNotFoundException("Reserva no encontrada.");

        // 3. Validar que la reserva no está cancelada
        if (reserva.Estado == EstadoReserva.Cancelada)
            throw new ReservaCanceladaException("No se puede registrar un pago para una reserva cancelada.");

        // 4. Crear entidad Pago
        var pago = new Pago
        {
            ReservaId = dto.ReservaId,
            FechaPago = DateTime.UtcNow,
            Monto = dto.Monto,
            TipoPago = dto.TipoPago,
            MedioPago = dto.MedioPago,
            EstadoPago = EstadoPago.Confirmado,
            Observaciones = dto.Observaciones?.Trim()
        };

        // 5. Persistir pago + actualizar saldo en una transacción atómica
        await _unitOfWork.BeginTransactionAsync();
        try
        {
            await _repositorio.AgregarAsync(pago);

            var totalPagado = await _repositorio.ObtenerTotalPagadoAsync(dto.ReservaId);
            reserva.SaldoPendiente = reserva.MontoTotal - totalPagado;
            await _repositorioReserva.ModificarAsync(reserva);

            await _unitOfWork.CommitAsync();
        }
        catch
        {
            await _unitOfWork.RollbackAsync();
            throw;
        }
    }
}
