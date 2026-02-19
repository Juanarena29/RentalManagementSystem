using CT.Domain.Enums;
using CT.Domain.Exceptions;
using CT.Domain.Interfaces;

namespace CT.Application.UseCases.Reservas;

public class ReservaFinalizarUseCase
{
    private readonly IRepositorioReserva _repositorio;

    public ReservaFinalizarUseCase(IRepositorioReserva repositorio)
    {
        _repositorio = repositorio;
    }

    public async Task EjecutarAsync(int reservaId)
    {
        // 1. Obtener reserva
        var reserva = await _repositorio.ObtenerPorIdAsync(reservaId);
        if (reserva == null)
            throw new EntityNotFoundException("Reserva no encontrada.");

        // 2. Validar estado
        if (reserva.Estado == EstadoReserva.Cancelada)
            throw new ReservaCanceladaException("No se puede finalizar una reserva cancelada.");

        if (reserva.Estado == EstadoReserva.Finalizada)
            throw new DomainInvalidOperationException("La reserva ya está finalizada.");

        if (reserva.Estado == EstadoReserva.Pendiente)
            throw new DomainInvalidOperationException("No se puede finalizar una reserva pendiente. Debe confirmarla primero.");

        // 3. Validar que el saldo esté pagado
        if (reserva.SaldoPendiente > 0)
            throw new PagoInsuficienteException(
                $"No se puede finalizar la reserva. Saldo pendiente: {reserva.SaldoPendiente:N2}");

        // 4. Cambiar estado a Finalizada
        reserva.Estado = EstadoReserva.Finalizada;

        // 5. Persistir
        await _repositorio.ModificarAsync(reserva);
    }
}
