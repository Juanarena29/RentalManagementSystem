using CT.Domain.Enums;
using CT.Domain.Exceptions;
using CT.Domain.Interfaces;

namespace CT.Application.UseCases.Reservas;

public class ReservaCancelarUseCase
{
    private readonly IRepositorioReserva _repositorio;

    public ReservaCancelarUseCase(IRepositorioReserva repositorio)
    {
        _repositorio = repositorio;
    }

    public async Task EjecutarAsync(int reservaId, string? motivoCancelacion = null)
    {
        // 1. Obtener reserva
        var reserva = await _repositorio.ObtenerPorIdAsync(reservaId);
        if (reserva == null)
            throw new EntityNotFoundException("Reserva no encontrada.");

        // 2. Validar que no está ya cancelada
        if (reserva.Estado == EstadoReserva.Cancelada)
            throw new DomainInvalidOperationException("La reserva ya está cancelada.");

        // 3. Validar que no está finalizada
        if (reserva.Estado == EstadoReserva.Finalizada)
            throw new DomainInvalidOperationException("No se puede cancelar una reserva finalizada.");

        // 4. Cambiar estado
        reserva.Estado = EstadoReserva.Cancelada;

        if (!string.IsNullOrWhiteSpace(motivoCancelacion))
        {
            reserva.Observaciones = string.IsNullOrWhiteSpace(reserva.Observaciones)
                ? $"CANCELADA: {motivoCancelacion}"
                : $"{reserva.Observaciones}\n\nCANCELADA: {motivoCancelacion}";
        }

        // 5. Persistir
        await _repositorio.ModificarAsync(reserva);
    }
}
