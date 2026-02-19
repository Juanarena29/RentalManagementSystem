using CT.Domain.Entities;
using CT.Domain.Interfaces;
using CT.Infraestructure.Data;
using Microsoft.EntityFrameworkCore;

namespace CT.Infraestructure.Repositories;

public class RepositorioUser : IRepositorioUser
{
    private readonly CTDbContext _context;

    public RepositorioUser(CTDbContext context)
    {
        _context = context;
    }

    // ========== CRUD ==========

    public async Task AgregarAsync(User user)
    {
        _context.Users.Add(user);
        await _context.SaveChangesAsync();
    }

    public async Task ModificarAsync(User user)
    {
        _context.Users.Update(user);
        await _context.SaveChangesAsync();
    }

    public async Task EliminarAsync(int id)
    {
        var user = await _context.Users.FindAsync(id);
        if (user != null)
        {
            _context.Users.Remove(user);
            await _context.SaveChangesAsync();
        }
    }

    public async Task<User?> ObtenerPorIdAsync(int id)
    {
        return await _context.Users.FindAsync(id);
    }

    public async Task<List<User>> ListarAsync()
    {
        return await _context.Users
            .OrderBy(u => u.Nombre)
            .AsNoTracking()
            .ToListAsync();
    }

    public async Task<bool> ExisteAsync(int id)
    {
        return await _context.Users.AnyAsync(u => u.Id == id);
    }

    // ========== Autenticación ==========

    public async Task<User?> ObtenerPorEmailAsync(string email)
    {
        return await _context.Users
            .FirstOrDefaultAsync(u => u.Email == email);
    }

    public async Task<bool> ExisteEmailAsync(string email)
    {
        return await _context.Users.AnyAsync(u => u.Email == email);
    }
}
