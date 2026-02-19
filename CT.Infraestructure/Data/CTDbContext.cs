using CT.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace CT.Infraestructure.Data;

public class CTDbContext : DbContext
{
    public CTDbContext(DbContextOptions<CTDbContext> options) : base(options) { }

    public DbSet<Cliente> Clientes => Set<Cliente>();
    public DbSet<Departamento> Departamentos => Set<Departamento>();
    public DbSet<Reserva> Reservas => Set<Reserva>();
    public DbSet<Pago> Pagos => Set<Pago>();
    public DbSet<User> Users => Set<User>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(CTDbContext).Assembly);

        // ========== SEED DATA: Usuario Admin Inicial ==========
        // Este usuario se crea automáticamente al ejecutar la primera migración.
        // Credenciales: admin@ct.com / Admin123!
        // IMPORTANTE: Cambiar la contraseña después del primer login.
        // Hash pre-computado con salt fijo para que no cambie entre migraciones.
        modelBuilder.Entity<User>().HasData(new
        {
            Id = 1,
            Nombre = "Admin",
            Apellido = "Sistema",
            Email = "admin@ct.com",
            PasswordHash = "K7gGU3sko+OL0wNhqoVWhg==.iuM/GYARyHjeoH8vFo7BTyZ0vt60R/LVBOP7DdKtki8=",
            DebeRestablecerPassword = true
        });
    }
}
