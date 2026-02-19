using CT.Domain.Entities;
using CT.Domain.Interfaces;
using CT.Infraestructure.Data;
using Microsoft.EntityFrameworkCore;

namespace CT.Infraestructure.Repositories;

public class RepositorioCliente : IRepositorioCliente
{
    private readonly CTDbContext _context;

    public RepositorioCliente(CTDbContext context)
    {
        _context = context;
    }

    // ========== CRUD ==========

    public async Task AgregarAsync(Cliente cliente)
    {
        _context.Clientes.Add(cliente);
        await _context.SaveChangesAsync();
    }

    public async Task ModificarAsync(Cliente cliente)
    {
        _context.Clientes.Update(cliente);
        await _context.SaveChangesAsync();
    }

    public async Task EliminarAsync(int id)
    {
        var cliente = await _context.Clientes.FindAsync(id);
        if (cliente != null)
        {
            _context.Clientes.Remove(cliente);
            await _context.SaveChangesAsync();
        }
    }

    // ========== Consultas b\u00e1sicas ==========

    public async Task<Cliente?> ObtenerPorIdAsync(int id)
    {
        return await _context.Clientes.FindAsync(id);
    }

    public async Task<List<Cliente>> ListarAsync()
    {
        return await _context.Clientes
            .OrderBy(c => c.Apellido)
            .ThenBy(c => c.Nombre)
            .AsNoTracking()
            .ToListAsync();
    }

    public async Task<bool> ExisteAsync(int id)
    {
        return await _context.Clientes.AnyAsync(c => c.ClienteId == id);
    }

    // ========== B\u00fasquedas espec\u00edficas ==========

    public async Task<Cliente?> ObtenerPorDniAsync(string dni)
    {
        return await _context.Clientes
            .AsNoTracking()
            .FirstOrDefaultAsync(c => c.DNI == dni);
    }

    public async Task<Cliente?> ObtenerPorTelefonoAsync(string telefono)
    {
        return await _context.Clientes
            .AsNoTracking()
            .FirstOrDefaultAsync(c => c.Telefono == telefono);
    }

    public async Task<List<Cliente>> BuscarAsync(string textoBusqueda)
    {
        var texto = textoBusqueda.ToLowerInvariant();
        return await _context.Clientes
            .Where(c =>
                c.Nombre.ToLower().Contains(texto) ||
                c.Apellido.ToLower().Contains(texto) ||
                c.DNI.Contains(texto) ||
                (c.Telefono != null && c.Telefono.Contains(texto)))
            .OrderBy(c => c.Apellido)
            .ThenBy(c => c.Nombre)
            .AsNoTracking()
            .ToListAsync();
    }

    public async Task<bool> ExisteDniAsync(string dni, int? excluirClienteId = null)
    {
        var query = _context.Clientes.Where(c => c.DNI == dni);

        if (excluirClienteId.HasValue)
            query = query.Where(c => c.ClienteId != excluirClienteId.Value);

        return await query.AnyAsync();
    }

    // ========== Consultas para historial ==========

    public async Task<List<Cliente>> ObtenerClientesConReservasAsync()
    {
        return await _context.Clientes
            .Where(c => _context.Reservas.Any(r => r.ClienteId == c.ClienteId))
            .OrderBy(c => c.Apellido)
            .AsNoTracking()
            .ToListAsync();
    }

    public async Task<List<Cliente>> ObtenerClientesRecurrentesAsync(int minimoReservas = 2)
    {
        return await _context.Clientes
            .Where(c => _context.Reservas.Count(r => r.ClienteId == c.ClienteId) >= minimoReservas)
            .OrderBy(c => c.Apellido)
            .AsNoTracking()
            .ToListAsync();
    }
}
