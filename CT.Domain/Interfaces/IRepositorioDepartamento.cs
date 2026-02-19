using System;
using CT.Domain.Entities;
using CT.Domain.Enums;

namespace CT.Domain.Interfaces;

/// <summary>
/// Repositorio para gestión de Departamentos
/// </summary>
public interface IRepositorioDepartamento
{
    // ========== Operaciones CRUD básicas ==========
    Task AgregarAsync(Departamento departamento);
    Task ModificarAsync(Departamento departamento);
    Task EliminarAsync(int id);

    // ========== Consultas básicas ==========
    Task<Departamento?> ObtenerPorIdAsync(int id);
    Task<List<Departamento>> ListarAsync();
    Task<bool> ExisteAsync(int id);

    // ========== Consultas específicas del dominio ==========
    /// <summary>
    /// Solo departamentos con estado Disponible (excluye los en Mantenimiento/Inactivos)
    /// </summary>
    Task<List<Departamento>> ListarDisponiblesAsync();

    /// <summary>
    /// Busca departamento por nombre
    /// </summary>
    Task<Departamento?> ObtenerPorNombreAsync(string nombre);

    /// <summary>
    /// Departamentos que cumplen capacidad mínima solicitada
    /// </summary>
    Task<List<Departamento>> ListarPorCapacidadAsync(int capacidadMinima);

    /// <summary>
    /// Valida si ya existe un nombre de departamento (útil para evitar duplicados)
    /// </summary>
    Task<bool> ExisteNombreAsync(string nombre, int? excluirDepartamentoId = null);

    // ========== Consultas para disponibilidad ==========
    /// <summary>
    /// Obtiene departamentos que están disponibles (sin reservas confirmadas) en el rango de fechas.
    /// Cruza con IRepositorioReserva para verificar solapamiento.
    /// </summary>
    Task<List<Departamento>> ObtenerDepartamentosDisponiblesEnRangoAsync(
        DateTime fechaInicio,
        DateTime fechaFin,
        int? cantidadHuespedes = null);
}
