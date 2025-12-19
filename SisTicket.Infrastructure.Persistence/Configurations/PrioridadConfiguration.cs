using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SisTicket.Core.Domain.Entities;

namespace SisTicket.Infrastructure.Persistence.Configurations;

public class PrioridadConfiguration : IEntityTypeConfiguration<Prioridad>
{
    public void Configure(EntityTypeBuilder<Prioridad> builder)
    {
        builder.ToTable("Prioridades");

        builder.HasKey(p => p.Id);

        builder.Property(p => p.Nombre)
            .IsRequired()
            .HasMaxLength(50);

        builder.Property(p => p.Nivel)
            .IsRequired();

        builder.Property(p => p.Descripcion)
            .HasMaxLength(500);

        builder.Property(p => p.FechaCreacion)
            .IsRequired();

        builder.Property(p => p.Activo)
            .IsRequired()
            .HasDefaultValue(true);

        // Índice único en Nombre
        builder.HasIndex(p => p.Nombre)
            .IsUnique()
            .HasDatabaseName("IX_Prioridades_Nombre");

        // Índice en Nivel para ordenamiento
        builder.HasIndex(p => p.Nivel)
            .HasDatabaseName("IX_Prioridades_Nivel");
    }
}
