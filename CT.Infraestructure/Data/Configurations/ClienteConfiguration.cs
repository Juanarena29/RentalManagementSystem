using CT.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace CT.Infraestructure.Data.Configurations;

public class ClienteConfiguration : IEntityTypeConfiguration<Cliente>
{
    public void Configure(EntityTypeBuilder<Cliente> builder)
    {
        builder.HasKey(c => c.ClienteId);

        builder.Property(c => c.Nombre)
            .IsRequired()
            .HasMaxLength(100);

        builder.Property(c => c.Apellido)
            .IsRequired()
            .HasMaxLength(100);

        builder.Property(c => c.DNI)
            .IsRequired()
            .HasMaxLength(8);

        builder.Property(c => c.Telefono)
            .HasMaxLength(30);

        builder.Property(c => c.Email)
            .HasMaxLength(200);

        builder.Property(c => c.Ciudad)
            .HasMaxLength(100);

        builder.Property(c => c.Provincia)
            .HasMaxLength(100);

        builder.Property(c => c.Pais)
            .HasMaxLength(100);

        builder.Property(c => c.Observaciones)
            .HasMaxLength(500);

        // Índice único en DNI
        builder.HasIndex(c => c.DNI)
            .IsUnique();
    }
}
