using CT.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace CT.Infraestructure.Data.Configurations;

public class PagoConfiguration : IEntityTypeConfiguration<Pago>
{
    public void Configure(EntityTypeBuilder<Pago> builder)
    {
        builder.HasKey(p => p.PagoId);

        builder.Property(p => p.Monto)
            .HasColumnType("decimal(18,2)");

        builder.Property(p => p.Observaciones)
            .HasMaxLength(500);

        // Enums como string
        builder.Property(p => p.TipoPago)
            .HasConversion<string>()
            .HasMaxLength(20);

        builder.Property(p => p.MedioPago)
            .HasConversion<string>()
            .HasMaxLength(20);

        builder.Property(p => p.EstadoPago)
            .HasConversion<string>()
            .HasMaxLength(30);

        // Relación: Pago -> Reserva (muchas a uno)
        builder.HasOne(p => p.Reserva)
            .WithMany()
            .HasForeignKey(p => p.ReservaId)
            .OnDelete(DeleteBehavior.Restrict);

        // Índice para listar pagos por reserva (consulta frecuente)
        builder.HasIndex(p => p.ReservaId);
        builder.HasIndex(p => p.FechaPago);
    }
}
