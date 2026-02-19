using CT.Application.UseCases.Reservas;
using CT.Domain.Entities;
using CT.Domain.Enums;
using CT.Domain.Exceptions;
using CT.Domain.Interfaces;
using FluentAssertions;
using Moq;

namespace CT.Tests.UseCases.Reservas;

public class ReservaCancelarUseCaseTests
{
    private readonly Mock<IRepositorioReserva> _repo = new();
    private readonly ReservaCancelarUseCase _useCase;

    public ReservaCancelarUseCaseTests()
    {
        _useCase = new ReservaCancelarUseCase(_repo.Object);
    }

    private static Reserva CrearReserva(EstadoReserva estado) => new()
    {
        ReservaId = 1,
        Estado = estado,
        Observaciones = "Obs original"
    };

    [Fact]
    public async Task EjecutarAsync_ReservaPendiente_CambiaACancelada()
    {
        var reserva = CrearReserva(EstadoReserva.Pendiente);
        _repo.Setup(r => r.ObtenerPorIdAsync(1)).ReturnsAsync(reserva);

        await _useCase.EjecutarAsync(1);

        reserva.Estado.Should().Be(EstadoReserva.Cancelada);
        _repo.Verify(r => r.ModificarAsync(reserva), Times.Once);
    }

    [Fact]
    public async Task EjecutarAsync_ReservaConfirmada_CambiaACancelada()
    {
        var reserva = CrearReserva(EstadoReserva.Confirmada);
        _repo.Setup(r => r.ObtenerPorIdAsync(1)).ReturnsAsync(reserva);

        await _useCase.EjecutarAsync(1);

        reserva.Estado.Should().Be(EstadoReserva.Cancelada);
    }

    [Fact]
    public async Task EjecutarAsync_YaCancelada_LanzaDomainInvalidOperation()
    {
        var reserva = CrearReserva(EstadoReserva.Cancelada);
        _repo.Setup(r => r.ObtenerPorIdAsync(1)).ReturnsAsync(reserva);

        var act = () => _useCase.EjecutarAsync(1);

        await act.Should().ThrowAsync<DomainInvalidOperationException>();
    }

    [Fact]
    public async Task EjecutarAsync_Finalizada_LanzaDomainInvalidOperation()
    {
        var reserva = CrearReserva(EstadoReserva.Finalizada);
        _repo.Setup(r => r.ObtenerPorIdAsync(1)).ReturnsAsync(reserva);

        var act = () => _useCase.EjecutarAsync(1);

        await act.Should().ThrowAsync<DomainInvalidOperationException>();
    }

    [Fact]
    public async Task EjecutarAsync_NoExiste_LanzaEntityNotFound()
    {
        _repo.Setup(r => r.ObtenerPorIdAsync(99)).ReturnsAsync((Reserva?)null);

        var act = () => _useCase.EjecutarAsync(99);

        await act.Should().ThrowAsync<EntityNotFoundException>();
    }

    [Fact]
    public async Task EjecutarAsync_ConMotivo_ConcatenaEnObservaciones()
    {
        var reserva = CrearReserva(EstadoReserva.Pendiente);
        _repo.Setup(r => r.ObtenerPorIdAsync(1)).ReturnsAsync(reserva);

        await _useCase.EjecutarAsync(1, "Cliente no puede venir");

        reserva.Observaciones.Should().Contain("CANCELADA: Cliente no puede venir");
        reserva.Observaciones.Should().Contain("Obs original");
    }

    [Fact]
    public async Task EjecutarAsync_SinMotivo_NoModificaObservaciones()
    {
        var reserva = CrearReserva(EstadoReserva.Pendiente);
        _repo.Setup(r => r.ObtenerPorIdAsync(1)).ReturnsAsync(reserva);

        await _useCase.EjecutarAsync(1);

        reserva.Observaciones.Should().Be("Obs original");
    }
}

public class ReservaConfirmarUseCaseTests
{
    private readonly Mock<IRepositorioReserva> _repo = new();
    private readonly ReservaConfirmarUseCase _useCase;

    public ReservaConfirmarUseCaseTests()
    {
        _useCase = new ReservaConfirmarUseCase(_repo.Object);
    }

    [Fact]
    public async Task EjecutarAsync_Pendiente_CambiaAConfirmada()
    {
        var reserva = new Reserva { ReservaId = 1, Estado = EstadoReserva.Pendiente };
        _repo.Setup(r => r.ObtenerPorIdAsync(1)).ReturnsAsync(reserva);

        await _useCase.EjecutarAsync(1);

        reserva.Estado.Should().Be(EstadoReserva.Confirmada);
        _repo.Verify(r => r.ModificarAsync(reserva), Times.Once);
    }

    [Fact]
    public async Task EjecutarAsync_YaConfirmada_LanzaDomainInvalidOperation()
    {
        var reserva = new Reserva { ReservaId = 1, Estado = EstadoReserva.Confirmada };
        _repo.Setup(r => r.ObtenerPorIdAsync(1)).ReturnsAsync(reserva);

        var act = () => _useCase.EjecutarAsync(1);

        await act.Should().ThrowAsync<DomainInvalidOperationException>();
    }

    [Fact]
    public async Task EjecutarAsync_Cancelada_LanzaReservaCancelada()
    {
        var reserva = new Reserva { ReservaId = 1, Estado = EstadoReserva.Cancelada };
        _repo.Setup(r => r.ObtenerPorIdAsync(1)).ReturnsAsync(reserva);

        var act = () => _useCase.EjecutarAsync(1);

        await act.Should().ThrowAsync<ReservaCanceladaException>();
    }

    [Fact]
    public async Task EjecutarAsync_Finalizada_LanzaDomainInvalidOperation()
    {
        var reserva = new Reserva { ReservaId = 1, Estado = EstadoReserva.Finalizada };
        _repo.Setup(r => r.ObtenerPorIdAsync(1)).ReturnsAsync(reserva);

        var act = () => _useCase.EjecutarAsync(1);

        await act.Should().ThrowAsync<DomainInvalidOperationException>();
    }
}

public class ReservaFinalizarUseCaseTests
{
    private readonly Mock<IRepositorioReserva> _repo = new();
    private readonly ReservaFinalizarUseCase _useCase;

    public ReservaFinalizarUseCaseTests()
    {
        _useCase = new ReservaFinalizarUseCase(_repo.Object);
    }

    [Fact]
    public async Task EjecutarAsync_ConfirmadaSinSaldo_Finaliza()
    {
        var reserva = new Reserva
        {
            ReservaId = 1,
            Estado = EstadoReserva.Confirmada,
            SaldoPendiente = 0
        };
        _repo.Setup(r => r.ObtenerPorIdAsync(1)).ReturnsAsync(reserva);

        await _useCase.EjecutarAsync(1);

        reserva.Estado.Should().Be(EstadoReserva.Finalizada);
    }

    [Fact]
    public async Task EjecutarAsync_ConSaldoPendiente_LanzaPagoInsuficiente()
    {
        var reserva = new Reserva
        {
            ReservaId = 1,
            Estado = EstadoReserva.Confirmada,
            SaldoPendiente = 5000m
        };
        _repo.Setup(r => r.ObtenerPorIdAsync(1)).ReturnsAsync(reserva);

        var act = () => _useCase.EjecutarAsync(1);

        await act.Should().ThrowAsync<PagoInsuficienteException>();
    }

    [Fact]
    public async Task EjecutarAsync_Pendiente_LanzaDomainInvalidOperation()
    {
        var reserva = new Reserva { ReservaId = 1, Estado = EstadoReserva.Pendiente };
        _repo.Setup(r => r.ObtenerPorIdAsync(1)).ReturnsAsync(reserva);

        var act = () => _useCase.EjecutarAsync(1);

        await act.Should().ThrowAsync<DomainInvalidOperationException>();
    }

    [Fact]
    public async Task EjecutarAsync_Cancelada_LanzaReservaCancelada()
    {
        var reserva = new Reserva { ReservaId = 1, Estado = EstadoReserva.Cancelada };
        _repo.Setup(r => r.ObtenerPorIdAsync(1)).ReturnsAsync(reserva);

        var act = () => _useCase.EjecutarAsync(1);

        await act.Should().ThrowAsync<ReservaCanceladaException>();
    }

    [Fact]
    public async Task EjecutarAsync_YaFinalizada_LanzaDomainInvalidOperation()
    {
        var reserva = new Reserva { ReservaId = 1, Estado = EstadoReserva.Finalizada };
        _repo.Setup(r => r.ObtenerPorIdAsync(1)).ReturnsAsync(reserva);

        var act = () => _useCase.EjecutarAsync(1);

        await act.Should().ThrowAsync<DomainInvalidOperationException>();
    }
}
