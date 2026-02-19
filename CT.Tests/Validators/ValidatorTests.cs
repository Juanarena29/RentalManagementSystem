using CT.Application.DTOs;
using CT.Application.Validations;
using CT.Domain.Exceptions;
using FluentAssertions;

namespace CT.Tests.Validators;

public class UserValidatorTests
{
    private readonly UserValidator _validador = new();

    // ========== Password Policy ==========

    [Theory]
    [InlineData("Ab1defgh")]   // 8 chars, mayúscula, número - OK
    [InlineData("Password1")]  // largo, mayúscula, número - OK
    [InlineData("A1bcdefgh")]  // OK
    public void ValidarFortaleza_PasswordFuerte_NoLanzaExcepcion(string password)
    {
        var act = () => _validador.ValidarCreacion("Juan", "Perez", "test@test.com", password);

        act.Should().NotThrow();
    }

    [Theory]
    [InlineData("short1A")]    // 7 chars (< 8)
    [InlineData("Ab1")]        // muy corta
    public void ValidarFortaleza_PasswordCorta_LanzaValidationException(string password)
    {
        var act = () => _validador.ValidarCreacion("Juan", "Perez", "test@test.com", password);

        act.Should().Throw<ValidationException>()
           .WithMessage("*al menos 8 caracteres*");
    }

    [Theory]
    [InlineData("abcdefgh1")]  // sin mayúscula
    [InlineData("12345678a")]  // sin mayúscula
    public void ValidarFortaleza_SinMayuscula_LanzaValidationException(string password)
    {
        var act = () => _validador.ValidarCreacion("Juan", "Perez", "test@test.com", password);

        act.Should().Throw<ValidationException>()
           .WithMessage("*mayúscula*");
    }

    [Theory]
    [InlineData("Abcdefgh")]   // sin número
    [InlineData("ABCDEFGH")]   // sin número
    public void ValidarFortaleza_SinNumero_LanzaValidationException(string password)
    {
        var act = () => _validador.ValidarCreacion("Juan", "Perez", "test@test.com", password);

        act.Should().Throw<ValidationException>()
           .WithMessage("*número*");
    }

    // ========== Email ==========

    [Theory]
    [InlineData("user@domain.com")]
    [InlineData("test.name@company.co")]
    [InlineData("a@b.c")]
    public void ValidarCreacion_EmailValido_NoLanzaExcepcion(string email)
    {
        var act = () => _validador.ValidarCreacion("Juan", "Perez", email, "Password1");

        act.Should().NotThrow();
    }

    [Theory]
    [InlineData("sinArroba")]
    [InlineData("sin@dominio")]
    [InlineData("")]
    public void ValidarCreacion_EmailInvalido_LanzaValidationException(string email)
    {
        var act = () => _validador.ValidarCreacion("Juan", "Perez", email, "Password1");

        act.Should().Throw<ValidationException>();
    }

    // ========== Nombre / Apellido ==========

    [Theory]
    [InlineData("", "Perez")]
    [InlineData("J", "Perez")]
    [InlineData("Juan", "")]
    [InlineData("Juan", "P")]
    public void ValidarCreacion_NombreOApellidoInvalido_LanzaValidationException(
        string nombre, string apellido)
    {
        var act = () => _validador.ValidarCreacion(nombre, apellido, "test@test.com", "Password1");

        act.Should().Throw<ValidationException>();
    }

    // ========== Cambio de Password ==========

    [Fact]
    public void ValidarCambioPassword_MismaPassword_LanzaValidationException()
    {
        var act = () => _validador.ValidarCambioPassword("SamePass1", "SamePass1");

        act.Should().Throw<ValidationException>()
           .WithMessage("*diferente*");
    }

    [Fact]
    public void ValidarCambioPassword_ActualVacia_LanzaValidationException()
    {
        var act = () => _validador.ValidarCambioPassword("", "NewPass1A");

        act.Should().Throw<ValidationException>();
    }
}

public class ClienteValidatorTests
{
    private readonly ClienteValidator _validador = new();

    [Fact]
    public void Validar_DatosCompletos_NoLanzaExcepcion()
    {
        var dto = new CrearClienteDto
        {
            Nombre = "Juan",
            Apellido = "Perez",
            DNI = "12345678",
            Email = "juan@test.com",
            Telefono = "+54 11 1234-5678"
        };

        var act = () => _validador.Validar(dto);

        act.Should().NotThrow();
    }

    [Fact]
    public void Validar_NombreVacio_LanzaValidationException()
    {
        var dto = new CrearClienteDto { Nombre = "", Apellido = "Perez", DNI = "12345678" };

        var act = () => _validador.Validar(dto);

        act.Should().Throw<ValidationException>().WithMessage("*nombre*");
    }

    [Fact]
    public void Validar_DniMuyCorto_LanzaValidationException()
    {
        var dto = new CrearClienteDto { Nombre = "Juan", Apellido = "Perez", DNI = "123" };

        var act = () => _validador.Validar(dto);

        act.Should().Throw<ValidationException>().WithMessage("*DNI*");
    }

    [Theory]
    [InlineData("1234567A")]  // letra
    [InlineData("12.345.678")] // puntos
    public void Validar_DniConCaracteresInvalidos_LanzaValidationException(string dni)
    {
        var dto = new CrearClienteDto { Nombre = "Juan", Apellido = "Perez", DNI = dni };

        var act = () => _validador.Validar(dto);

        act.Should().Throw<ValidationException>();
    }

    [Fact]
    public void Validar_EmailOpcionalVacio_NoLanzaExcepcion()
    {
        var dto = new CrearClienteDto
        {
            Nombre = "Juan",
            Apellido = "Perez",
            DNI = "12345678",
            Email = null
        };

        var act = () => _validador.Validar(dto);

        act.Should().NotThrow();
    }

    [Fact]
    public void Validar_EmailFormatoInvalido_LanzaValidationException()
    {
        var dto = new CrearClienteDto
        {
            Nombre = "Juan",
            Apellido = "Perez",
            DNI = "12345678",
            Email = "noesunmail"
        };

        var act = () => _validador.Validar(dto);

        act.Should().Throw<ValidationException>().WithMessage("*email*");
    }

    [Fact]
    public void Validar_TelefonoMuyCorto_LanzaValidationException()
    {
        var dto = new CrearClienteDto
        {
            Nombre = "Juan",
            Apellido = "Perez",
            DNI = "12345678",
            Telefono = "123"
        };

        var act = () => _validador.Validar(dto);

        act.Should().Throw<ValidationException>().WithMessage("*teléfono*");
    }
}

public class ReservaValidatorTests
{
    private readonly ReservaValidator _validador = new();

    [Fact]
    public void Validar_DatosValidos_NoLanzaExcepcion()
    {
        var dto = new CrearReservaDto
        {
            FechaInicio = DateTime.UtcNow.Date.AddDays(1),
            FechaFin = DateTime.UtcNow.Date.AddDays(3),
            CantidadHuespedes = 2
        };

        var act = () => _validador.Validar(dto);

        act.Should().NotThrow();
    }

    [Fact]
    public void Validar_FechaInicioIgualAFin_LanzaFechasInvalidas()
    {
        var fecha = DateTime.UtcNow.Date.AddDays(1);
        var dto = new CrearReservaDto
        {
            FechaInicio = fecha,
            FechaFin = fecha,
            CantidadHuespedes = 2
        };

        var act = () => _validador.Validar(dto);

        act.Should().Throw<FechasInvalidasException>();
    }

    [Fact]
    public void Validar_FechaEnPasado_LanzaFechasInvalidas()
    {
        var dto = new CrearReservaDto
        {
            FechaInicio = DateTime.UtcNow.Date.AddDays(-1),
            FechaFin = DateTime.UtcNow.Date.AddDays(2),
            CantidadHuespedes = 2
        };

        var act = () => _validador.Validar(dto);

        act.Should().Throw<FechasInvalidasException>();
    }

    [Theory]
    [InlineData(0)]
    [InlineData(-1)]
    public void Validar_HuespedesInvalidos_LanzaValidationException(int cantidad)
    {
        var dto = new CrearReservaDto
        {
            FechaInicio = DateTime.UtcNow.Date.AddDays(1),
            FechaFin = DateTime.UtcNow.Date.AddDays(3),
            CantidadHuespedes = cantidad
        };

        var act = () => _validador.Validar(dto);

        act.Should().Throw<ValidationException>();
    }
}
