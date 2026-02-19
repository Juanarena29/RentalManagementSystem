using CT.Application.DTOs;
using CT.Application.Validations;
using CT.Domain.Enums;
using CT.Domain.Exceptions;
using CT.Domain.Interfaces;

namespace CT.Application.UseCases.Reservas;

public class ReservaModificarUseCase
{
    private readonly IRepositorioReserva _repositorio;
    private readonly IRepositorioDepartamento _repositorioDepartamento;
    private readonly IRepositorioPago _repositorioPago;
    private readonly IRepositorioCliente _repositorioCliente;
    private readonly ReservaValidator _validador;

    public ReservaModificarUseCase(
        IRepositorioReserva repositorio,
        IRepositorioDepartamento repositorioDepartamento,
        IRepositorioPago repositorioPago,
        IRepositorioCliente repositorioCliente,
        ReservaValidator validador)
    {
        _repositorio = repositorio;
        _repositorioDepartamento = repositorioDepartamento;
        _repositorioPago = repositorioPago;
        _repositorioCliente = repositorioCliente;
        _validador = validador;
    }

    public async Task EjecutarAsync(ModificarReservaDto dto)
    {
        // 1. Validaciones de formato
        _validador.Validar(dto);

        // 2. Obtener reserva existente
        var reserva = await _repositorio.ObtenerPorIdAsync(dto.ReservaId);
        if (reserva == null)
            throw new EntityNotFoundException("Reserva no encontrada.");

        // 3. Validar que no está cancelada
        if (reserva.Estado == EstadoReserva.Cancelada)
            throw new ReservaCanceladaException("No se puede modificar una reserva cancelada.");

        // 4. Validar departamento existe, está activo y capacidad
        var departamento = await _repositorioDepartamento.ObtenerPorIdAsync(dto.DepartamentoId);
        if (departamento == null)
            throw new EntityNotFoundException("Departamento no encontrado.");

        if (departamento.Estado != EstadoDepartamento.Activo)
            throw new DepartamentoNoDisponibleException("El departamento no está disponible.");

        if (dto.CantidadHuespedes > departamento.CapacidadMaxima)
            throw new CapacidadExcedidaException(
                $"La cantidad de huéspedes ({dto.CantidadHuespedes}) supera la capacidad máxima del departamento ({departamento.CapacidadMaxima}).");

        // 5. Validar que no está finalizada
        if (reserva.Estado == EstadoReserva.Finalizada)
            throw new DomainInvalidOperationException("No se puede modificar una reserva finalizada.");

        // 6. Validar solapamiento (excluyendo la reserva actual) y que exista cliente
        if (await _repositorio.HaySolapamientoAsync(
            dto.DepartamentoId,
            dto.FechaInicio,
            dto.FechaFin,
            dto.ReservaId))
            throw new ReservaSuperpuestaException("Ya existe otra reserva confirmada en esas fechas.");

        if (await _repositorioCliente.ExisteAsync(dto.ClienteId) == false)
            throw new EntityNotFoundException("Cliente no encontrado.");

        // 7. Mapear campos editables (preserva Estado, MontoSenia, FechaCreacion)
        reserva.DepartamentoId = dto.DepartamentoId;
        reserva.ClienteId = dto.ClienteId;
        reserva.FechaInicio = dto.FechaInicio;
        reserva.FechaFin = dto.FechaFin;
        reserva.CantidadHuespedes = dto.CantidadHuespedes;
        reserva.OrigenReserva = dto.OrigenReserva;
        reserva.Observaciones = dto.Observaciones?.Trim();

        // 8. Recalcular montos con el precio actual del departamento
        reserva.PrecioPorNoche = departamento.PrecioPorNoche;
        int cantidadNoches = (dto.FechaFin.Date - dto.FechaInicio.Date).Days;
        reserva.MontoTotal = cantidadNoches * departamento.PrecioPorNoche;

        // 9. Recalcular SaldoPendiente considerando pagos existentes
        var totalPagado = await _repositorioPago.ObtenerTotalPagadoAsync(dto.ReservaId);
        reserva.SaldoPendiente = reserva.MontoTotal - totalPagado;

        // 10. Persistir cambios
        await _repositorio.ModificarAsync(reserva);
    }
}
