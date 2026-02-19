using CT.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace CT.Infraestructure.Data.Configurations;

public class ReservaConfiguration : IEntityTypeConfiguration<Reserva>
{
    public void Configure(EntityTypeBuilder<Reserva> builder)
    {
        builder.HasKey(r => r.ReservaId);

        builder.Property(r => r.PrecioPorNoche)
            .HasColumnType("decimal(18,2)");

        builder.Property(r => r.MontoTotal)
            .HasColumnType("decimal(18,2)");

        builder.Property(r => r.MontoSenia)
            .HasColumnType("decimal(18,2)");

        builder.Property(r => r.SaldoPendiente)
            .HasColumnType("decimal(18,2)");

        builder.Property(r => r.Observaciones)
            .HasMaxLength(500);

        // Enums como string
        builder.Property(r => r.Estado)
            .HasConversion<string>()
            .HasMaxLength(20);

        builder.Property(r => r.OrigenReserva)
            .HasConversion<string>()
            .HasMaxLength(20);

        // Relación: Reserva -> Departamento (muchas a uno)
        builder.HasOne(r => r.Departamento)
            .WithMany()
            .HasForeignKey(r => r.DepartamentoId)
            .OnDelete(DeleteBehavior.Restrict);

        // Relación: Reserva -> Cliente (muchas a uno)
        builder.HasOne(r => r.Cliente)
            .WithMany()
            .HasForeignKey(r => r.ClienteId)
            .OnDelete(DeleteBehavior.Restrict);

        // Índice para consultas frecuentes de disponibilidad
        builder.HasIndex(r => new { r.DepartamentoId, r.FechaInicio, r.FechaFin });
        builder.HasIndex(r => r.Estado);
        builder.HasIndex(r => r.ClienteId);
    }
}
