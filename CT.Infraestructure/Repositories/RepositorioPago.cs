using CT.Domain.Entities;
using CT.Domain.Enums;
using CT.Domain.Interfaces;
using CT.Infraestructure.Data;
using Microsoft.EntityFrameworkCore;

namespace CT.Infraestructure.Repositories;

public class RepositorioPago : IRepositorioPago
{
    private readonly CTDbContext _context;

    public RepositorioPago(CTDbContext context)
    {
        _context = context;
    }

    // ========== CRUD ==========

    public async Task AgregarAsync(Pago pago)
    {
        _context.Pagos.Add(pago);
        await _context.SaveChangesAsync();
    }

    public async Task ModificarAsync(Pago pago)
    {
        _context.Pagos.Update(pago);
        await _context.SaveChangesAsync();
    }

    public async Task EliminarAsync(int id)
    {
        var pago = await _context.Pagos.FindAsync(id);
        if (pago != null)
        {
            _context.Pagos.Remove(pago);
            await _context.SaveChangesAsync();
        }
    }

    // ========== Consultas básicas ==========

    public async Task<Pago?> ObtenerPorIdAsync(int id)
    {
        return await _context.Pagos
            .Include(p => p.Reserva)
            .FirstOrDefaultAsync(p => p.PagoId == id);
    }

    public async Task<List<Pago>> ListarAsync()
    {
        return await _context.Pagos
            .Include(p => p.Reserva)
            .OrderByDescending(p => p.FechaPago)
            .AsNoTracking()
            .ToListAsync();
    }

    public async Task<bool> ExisteAsync(int id)
    {
        return await _context.Pagos.AnyAsync(p => p.PagoId == id);
    }

    // ========== Consultas por relación ==========

    public async Task<List<Pago>> ListarPorReservaAsync(int reservaId)
    {
        return await _context.Pagos
            .Where(p => p.ReservaId == reservaId)
            .OrderByDescending(p => p.FechaPago)
            .AsNoTracking()
            .ToListAsync();
    }

    public async Task<List<Pago>> ListarPorClienteAsync(int clienteId)
    {
        return await _context.Pagos
            .Include(p => p.Reserva)
            .Where(p => p.Reserva!.ClienteId == clienteId)
            .OrderByDescending(p => p.FechaPago)
            .AsNoTracking()
            .ToListAsync();
    }

    // ========== Cálculos y agregaciones ==========

    public async Task<decimal> ObtenerTotalPagadoAsync(int reservaId)
    {
        var montos = await _context.Pagos
            .Where(p => p.ReservaId == reservaId)
            .Where(p => p.EstadoPago == EstadoPago.Confirmado)
            .Select(p => p.Monto)
            .ToListAsync();
        return montos.Sum();
    }

    public async Task<decimal> ObtenerTotalPagadoPorClienteAsync(int clienteId)
    {
        var montos = await _context.Pagos
            .Where(p => p.Reserva!.ClienteId == clienteId)
            .Where(p => p.EstadoPago == EstadoPago.Confirmado)
            .Select(p => p.Monto)
            .ToListAsync();
        return montos.Sum();
    }

    // ========== Consultas para reportes/administración ==========

    public async Task<List<Pago>> ListarPorFechaAsync(DateTime fecha)
    {
        return await _context.Pagos
            .Include(p => p.Reserva)
            .Where(p => p.FechaPago.Date == fecha.Date)
            .OrderBy(p => p.FechaPago)
            .AsNoTracking()
            .ToListAsync();
    }

    public async Task<List<Pago>> ListarPorRangoFechasAsync(DateTime desde, DateTime hasta)
    {
        return await _context.Pagos
            .Include(p => p.Reserva)
            .Where(p => p.FechaPago >= desde && p.FechaPago <= hasta)
            .OrderBy(p => p.FechaPago)
            .AsNoTracking()
            .ToListAsync();
    }

    public async Task<List<Pago>> ListarPorMedioPagoAsync(MedioPago medioPago)
    {
        return await _context.Pagos
            .Include(p => p.Reserva)
            .Where(p => p.MedioPago == medioPago)
            .OrderByDescending(p => p.FechaPago)
            .AsNoTracking()
            .ToListAsync();
    }

    public async Task<List<Pago>> ListarPorTipoPagoAsync(TipoPago tipoPago)
    {
        return await _context.Pagos
            .Include(p => p.Reserva)
            .Where(p => p.TipoPago == tipoPago)
            .OrderByDescending(p => p.FechaPago)
            .AsNoTracking()
            .ToListAsync();
    }

    public async Task<decimal> ObtenerIngresosPorPeriodoAsync(DateTime desde, DateTime hasta)
    {
        var montos = await _context.Pagos
            .Where(p => p.FechaPago >= desde && p.FechaPago <= hasta)
            .Where(p => p.EstadoPago == EstadoPago.Confirmado)
            .Select(p => p.Monto)
            .ToListAsync();
        return montos.Sum();
    }
}
