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

        // Relación con TiposSolicitud - la relación inversa se configura en TipoSolicitudConfiguration
        builder.HasMany(a => a.TiposSolicitud)
            .WithOne(t => t.Area)
            .HasForeignKey(t => t.AreaId)
            .OnDelete(DeleteBehavior.Restrict);

        // Índice único en Nombre
        builder.HasIndex(a => a.Nombre)
            .IsUnique()
            .HasDatabaseName("IX_Areas_Nombre");
    }
}
