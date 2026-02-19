using CT.Domain.Entities;
using CT.Domain.Enums;
using CT.Domain.Interfaces;
using CT.Infraestructure.Data;
using Microsoft.EntityFrameworkCore;

namespace CT.Infraestructure.Repositories;

public class RepositorioReserva : IRepositorioReserva
{
    private readonly CTDbContext _context;

    public RepositorioReserva(CTDbContext context)
    {
        _context = context;
    }

    // ========== CRUD ==========

    public async Task AgregarAsync(Reserva reserva)
    {
        _context.Reservas.Add(reserva);
        await _context.SaveChangesAsync();
    }

    public async Task ModificarAsync(Reserva reserva)
    {
        _context.Reservas.Update(reserva);
        await _context.SaveChangesAsync();
    }

    public async Task EliminarAsync(int id)
    {
        var reserva = await _context.Reservas.FindAsync(id);
        if (reserva != null)
        {
            _context.Reservas.Remove(reserva);
            await _context.SaveChangesAsync();
        }
    }

    // ========== Consultas b\u00e1sicas ==========

    public async Task<Reserva?> ObtenerPorIdAsync(int id)
    {
        return await _context.Reservas.FindAsync(id);
    }

    public async Task<List<Reserva>> ListarAsync()
    {
        return await _context.Reservas
            .Include(r => r.Departamento)
            .Include(r => r.Cliente)
            .OrderByDescending(r => r.FechaCreacion)
            .AsNoTracking()
            .ToListAsync();
    }

    // ========== Filtros ==========

    public async Task<List<Reserva>> ListarPorDepartamentoAsync(int departamentoId)
    {
        return await _context.Reservas
            .Include(r => r.Departamento)
            .Include(r => r.Cliente)
            .Where(r => r.DepartamentoId == departamentoId)
            .OrderByDescending(r => r.FechaInicio)
            .AsNoTracking()
            .ToListAsync();
    }

    public async Task<List<Reserva>> ListarPorClienteAsync(int clienteId)
    {
        return await _context.Reservas
            .Include(r => r.Departamento)
            .Where(r => r.ClienteId == clienteId)
            .OrderByDescending(r => r.FechaInicio)
            .AsNoTracking()
            .ToListAsync();
    }

    public async Task<List<Reserva>> ListarPorEstadoAsync(EstadoReserva estado)
    {
        return await _context.Reservas
            .Include(r => r.Departamento)
            .Include(r => r.Cliente)
            .Where(r => r.Estado == estado)
            .OrderByDescending(r => r.FechaInicio)
            .AsNoTracking()
            .ToListAsync();
    }

    public async Task<List<Reserva>> ListarPorRangoFechasAsync(DateTime desde, DateTime hasta)
    {
        return await _context.Reservas
            .Include(r => r.Departamento)
            .Include(r => r.Cliente)
            .Where(r => r.FechaInicio < hasta && r.FechaFin > desde)
            .OrderBy(r => r.FechaInicio)
            .AsNoTracking()
            .ToListAsync();
    }

    // ========== Validaciones cr\u00edticas ==========

    public async Task<bool> ExisteAsync(int id)
    {
        return await _context.Reservas.AnyAsync(r => r.ReservaId == id);
    }

    public async Task<bool> HaySolapamientoAsync(
        int departamentoId, DateTime fechaInicio, DateTime fechaFin, int? excluirReservaId = null)
    {
        var query = _context.Reservas
            .Where(r => r.DepartamentoId == departamentoId)
            .Where(r => r.Estado != EstadoReserva.Cancelada)
            .Where(r => r.FechaInicio < fechaFin && r.FechaFin > fechaInicio);

        if (excluirReservaId.HasValue)
            query = query.Where(r => r.ReservaId != excluirReservaId.Value);

        return await query.AnyAsync();
    }

    // ========== UI / Calendario ==========

    public async Task<List<Reserva>> ObtenerOcupacionAsync(int departamentoId, DateTime desde, DateTime hasta)
    {
        return await _context.Reservas
            .Include(r => r.Cliente)
            .Where(r => r.DepartamentoId == departamentoId)
            .Where(r => r.Estado != EstadoReserva.Cancelada)
            .Where(r => r.FechaInicio < hasta && r.FechaFin > desde)
            .OrderBy(r => r.FechaInicio)
            .AsNoTracking()
            .ToListAsync();
    }

    public async Task<Reserva?> ObtenerDetalleAsync(int id)
    {
        return await _context.Reservas
            .Include(r => r.Departamento)
            .Include(r => r.Cliente)
            .AsNoTracking()
            .FirstOrDefaultAsync(r => r.ReservaId == id);
    }

    // ========== Consultas adicionales ==========

    public async Task<List<Reserva>> ListarReservasActivasAsync()
    {
        return await _context.Reservas
            .Include(r => r.Departamento)
            .Include(r => r.Cliente)
            .Where(r => r.Estado == EstadoReserva.Pendiente || r.Estado == EstadoReserva.Confirmada)
            .OrderBy(r => r.FechaInicio)
            .AsNoTracking()
            .ToListAsync();
    }

    public async Task<List<Reserva>> ListarProximosCheckInsAsync(DateTime fecha)
    {
        return await _context.Reservas
            .Include(r => r.Departamento)
            .Include(r => r.Cliente)
            .Where(r => r.FechaInicio.Date == fecha.Date)
            .Where(r => r.Estado == EstadoReserva.Pendiente || r.Estado == EstadoReserva.Confirmada)
            .OrderBy(r => r.DepartamentoId)
            .AsNoTracking()
            .ToListAsync();
    }

    public async Task<List<Reserva>> ListarProximosCheckOutsAsync(DateTime fecha)
    {
        return await _context.Reservas
            .Include(r => r.Departamento)
            .Include(r => r.Cliente)
            .Where(r => r.FechaFin.Date == fecha.Date)
            .Where(r => r.Estado == EstadoReserva.Confirmada)
            .OrderBy(r => r.DepartamentoId)
            .AsNoTracking()
            .ToListAsync();
    }

    public async Task<List<Reserva>> ObtenerReservasConSaldoPendienteAsync()
    {
        return await _context.Reservas
            .Include(r => r.Departamento)
            .Include(r => r.Cliente)
            .Where(r => r.Estado != EstadoReserva.Cancelada)
            .Where(r => r.SaldoPendiente > 0)
            .OrderBy(r => r.FechaInicio)
            .AsNoTracking()
            .ToListAsync();
    }
}
