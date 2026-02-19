using CT.Domain.Enums;
using CT.Domain.Exceptions;
using CT.Domain.Interfaces;

namespace CT.Application.UseCases.Reservas;

public class ReservaConfirmarUseCase
{
    private readonly IRepositorioReserva _repositorio;

    public ReservaConfirmarUseCase(IRepositorioReserva repositorio)
    {
        _repositorio = repositorio;
    }

    public async Task EjecutarAsync(int reservaId)
    {
        // 1. Obtener reserva
        var reserva = await _repositorio.ObtenerPorIdAsync(reservaId);
        if (reserva == null)
            throw new EntityNotFoundException("Reserva no encontrada.");

        // 2. Validar estado actual — solo se permite Pendiente → Confirmada
        if (reserva.Estado == EstadoReserva.Cancelada)
            throw new ReservaCanceladaException("No se puede confirmar una reserva cancelada.");

        if (reserva.Estado == EstadoReserva.Confirmada)
            throw new DomainInvalidOperationException("La reserva ya está confirmada.");

        if (reserva.Estado == EstadoReserva.Finalizada)
            throw new DomainInvalidOperationException("No se puede confirmar una reserva finalizada.");

        // 3. Cambiar estado a Confirmada
        reserva.Estado = EstadoReserva.Confirmada;

        // 4. Persistir
        await _repositorio.ModificarAsync(reserva);
    }
}
