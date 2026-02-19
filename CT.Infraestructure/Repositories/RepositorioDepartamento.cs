using CT.Domain.Entities;
using CT.Domain.Enums;
using CT.Domain.Interfaces;
using CT.Infraestructure.Data;
using Microsoft.EntityFrameworkCore;

namespace CT.Infraestructure.Repositories;

public class RepositorioDepartamento : IRepositorioDepartamento
{
    private readonly CTDbContext _context;

    public RepositorioDepartamento(CTDbContext context)
    {
        _context = context;
    }

    // ========== CRUD ==========

    public async Task AgregarAsync(Departamento departamento)
    {
        _context.Departamentos.Add(departamento);
        await _context.SaveChangesAsync();
    }

    public async Task ModificarAsync(Departamento departamento)
    {
        _context.Departamentos.Update(departamento);
        await _context.SaveChangesAsync();
    }

    public async Task EliminarAsync(int id)
    {
        var departamento = await _context.Departamentos.FindAsync(id);
        if (departamento != null)
        {
            _context.Departamentos.Remove(departamento);
            await _context.SaveChangesAsync();
        }
    }

    // ========== Consultas b\u00e1sicas ==========

    public async Task<Departamento?> ObtenerPorIdAsync(int id)
    {
        return await _context.Departamentos.FindAsync(id);
    }

    public async Task<List<Departamento>> ListarAsync()
    {
        return await _context.Departamentos
            .OrderBy(d => d.Nombre)
            .AsNoTracking()
            .ToListAsync();
    }

    public async Task<bool> ExisteAsync(int id)
    {
        return await _context.Departamentos.AnyAsync(d => d.DepartamentoId == id);
    }

    // ========== Consultas espec\u00edficas ==========

    public async Task<List<Departamento>> ListarDisponiblesAsync()
    {
        return await _context.Departamentos
            .Where(d => d.Estado == EstadoDepartamento.Activo)
            .OrderBy(d => d.Nombre)
            .AsNoTracking()
            .ToListAsync();
    }

    public async Task<Departamento?> ObtenerPorNombreAsync(string nombre)
    {
        return await _context.Departamentos
            .AsNoTracking()
            .FirstOrDefaultAsync(d => d.Nombre == nombre);
    }

    public async Task<List<Departamento>> ListarPorCapacidadAsync(int capacidadMinima)
    {
        return await _context.Departamentos
            .Where(d => d.CapacidadMaxima >= capacidadMinima && d.Estado == EstadoDepartamento.Activo)
            .OrderBy(d => d.Nombre)
            .AsNoTracking()
            .ToListAsync();
    }

    public async Task<bool> ExisteNombreAsync(string nombre, int? excluirDepartamentoId = null)
    {
        var query = _context.Departamentos.Where(d => d.Nombre == nombre);

        if (excluirDepartamentoId.HasValue)
            query = query.Where(d => d.DepartamentoId != excluirDepartamentoId.Value);

        return await query.AnyAsync();
    }

    // ========== Disponibilidad ==========

    public async Task<List<Departamento>> ObtenerDepartamentosDisponiblesEnRangoAsync(
        DateTime fechaInicio, DateTime fechaFin, int? cantidadHuespedes = null)
    {
        var query = _context.Departamentos
            .Where(d => d.Estado == EstadoDepartamento.Activo)
            .Where(d => !_context.Reservas.Any(r =>
                r.DepartamentoId == d.DepartamentoId &&
                r.Estado != EstadoReserva.Cancelada &&
                r.FechaInicio < fechaFin &&
                r.FechaFin > fechaInicio));

        if (cantidadHuespedes.HasValue)
            query = query.Where(d => d.CapacidadMaxima >= cantidadHuespedes.Value);

        return await query
            .OrderBy(d => d.Nombre)
            .AsNoTracking()
            .ToListAsync();
    }
}
