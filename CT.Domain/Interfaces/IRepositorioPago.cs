using System;
using CT.Domain.Entities;
using CT.Domain.Enums;

namespace CT.Domain.Interfaces;

/// <summary>
/// Repositorio para gestión de Pagos
/// </summary>
public interface IRepositorioPago
{
    // ========== Operaciones CRUD básicas ==========
    Task AgregarAsync(Pago pago);
    Task ModificarAsync(Pago pago);
    Task EliminarAsync(int id);

    // ========== Consultas básicas ==========
    Task<Pago?> ObtenerPorIdAsync(int id);
    Task<List<Pago>> ListarAsync();
    Task<bool> ExisteAsync(int id);

    // ========== Consultas por relación ==========
    /// <summary>
    /// Todos los pagos asociados a una reserva específica
    /// </summary>
    Task<List<Pago>> ListarPorReservaAsync(int reservaId);

    /// <summary>
    /// Todos los pagos realizados por un cliente (a través de sus reservas)
    /// </summary>
    Task<List<Pago>> ListarPorClienteAsync(int clienteId);

    // ========== Cálculos y agregaciones ==========
    /// <summary>
    /// Suma total de pagos confirmados para una reserva
    /// </summary>
    Task<decimal> ObtenerTotalPagadoAsync(int reservaId);

    /// <summary>
    /// Total histórico que ha pagado un cliente
    /// </summary>
    Task<decimal> ObtenerTotalPagadoPorClienteAsync(int clienteId);

    // ========== Consultas para reportes/administración ==========
    /// <summary>
    /// Pagos realizados en una fecha específica (útil para cierre de caja)
    /// </summary>
    Task<List<Pago>> ListarPorFechaAsync(DateTime fecha);

    /// <summary>
    /// Pagos en un período (reportes mensuales/anuales)
    /// </summary>
    Task<List<Pago>> ListarPorRangoFechasAsync(DateTime desde, DateTime hasta);

    /// <summary>
    /// Filtra pagos por medio de pago (Efectivo, Transferencia, etc.)
    /// </summary>
    Task<List<Pago>> ListarPorMedioPagoAsync(MedioPago medioPago);

    /// <summary>
    /// Filtra pagos por tipo (Seña, Parcial, Total)
    /// </summary>
    Task<List<Pago>> ListarPorTipoPagoAsync(TipoPago tipoPago);

    /// <summary>
    /// Suma de ingresos en un rango de fechas (para reportes financieros)
    /// </summary>
    Task<decimal> ObtenerIngresosPorPeriodoAsync(DateTime desde, DateTime hasta);
}
