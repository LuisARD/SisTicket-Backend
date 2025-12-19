using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SisTicket.Core.Domain.Entities;

namespace SisTicket.Infrastructure.Persistence.Configurations;

public class AreaConfiguration : IEntityTypeConfiguration<Area>
{
    public void Configure(EntityTypeBuilder<Area> builder)
    {
        builder.ToTable("Areas");

        builder.HasKey(a => a.Id);

        builder.Property(a => a.Nombre)
            .IsRequired()
            .HasMaxLength(100);

        builder.Property(a => a.Descripcion)
            .HasMaxLength(500);

        builder.Property(a => a.FechaCreacion)
            .IsRequired();

        builder.Property(a => a.Activo)
            .IsRequired()
            .HasDefaultValue(true);

        // Índice único en Nombre
        builder.HasIndex(a => a.Nombre)
            .IsUnique()
            .HasDatabaseName("IX_Areas_Nombre");
    }
}
