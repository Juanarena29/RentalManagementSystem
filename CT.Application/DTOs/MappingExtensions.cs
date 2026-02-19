using CT.Domain.Entities;

namespace CT.Application.DTOs;

/// <summary>
/// Extension methods para mapear entidades de dominio a DTOs de salida.
/// Centraliza el mapeo para evitar duplicación en los UseCases.
/// </summary>
public static class MappingExtensions
{
    // ========== Cliente ==========

    public static ClienteDto MapToDto(this Cliente c) => new()
    {
        ClienteId = c.ClienteId,
        Nombre = c.Nombre,
        Apellido = c.Apellido,
        DNI = c.DNI,
        Telefono = c.Telefono,
        Email = c.Email,
        Ciudad = c.Ciudad,
        Provincia = c.Provincia,
        Pais = c.Pais,
        Observaciones = c.Observaciones,
        FechaCreacion = c.FechaCreacion
    };

    public static List<ClienteDto> MapToDto(this IEnumerable<Cliente> clientes) =>
        clientes.Select(c => c.MapToDto()).ToList();

    // ========== Departamento ==========

    public static DepartamentoDto MapToDto(this Departamento d) => new()
    {
        DepartamentoId = d.DepartamentoId,
        Nombre = d.Nombre,
        Descripcion = d.Descripcion,
        CapacidadMaxima = d.CapacidadMaxima,
        PrecioPorNoche = d.PrecioPorNoche,
        Estado = d.Estado,
        Observaciones = d.Observaciones
    };

    public static List<DepartamentoDto> MapToDto(this IEnumerable<Departamento> departamentos) =>
        departamentos.Select(d => d.MapToDto()).ToList();

    // ========== Reserva → ReservaResumenDto ==========

    public static ReservaResumenDto MapToResumenDto(this Reserva r) => new()
    {
        ReservaId = r.ReservaId,
        DepartamentoId = r.DepartamentoId,
        DepartamentoNombre = r.Departamento?.Nombre ?? string.Empty,
        ClienteId = r.ClienteId,
        ClienteNombreCompleto = $"{r.Cliente?.Nombre} {r.Cliente?.Apellido}".Trim(),
        FechaInicio = r.FechaInicio,
        FechaFin = r.FechaFin,
        CantidadHuespedes = r.CantidadHuespedes,
        Estado = r.Estado,
        MontoTotal = r.MontoTotal,
        SaldoPendiente = r.SaldoPendiente,
        OrigenReserva = r.OrigenReserva
    };

    public static List<ReservaResumenDto> MapToResumenDto(this IEnumerable<Reserva> reservas) =>
        reservas.Select(r => r.MapToResumenDto()).ToList();

    // ========== Pago ==========

    public static PagoDto MapToDto(this Pago p) => new()
    {
        PagoId = p.PagoId,
        ReservaId = p.ReservaId,
        FechaPago = p.FechaPago,
        Monto = p.Monto,
        TipoPago = p.TipoPago,
        MedioPago = p.MedioPago,
        EstadoPago = p.EstadoPago,
        Observaciones = p.Observaciones
    };

    public static List<PagoDto> MapToDto(this IEnumerable<Pago> pagos) =>
        pagos.Select(p => p.MapToDto()).ToList();
}
