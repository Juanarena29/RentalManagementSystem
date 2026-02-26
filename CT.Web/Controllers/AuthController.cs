using CT.Application.DTOs;
using CT.Application.UseCases.Users;
using CT.Domain.Exceptions;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.RateLimiting;
using System.Security.Claims;

namespace CT.Web.Controllers;

[Route("api/auth")]
[ApiController]
public sealed class AuthController : ControllerBase
{
    private readonly UserLoginUseCase _loginUseCase;

    public AuthController(UserLoginUseCase loginUseCase)
    {
        _loginUseCase = loginUseCase;
    }

    [HttpPost("login")]
    [EnableRateLimiting("login")]
    public async Task<IActionResult> Login([FromForm] string email, [FromForm] string password)
    {
        try
        {
            var dto = new LoginDto(email, password);
            var user = await _loginUseCase.EjecutarAsync(dto);

            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
                new Claim(ClaimTypes.Name, $"{user.Nombre} {user.Apellido}"),
                new Claim(ClaimTypes.Email, user.Email),
                new Claim("DebeRestablecerPassword", user.DebeRestablecerPassword.ToString())
            };

            var identity = new ClaimsIdentity(claims, "CookieAuth");
            var principal = new ClaimsPrincipal(identity);

            await HttpContext.SignInAsync("CookieAuth", principal,
                new AuthenticationProperties
                {
                    IsPersistent = true,
                    ExpiresUtc = DateTimeOffset.UtcNow.AddDays(7)
                });

            if (user.DebeRestablecerPassword)
                return Redirect("/CambiarPassword");

            return Redirect("/");
        }
        catch (WrongCredentialsException)
        {
            return Redirect("/Login?error=1");
        }
        catch (Exception)
        {
            return Redirect("/Login?error=2");
        }
    }

    [HttpPost("logout")]
    public async Task<IActionResult> Logout()
    {
        await HttpContext.SignOutAsync("CookieAuth");
        return Redirect("/Login");
    }
}
