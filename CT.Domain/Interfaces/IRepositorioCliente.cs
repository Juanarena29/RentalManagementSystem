using System;
using CT.Domain.Entities;

namespace CT.Domain.Interfaces;

/// <summary>
/// Repositorio para gestión de Clientes
/// </summary>
public interface IRepositorioCliente
{
    // ========== Operaciones CRUD básicas ==========
    Task AgregarAsync(Cliente cliente);
    Task ModificarAsync(Cliente cliente);
    Task EliminarAsync(int id);

    // ========== Consultas básicas ==========
    Task<Cliente?> ObtenerPorIdAsync(int id);
    Task<List<Cliente>> ListarAsync();
    Task<bool> ExisteAsync(int id);

    // ========== Búsquedas específicas del dominio ==========
    /// <summary>
    /// Busca cliente por DNI (debe ser único)
    /// </summary>
    Task<Cliente?> ObtenerPorDniAsync(string dni);

    /// <summary>
    /// Busca cliente por teléfono (útil para contacto por WhatsApp)
    /// </summary>
    Task<Cliente?> ObtenerPorTelefonoAsync(string telefono);

    /// <summary>
    /// Búsqueda por texto en múltiples campos: Nombre, Apellido, DNI, Teléfono
    /// </summary>
    Task<List<Cliente>> BuscarAsync(string textoBusqueda);

    /// <summary>
    /// Valida si ya existe un DNI registrado (útil para validación al crear/editar)
    /// </summary>
    Task<bool> ExisteDniAsync(string dni, int? excluirClienteId = null);

    // ========== Consultas para historial ==========
    /// <summary>
    /// Clientes que tienen al menos una reserva
    /// </summary>
    Task<List<Cliente>> ObtenerClientesConReservasAsync();

    /// <summary>
    /// Clientes con 2 o más reservas (para marketing/fidelización)
    /// </summary>
    Task<List<Cliente>> ObtenerClientesRecurrentesAsync(int minimoReservas = 2);
}
