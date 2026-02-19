using CT.Application.DTOs;
using CT.Application.Validations;
using CT.Domain.Entities;
using CT.Domain.Enums;
using CT.Domain.Exceptions;
using CT.Domain.Interfaces;

namespace CT.Application.UseCases.Reservas;

public class ReservaAltaUseCase
{
    private readonly IRepositorioReserva _repositorio;
    private readonly IRepositorioDepartamento _repositorioDepartamento;
    private readonly IRepositorioCliente _repositorioCliente;
    private readonly ReservaValidator _validador;

    public ReservaAltaUseCase(
        IRepositorioReserva repositorio,
        IRepositorioDepartamento repositorioDepartamento,
        IRepositorioCliente repositorioCliente,
        ReservaValidator validador)
    {
        _repositorio = repositorio;
        _repositorioDepartamento = repositorioDepartamento;
        _repositorioCliente = repositorioCliente;
        _validador = validador;
    }

    public async Task EjecutarAsync(CrearReservaDto dto)
    {
        // 1. Validaciones de formato
        _validador.Validar(dto);

        // 2. Validar que el departamento existe y está activo
        var departamento = await _repositorioDepartamento.ObtenerPorIdAsync(dto.DepartamentoId);
        if (departamento == null)
            throw new EntityNotFoundException("Departamento no encontrado.");

        if (departamento.Estado != EstadoDepartamento.Activo)
            throw new DepartamentoNoDisponibleException("El departamento no está disponible.");

        // 3. Validar que el cliente existe
        if (!await _repositorioCliente.ExisteAsync(dto.ClienteId))
            throw new EntityNotFoundException("Cliente no encontrado.");

        // 4. Validar capacidad
        if (dto.CantidadHuespedes > departamento.CapacidadMaxima)
            throw new CapacidadExcedidaException(
                $"La cantidad de huéspedes ({dto.CantidadHuespedes}) supera la capacidad máxima del departamento ({departamento.CapacidadMaxima}).");

        // 5. Validar que no hay solapamiento con otras reservas
        if (await _repositorio.HaySolapamientoAsync(dto.DepartamentoId, dto.FechaInicio, dto.FechaFin))
            throw new ReservaSuperpuestaException("Existe solapamiento de reservas en esas fechas.");

        // 6. Calcular montos
        int cantidadNoches = (dto.FechaFin.Date - dto.FechaInicio.Date).Days;
        decimal montoTotal = cantidadNoches * departamento.PrecioPorNoche;

        // 7. Crear entidad Reserva (pagos se registran aparte con PagoRegistrarUseCase)
        var reserva = new Reserva
        {
            DepartamentoId = dto.DepartamentoId,
            ClienteId = dto.ClienteId,
            FechaInicio = dto.FechaInicio.Date,
            FechaFin = dto.FechaFin.Date,
            CantidadHuespedes = dto.CantidadHuespedes,
            Estado = EstadoReserva.Pendiente,
            PrecioPorNoche = departamento.PrecioPorNoche,
            MontoTotal = montoTotal,
            MontoSenia = 0,
            SaldoPendiente = montoTotal,
            OrigenReserva = dto.OrigenReserva,
            Observaciones = dto.Observaciones?.Trim(),
            FechaCreacion = DateTime.UtcNow
        };

        // 8. Persistir reserva
        await _repositorio.AgregarAsync(reserva);
    }
}
