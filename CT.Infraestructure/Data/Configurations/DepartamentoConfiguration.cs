using CT.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace CT.Infraestructure.Data.Configurations;

public class DepartamentoConfiguration : IEntityTypeConfiguration<Departamento>
{
    public void Configure(EntityTypeBuilder<Departamento> builder)
    {
        builder.HasKey(d => d.DepartamentoId);

        builder.Property(d => d.Nombre)
            .IsRequired()
            .HasMaxLength(50);

        builder.Property(d => d.Descripcion)
            .HasMaxLength(300);

        builder.Property(d => d.PrecioPorNoche)
            .HasColumnType("decimal(18,2)");

        builder.Property(d => d.Observaciones)
            .HasMaxLength(500);

        // Enum se guarda como string en la BD (legible)
        builder.Property(d => d.Estado)
            .HasConversion<string>()
            .HasMaxLength(20);

        // Índice único en Nombre
        builder.HasIndex(d => d.Nombre)
            .IsUnique();
    }
}
