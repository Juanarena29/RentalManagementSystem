using CT.Application.DTOs;
using CT.Application.UseCases.Users;
using CT.Application.Validations;
using CT.Domain.Entities;
using CT.Domain.Exceptions;
using CT.Domain.Interfaces;
using FluentAssertions;
using Moq;

namespace CT.Tests.UseCases.Users;

public class UserLoginUseCaseTests
{
    private readonly Mock<IRepositorioUser> _repoUser = new();
    private readonly Mock<IPasswordHasher> _hasher = new();
    private readonly UserValidator _validador = new();
    private readonly UserLoginUseCase _useCase;

    public UserLoginUseCaseTests()
    {
        _useCase = new UserLoginUseCase(_repoUser.Object, _hasher.Object, _validador);
    }

    private static User CrearUserValido() => new("Admin", "Sistema", "admin@ct.com")
    {
        Id = 1,
        PasswordHash = "hash_simulado",
        DebeRestablecerPassword = false
    };

    [Fact]
    public async Task EjecutarAsync_CredencialesValidas_RetornaUserDto()
    {
        var user = CrearUserValido();
        _repoUser.Setup(r => r.ObtenerPorEmailAsync("admin@ct.com")).ReturnsAsync(user);
        _hasher.Setup(h => h.Verify("Password1", "hash_simulado")).Returns(true);

        var resultado = await _useCase.EjecutarAsync(new LoginDto("admin@ct.com", "Password1"));

        resultado.Should().NotBeNull();
        resultado.Id.Should().Be(1);
        resultado.Email.Should().Be("admin@ct.com");
        resultado.DebeRestablecerPassword.Should().BeFalse();
    }

    [Fact]
    public async Task EjecutarAsync_EmailNoExiste_LanzaWrongCredentials()
    {
        _repoUser.Setup(r => r.ObtenerPorEmailAsync(It.IsAny<string>())).ReturnsAsync((User?)null);

        var act = () => _useCase.EjecutarAsync(new LoginDto("noexiste@ct.com", "Password1"));

        await act.Should().ThrowAsync<WrongCredentialsException>();
    }

    [Fact]
    public async Task EjecutarAsync_PasswordIncorrecta_LanzaWrongCredentials()
    {
        var user = CrearUserValido();
        _repoUser.Setup(r => r.ObtenerPorEmailAsync("admin@ct.com")).ReturnsAsync(user);
        _hasher.Setup(h => h.Verify(It.IsAny<string>(), It.IsAny<string>())).Returns(false);

        var act = () => _useCase.EjecutarAsync(new LoginDto("admin@ct.com", "WrongPass1"));

        await act.Should().ThrowAsync<WrongCredentialsException>();
    }

    [Fact]
    public async Task EjecutarAsync_MismoMensajeParaEmailYPasswordIncorrectos()
    {
        // Email incorrecto
        _repoUser.Setup(r => r.ObtenerPorEmailAsync(It.IsAny<string>())).ReturnsAsync((User?)null);
        var ex1 = await Assert.ThrowsAsync<WrongCredentialsException>(
            () => _useCase.EjecutarAsync(new LoginDto("wrong@ct.com", "Password1")));

        // Password incorrecta
        var user = CrearUserValido();
        _repoUser.Setup(r => r.ObtenerPorEmailAsync("admin@ct.com")).ReturnsAsync(user);
        _hasher.Setup(h => h.Verify(It.IsAny<string>(), It.IsAny<string>())).Returns(false);
        var ex2 = await Assert.ThrowsAsync<WrongCredentialsException>(
            () => _useCase.EjecutarAsync(new LoginDto("admin@ct.com", "WrongPass1")));

        // Mismo mensaje — no filtrar si falla email o password
        ex1.Message.Should().Be(ex2.Message);
    }

    [Fact]
    public async Task EjecutarAsync_EmailVacio_LanzaValidationException()
    {
        var act = () => _useCase.EjecutarAsync(new LoginDto("", "Password1"));

        await act.Should().ThrowAsync<ValidationException>();
    }

    [Fact]
    public async Task EjecutarAsync_PasswordVacia_LanzaValidationException()
    {
        var act = () => _useCase.EjecutarAsync(new LoginDto("admin@ct.com", ""));

        await act.Should().ThrowAsync<ValidationException>();
    }

    [Fact]
    public async Task EjecutarAsync_NormalizaEmailAMinusculas()
    {
        var user = CrearUserValido();
        _repoUser.Setup(r => r.ObtenerPorEmailAsync("admin@ct.com")).ReturnsAsync(user);
        _hasher.Setup(h => h.Verify(It.IsAny<string>(), It.IsAny<string>())).Returns(true);

        await _useCase.EjecutarAsync(new LoginDto("  Admin@CT.com  ", "Password1"));

        // Verifica que se buscó con email normalizado
        _repoUser.Verify(r => r.ObtenerPorEmailAsync("admin@ct.com"), Times.Once);
    }

    [Fact]
    public async Task EjecutarAsync_DebeRestablecerPassword_RetornaTrue()
    {
        var user = CrearUserValido();
        user.DebeRestablecerPassword = true;
        _repoUser.Setup(r => r.ObtenerPorEmailAsync("admin@ct.com")).ReturnsAsync(user);
        _hasher.Setup(h => h.Verify("Password1", "hash_simulado")).Returns(true);

        var resultado = await _useCase.EjecutarAsync(new LoginDto("admin@ct.com", "Password1"));

        resultado.DebeRestablecerPassword.Should().BeTrue();
    }
}

public class UserCambiarPasswordUseCaseTests
{
    private readonly Mock<IRepositorioUser> _repoUser = new();
    private readonly Mock<IPasswordHasher> _hasher = new();
    private readonly UserValidator _validador = new();
    private readonly UserCambiarPasswordUseCase _useCase;

    public UserCambiarPasswordUseCaseTests()
    {
        _useCase = new UserCambiarPasswordUseCase(_repoUser.Object, _hasher.Object, _validador);
    }

    [Fact]
    public async Task EjecutarAsync_CambioValido_ActualizaHashYDesmarcaFlag()
    {
        var user = new User("Test", "User", "test@ct.com")
        {
            Id = 1,
            PasswordHash = "old_hash",
            DebeRestablecerPassword = true
        };
        _repoUser.Setup(r => r.ObtenerPorIdAsync(1)).ReturnsAsync(user);
        _hasher.Setup(h => h.Verify("OldPass1", "old_hash")).Returns(true);
        _hasher.Setup(h => h.Hash("NewPass1A")).Returns("new_hash");

        await _useCase.EjecutarAsync(new CambiarPasswordDto(1, "OldPass1", "NewPass1A"));

        user.PasswordHash.Should().Be("new_hash");
        user.DebeRestablecerPassword.Should().BeFalse();
        _repoUser.Verify(r => r.ModificarAsync(user), Times.Once);
    }

    [Fact]
    public async Task EjecutarAsync_PasswordActualIncorrecta_LanzaWrongCredentials()
    {
        var user = new User("Test", "User", "test@ct.com")
        {
            Id = 1,
            PasswordHash = "old_hash"
        };
        _repoUser.Setup(r => r.ObtenerPorIdAsync(1)).ReturnsAsync(user);
        _hasher.Setup(h => h.Verify(It.IsAny<string>(), It.IsAny<string>())).Returns(false);

        var act = () => _useCase.EjecutarAsync(new CambiarPasswordDto(1, "WrongOld1", "NewPass1A"));

        await act.Should().ThrowAsync<WrongCredentialsException>();
    }

    [Fact]
    public async Task EjecutarAsync_MismaPassword_LanzaValidationException()
    {
        var act = () => _useCase.EjecutarAsync(new CambiarPasswordDto(1, "SamePass1", "SamePass1"));

        await act.Should().ThrowAsync<ValidationException>();
    }

    [Fact]
    public async Task EjecutarAsync_PasswordNuevaDebil_LanzaValidationException()
    {
        var act = () => _useCase.EjecutarAsync(new CambiarPasswordDto(1, "OldPass1A", "weak"));

        await act.Should().ThrowAsync<ValidationException>();
    }

    [Fact]
    public async Task EjecutarAsync_UserNoExiste_LanzaEntityNotFound()
    {
        _repoUser.Setup(r => r.ObtenerPorIdAsync(99)).ReturnsAsync((User?)null);

        var act = () => _useCase.EjecutarAsync(new CambiarPasswordDto(99, "OldPass1A", "NewPass1A"));

        await act.Should().ThrowAsync<EntityNotFoundException>();
    }
}
