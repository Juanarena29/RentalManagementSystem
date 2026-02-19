using CT.Application.DTOs;
using CT.Application.UseCases.Reservas;
using CT.Application.Validations;
using CT.Domain.Entities;
using CT.Domain.Enums;
using CT.Domain.Exceptions;
using CT.Domain.Interfaces;
using FluentAssertions;
using Moq;

namespace CT.Tests.UseCases.Reservas;

public class ReservaAltaUseCaseTests
{
    private readonly Mock<IRepositorioReserva> _repoReserva = new();
    private readonly Mock<IRepositorioDepartamento> _repoDpto = new();
    private readonly Mock<IRepositorioCliente> _repoCliente = new();
    private readonly ReservaValidator _validador = new();
    private readonly ReservaAltaUseCase _useCase;

    private static readonly Departamento DeptoActivo = new()
    {
        DepartamentoId = 1,
        Nombre = "Depto 1",
        CapacidadMaxima = 4,
        PrecioPorNoche = 50000m,
        Estado = EstadoDepartamento.Activo
    };

    public ReservaAltaUseCaseTests()
    {
        _useCase = new ReservaAltaUseCase(
            _repoReserva.Object, _repoDpto.Object, _repoCliente.Object, _validador);
    }

    private CrearReservaDto CrearDtoValido() => new()
    {
        DepartamentoId = 1,
        ClienteId = 1,
        FechaInicio = DateTime.UtcNow.Date.AddDays(1),
        FechaFin = DateTime.UtcNow.Date.AddDays(4),
        CantidadHuespedes = 2,
        OrigenReserva = OrigenReserva.WhatsApp
    };

    [Fact]
    public async Task EjecutarAsync_DatosValidos_CreaReserva()
    {
        // Arrange
        var dto = CrearDtoValido();
        _repoDpto.Setup(r => r.ObtenerPorIdAsync(1)).ReturnsAsync(DeptoActivo);
        _repoCliente.Setup(r => r.ExisteAsync(1)).ReturnsAsync(true);
        _repoReserva.Setup(r => r.HaySolapamientoAsync(1, dto.FechaInicio, dto.FechaFin, null)).ReturnsAsync(false);

        // Act
        await _useCase.EjecutarAsync(dto);

        // Assert
        _repoReserva.Verify(r => r.AgregarAsync(It.Is<Reserva>(res =>
            res.Estado == EstadoReserva.Pendiente &&
            res.PrecioPorNoche == 50000m &&
            res.MontoTotal == 3 * 50000m &&
            res.SaldoPendiente == 3 * 50000m &&
            res.MontoSenia == 0
        )), Times.Once);
    }

    [Fact]
    public async Task EjecutarAsync_DepartamentoNoExiste_LanzaEntityNotFound()
    {
        var dto = CrearDtoValido();
        _repoDpto.Setup(r => r.ObtenerPorIdAsync(1)).ReturnsAsync((Departamento?)null);

        var act = () => _useCase.EjecutarAsync(dto);

        await act.Should().ThrowAsync<EntityNotFoundException>();
    }

    [Fact]
    public async Task EjecutarAsync_DepartamentoInactivo_LanzaDepartamentoNoDisponible()
    {
        var dto = CrearDtoValido();
        var dptoInactivo = new Departamento
        {
            DepartamentoId = 1,
            Estado = EstadoDepartamento.Inactivo,
            CapacidadMaxima = 4
        };
        _repoDpto.Setup(r => r.ObtenerPorIdAsync(1)).ReturnsAsync(dptoInactivo);

        var act = () => _useCase.EjecutarAsync(dto);

        await act.Should().ThrowAsync<DepartamentoNoDisponibleException>();
    }

    [Fact]
    public async Task EjecutarAsync_ClienteNoExiste_LanzaEntityNotFound()
    {
        var dto = CrearDtoValido();
        _repoDpto.Setup(r => r.ObtenerPorIdAsync(1)).ReturnsAsync(DeptoActivo);
        _repoCliente.Setup(r => r.ExisteAsync(1)).ReturnsAsync(false);

        var act = () => _useCase.EjecutarAsync(dto);

        await act.Should().ThrowAsync<EntityNotFoundException>();
    }

    [Fact]
    public async Task EjecutarAsync_CapacidadExcedida_LanzaCapacidadExcedida()
    {
        var dto = CrearDtoValido();
        dto.CantidadHuespedes = 10; // Máximo es 4
        _repoDpto.Setup(r => r.ObtenerPorIdAsync(1)).ReturnsAsync(DeptoActivo);
        _repoCliente.Setup(r => r.ExisteAsync(1)).ReturnsAsync(true);

        var act = () => _useCase.EjecutarAsync(dto);

        await act.Should().ThrowAsync<CapacidadExcedidaException>();
    }

    [Fact]
    public async Task EjecutarAsync_Solapamiento_LanzaReservaSuperpuesta()
    {
        var dto = CrearDtoValido();
        _repoDpto.Setup(r => r.ObtenerPorIdAsync(1)).ReturnsAsync(DeptoActivo);
        _repoCliente.Setup(r => r.ExisteAsync(1)).ReturnsAsync(true);
        _repoReserva.Setup(r => r.HaySolapamientoAsync(1, dto.FechaInicio, dto.FechaFin, null)).ReturnsAsync(true);

        var act = () => _useCase.EjecutarAsync(dto);

        await act.Should().ThrowAsync<ReservaSuperpuestaException>();
    }

    [Fact]
    public async Task EjecutarAsync_FechaInicioEnPasado_LanzaFechasInvalidas()
    {
        var dto = CrearDtoValido();
        dto.FechaInicio = DateTime.UtcNow.Date.AddDays(-1);
        dto.FechaFin = DateTime.UtcNow.Date.AddDays(2);

        var act = () => _useCase.EjecutarAsync(dto);

        await act.Should().ThrowAsync<FechasInvalidasException>();
    }

    [Fact]
    public async Task EjecutarAsync_FechaInicioPosteriorAFin_LanzaFechasInvalidas()
    {
        var dto = CrearDtoValido();
        dto.FechaInicio = DateTime.UtcNow.Date.AddDays(5);
        dto.FechaFin = DateTime.UtcNow.Date.AddDays(2);

        var act = () => _useCase.EjecutarAsync(dto);

        await act.Should().ThrowAsync<FechasInvalidasException>();
    }

    [Fact]
    public async Task EjecutarAsync_CeroHuespedes_LanzaValidationException()
    {
        var dto = CrearDtoValido();
        dto.CantidadHuespedes = 0;

        var act = () => _useCase.EjecutarAsync(dto);

        await act.Should().ThrowAsync<ValidationException>();
    }

    [Fact]
    public async Task EjecutarAsync_CalculaMontoCorrectamente()
    {
        var dto = CrearDtoValido();
        // 3 noches x $50.000
        _repoDpto.Setup(r => r.ObtenerPorIdAsync(1)).ReturnsAsync(DeptoActivo);
        _repoCliente.Setup(r => r.ExisteAsync(1)).ReturnsAsync(true);
        _repoReserva.Setup(r => r.HaySolapamientoAsync(It.IsAny<int>(), It.IsAny<DateTime>(), It.IsAny<DateTime>(), null)).ReturnsAsync(false);

        await _useCase.EjecutarAsync(dto);

        _repoReserva.Verify(r => r.AgregarAsync(It.Is<Reserva>(res =>
            res.MontoTotal == 150000m
        )), Times.Once);
    }
}
