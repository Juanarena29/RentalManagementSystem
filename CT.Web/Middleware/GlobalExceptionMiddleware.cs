using CT.Domain.Exceptions;
using System.Text.Json;

namespace CT.Web.Middleware;

/// <summary>
/// Middleware global que captura excepciones de dominio y retorna
/// respuestas JSON con el código HTTP apropiado, evitando la
/// filtración de stack traces al cliente.
/// </summary>
public class GlobalExceptionMiddleware
{
    private readonly RequestDelegate _next;
    private readonly ILogger<GlobalExceptionMiddleware> _logger;

    public GlobalExceptionMiddleware(RequestDelegate next, ILogger<GlobalExceptionMiddleware> logger)
    {
        _next = next;
        _logger = logger;
    }

    public async Task InvokeAsync(HttpContext context)
    {
        try
        {
            await _next(context);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Excepción no controlada: {Message}", ex.Message);

            if (!context.Response.HasStarted)
            {
                await HandleExceptionAsync(context, ex);
            }
        }
    }

    private static async Task HandleExceptionAsync(HttpContext context, Exception exception)
    {
        var (statusCode, message) = exception switch
        {
            EntityNotFoundException => (StatusCodes.Status404NotFound, exception.Message),
            WrongCredentialsException => (StatusCodes.Status401Unauthorized, "Credenciales inválidas."),
            ValidationException => (StatusCodes.Status400BadRequest, exception.Message),
            FechasInvalidasException => (StatusCodes.Status400BadRequest, exception.Message),
            CapacidadExcedidaException => (StatusCodes.Status400BadRequest, exception.Message),
            PagoInsuficienteException => (StatusCodes.Status400BadRequest, exception.Message),
            ReservaSuperpuestaException => (StatusCodes.Status409Conflict, exception.Message),
            DepartamentoNoDisponibleException => (StatusCodes.Status409Conflict, exception.Message),
            ReservaCanceladaException => (StatusCodes.Status409Conflict, exception.Message),
            DomainInvalidOperationException => (StatusCodes.Status409Conflict, exception.Message),
            _ => (StatusCodes.Status500InternalServerError,
                                                    "Ocurrió un error interno en el servidor.")
        };

        context.Response.ContentType = "application/json";
        context.Response.StatusCode = statusCode;

        var response = JsonSerializer.Serialize(new { error = message });
        await context.Response.WriteAsync(response);
    }
}
