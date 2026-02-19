using CT.Application.DTOs;
using CT.Application.UseCases.Pagos;
using CT.Application.Validations;
using CT.Domain.Entities;
using CT.Domain.Enums;
using CT.Domain.Exceptions;
using CT.Domain.Interfaces;
using FluentAssertions;
using Moq;

namespace CT.Tests.UseCases.Pagos;

public class PagoRegistrarUseCaseTests
{
    private readonly Mock<IRepositorioPago> _repoPago = new();
    private readonly Mock<IRepositorioReserva> _repoReserva = new();
    private readonly Mock<IUnitOfWork> _unitOfWork = new();
    private readonly PagoValidator _validador = new();
    private readonly PagoRegistrarUseCase _useCase;

    public PagoRegistrarUseCaseTests()
    {
        _useCase = new PagoRegistrarUseCase(
            _repoPago.Object, _repoReserva.Object, _unitOfWork.Object, _validador);
    }

    private static Reserva CrearReservaConfirmada() => new()
    {
        ReservaId = 1,
        Estado = EstadoReserva.Confirmada,
        MontoTotal = 150000m,
        SaldoPendiente = 150000m
    };

    private static RegistrarPagoDto CrearDtoValido() => new()
    {
        ReservaId = 1,
        Monto = 50000m,
        TipoPago = TipoPago.Senia,
        MedioPago = MedioPago.Transferencia
    };

    [Fact]
    public async Task EjecutarAsync_PagoValido_RegistraYActualizaSaldo()
    {
        var reserva = CrearReservaConfirmada();
        var dto = CrearDtoValido();
        _repoReserva.Setup(r => r.ObtenerPorIdAsync(1)).ReturnsAsync(reserva);
        _repoPago.Setup(r => r.ObtenerTotalPagadoAsync(1)).ReturnsAsync(50000m);

        await _useCase.EjecutarAsync(dto);

        // Verifica que se registró el pago
        _repoPago.Verify(r => r.AgregarAsync(It.Is<Pago>(p =>
            p.Monto == 50000m &&
            p.TipoPago == TipoPago.Senia &&
            p.EstadoPago == EstadoPago.Confirmado
        )), Times.Once);

        // Verifica que se actualizó el saldo
        reserva.SaldoPendiente.Should().Be(100000m); // 150000 - 50000

        // Verifica transacción
        _unitOfWork.Verify(u => u.BeginTransactionAsync(), Times.Once);
        _unitOfWork.Verify(u => u.CommitAsync(), Times.Once);
    }

    [Fact]
    public async Task EjecutarAsync_ReservaNoExiste_LanzaEntityNotFound()
    {
        var dto = CrearDtoValido();
        _repoReserva.Setup(r => r.ObtenerPorIdAsync(1)).ReturnsAsync((Reserva?)null);

        var act = () => _useCase.EjecutarAsync(dto);

        await act.Should().ThrowAsync<EntityNotFoundException>();
    }

    [Fact]
    public async Task EjecutarAsync_ReservaCancelada_LanzaReservaCancelada()
    {
        var dto = CrearDtoValido();
        var reserva = new Reserva { ReservaId = 1, Estado = EstadoReserva.Cancelada };
        _repoReserva.Setup(r => r.ObtenerPorIdAsync(1)).ReturnsAsync(reserva);

        var act = () => _useCase.EjecutarAsync(dto);

        await act.Should().ThrowAsync<ReservaCanceladaException>();
    }

    [Fact]
    public async Task EjecutarAsync_MontoNegativo_LanzaValidationException()
    {
        var dto = CrearDtoValido();
        dto.Monto = -100;

        var act = () => _useCase.EjecutarAsync(dto);

        await act.Should().ThrowAsync<ValidationException>();
    }

    [Fact]
    public async Task EjecutarAsync_MontoCero_LanzaValidationException()
    {
        var dto = CrearDtoValido();
        dto.Monto = 0;

        var act = () => _useCase.EjecutarAsync(dto);

        await act.Should().ThrowAsync<ValidationException>();
    }

    [Fact]
    public async Task EjecutarAsync_ErrorEnPersistencia_HaceRollback()
    {
        var reserva = CrearReservaConfirmada();
        var dto = CrearDtoValido();
        _repoReserva.Setup(r => r.ObtenerPorIdAsync(1)).ReturnsAsync(reserva);
        _repoPago.Setup(r => r.AgregarAsync(It.IsAny<Pago>())).ThrowsAsync(new Exception("DB Error"));

        var act = () => _useCase.EjecutarAsync(dto);

        await act.Should().ThrowAsync<Exception>();
        _unitOfWork.Verify(u => u.RollbackAsync(), Times.Once);
        _unitOfWork.Verify(u => u.CommitAsync(), Times.Never);
    }
}
