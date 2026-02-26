using CT.Web.Components;
using CT.Web.Services;
using CT.Infraestructure.Data;
using CT.Infraestructure.Repositories;
using CT.Infraestructure.Services;
using CT.Domain.Interfaces;
using CT.Application.UseCases.Clientes;
using CT.Application.UseCases.Departamentos;
using CT.Application.UseCases.Reservas;
using CT.Application.UseCases.Pagos;
using CT.Application.UseCases.Users;
using CT.Application.UseCases.Utils;
using CT.Application.Validations;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.AspNetCore.RateLimiting;
using Microsoft.EntityFrameworkCore;
using System.Threading.RateLimiting;
using CT.Web.Middleware;

var builder = WebApplication.CreateBuilder(args);

// --- EF Core ---
builder.Services.AddDbContext<CTDbContext>(options =>
    options.UseSqlite(builder.Configuration.GetConnectionString("DefaultConnection")));

// --- Repositorios ---
builder.Services.AddScoped<IRepositorioCliente, RepositorioCliente>();
builder.Services.AddScoped<IRepositorioDepartamento, RepositorioDepartamento>();
builder.Services.AddScoped<IRepositorioPago, RepositorioPago>();
builder.Services.AddScoped<IRepositorioReserva, RepositorioReserva>();
builder.Services.AddScoped<IRepositorioUser, RepositorioUser>();

// --- Servicios de infraestructura ---
builder.Services.AddSingleton<IPasswordHasher, PasswordHasher>();
builder.Services.AddScoped<IUnitOfWork, UnitOfWork>();

// --- Validators ---
builder.Services.AddScoped<ClienteValidator>();
builder.Services.AddScoped<DepartamentoValidator>();
builder.Services.AddScoped<PagoValidator>();
builder.Services.AddScoped<ReservaValidator>();
builder.Services.AddScoped<UserValidator>();

// --- UseCases: Clientes ---
builder.Services.AddScoped<ClienteAltaUseCase>();
builder.Services.AddScoped<ClienteBuscarUseCase>();
builder.Services.AddScoped<ClienteBuscarPorDniUseCase>();
builder.Services.AddScoped<ClienteListarUseCase>();
builder.Services.AddScoped<ClienteModificarUseCase>();
builder.Services.AddScoped<ClienteObtenerDetalleUseCase>();

// --- UseCases: Departamentos ---
builder.Services.AddScoped<DepartamentoAltaUseCase>();
builder.Services.AddScoped<DepartamentoBajaUseCase>();
builder.Services.AddScoped<DepartamentoCambiarEstadoUseCase>();
builder.Services.AddScoped<DepartamentoListarUseCase>();
builder.Services.AddScoped<DepartamentoModificacionUseCase>();
builder.Services.AddScoped<DepartamentoObtenerDetalleUseCase>();

// --- UseCases: Reservas ---
builder.Services.AddScoped<ConsultarDisponibilidadUseCase>();
builder.Services.AddScoped<ObtenerCalendarioOcupacionUseCase>();
builder.Services.AddScoped<ReservaAltaUseCase>();
builder.Services.AddScoped<ReservaCancelarUseCase>();
builder.Services.AddScoped<ReservaConfirmarUseCase>();
builder.Services.AddScoped<ReservaFinalizarUseCase>();
builder.Services.AddScoped<ReservaListarCheckInHoyUseCase>();
builder.Services.AddScoped<ReservaListarCheckOutUseCase>();
builder.Services.AddScoped<ReservaListarPorClienteUseCase>();
builder.Services.AddScoped<ReservaListarPorDepartamentoUseCase>();
builder.Services.AddScoped<ReservaModificarUseCase>();
builder.Services.AddScoped<ReservaObtenerDetalleUseCase>();

// --- UseCases: Pagos ---
builder.Services.AddScoped<PagoListarReservaUseCase>();
builder.Services.AddScoped<PagoObtenerReporteCajaUseCase>();
builder.Services.AddScoped<PagoObtenerReservasConSaldoPendienteUseCase>();
builder.Services.AddScoped<PagoRegistrarUseCase>();

// --- UseCases: Users ---
builder.Services.AddScoped<UserCambiarPasswordUseCase>();
builder.Services.AddScoped<UserCrearUseCase>();
builder.Services.AddScoped<UserListarUseCase>();
builder.Services.AddScoped<UserLoginUseCase>();
builder.Services.AddScoped<UserModificarUseCase>();
builder.Services.AddScoped<UserObtenerPorIdUseCase>();

// --- UseCases: Utils ---
builder.Services.AddScoped<ObtenerEstadisticasOcupacionUseCase>();

// --- Autenticación ---
builder.Services.AddAuthentication("CookieAuth")
    .AddCookie("CookieAuth", options =>
    {
        options.LoginPath = "/Login";
        options.ExpireTimeSpan = TimeSpan.FromDays(7);
        options.SlidingExpiration = true;
        options.Cookie.HttpOnly = true;
        options.Cookie.SameSite = SameSiteMode.Strict;
    });
builder.Services.AddAuthorization();
builder.Services.AddCascadingAuthenticationState();
builder.Services.AddScoped<AuthenticationStateProvider, CustomAuthStateProvider>();

// --- Blazor ---
builder.Services.AddRazorComponents()
    .AddInteractiveServerComponents();
builder.Services.AddHttpContextAccessor();
builder.Services.AddControllers();

// --- Rate Limiting ---
builder.Services.AddRateLimiter(options =>
{
    options.AddFixedWindowLimiter("login", opt =>
    {
        opt.PermitLimit = 5;
        opt.Window = TimeSpan.FromMinutes(1);
        opt.QueueLimit = 0;
    });
    options.RejectionStatusCode = StatusCodes.Status429TooManyRequests;
});

var app = builder.Build();

// --- Middleware global de excepciones ---
app.UseMiddleware<GlobalExceptionMiddleware>();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error", createScopeForErrors: true);
    app.UseHsts();
}

app.UseHttpsRedirection();

app.UseStaticFiles();
app.UseRateLimiter();
app.UseAuthentication();
app.UseAuthorization();
app.UseAntiforgery();

app.MapControllers();
app.MapRazorComponents<App>()
    .AddInteractiveServerRenderMode();

app.Run();
