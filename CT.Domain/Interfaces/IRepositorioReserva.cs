using System;
using CT.Domain.Entities;
using CT.Domain.Enums;

namespace CT.Domain.Interfaces;

/// <summary>
/// Repositorio para gestión de Reservas
/// </summary>
public interface IRepositorioReserva
{
    // ========== Operaciones CRUD básicas ==========
    Task AgregarAsync(Reserva reserva);
    Task ModificarAsync(Reserva reserva);
    Task EliminarAsync(int id);

    // ========== Consultas básicas ==========
    Task<Reserva?> ObtenerPorIdAsync(int id);
    Task<List<Reserva>> ListarAsync();

    // ========== Filtros típicos del dominio ==========
    Task<List<Reserva>> ListarPorDepartamentoAsync(int departamentoId);
    Task<List<Reserva>> ListarPorClienteAsync(int clienteId);
    Task<List<Reserva>> ListarPorEstadoAsync(EstadoReserva estado);
    Task<List<Reserva>> ListarPorRangoFechasAsync(DateTime desde, DateTime hasta);

    // ========== Validaciones críticas de negocio ==========
    Task<bool> ExisteAsync(int id);

    /// <summary>
    /// CRÍTICO: Valida si existe solapamiento de reservas CONFIRMADAS en el mismo departamento.
    /// Solo considera estados activos: Confirmada, CheckIn, CheckOut (NO Tentativa ni Cancelada)
    /// </summary>
    Task<bool> HaySolapamientoAsync(
        int departamentoId,
        DateTime fechaInicio,
        DateTime fechaFin,
        int? excluirReservaId = null);

    // ========== Consultas para UI/Calendario ==========
    /// <summary>
    /// Obtiene todas las reservas que intersectan con el rango de fechas dado.
    /// Útil para mostrar ocupación en calendario.
    /// </summary>
    Task<List<Reserva>> ObtenerOcupacionAsync(
        int departamentoId,
        DateTime desde,
        DateTime hasta);

    /// <summary>
    /// Trae la reserva con relaciones cargadas (Include: Departamento, Cliente)
    /// </summary>
    Task<Reserva?> ObtenerDetalleAsync(int id);

    // ========== Consultas adicionales útiles ==========
    /// <summary>
    /// Lista reservas activas (Confirmada + CheckIn), excluyendo Tentativas y Canceladas
    /// </summary>
    Task<List<Reserva>> ListarReservasActivasAsync();

    /// <summary>
    /// Lista reservas con check-in programado para una fecha específica
    /// </summary>
    Task<List<Reserva>> ListarProximosCheckInsAsync(DateTime fecha);

    /// <summary>
    /// Lista reservas con check-out programado para una fecha específica
    /// </summary>
    Task<List<Reserva>> ListarProximosCheckOutsAsync(DateTime fecha);

    /// <summary>
    /// Lista reservas que tienen saldo pendiente (MontoTotal > MontoPagado)
    /// Requiere calcular con suma de pagos
    /// </summary>
    Task<List<Reserva>> ObtenerReservasConSaldoPendienteAsync();
}
