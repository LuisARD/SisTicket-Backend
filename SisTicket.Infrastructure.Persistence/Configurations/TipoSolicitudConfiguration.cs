using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SisTicket.Core.Domain.Entities;

namespace SisTicket.Infrastructure.Persistence.Configurations;

public class TipoSolicitudConfiguration : IEntityTypeConfiguration<TipoSolicitud>
{
    public void Configure(EntityTypeBuilder<TipoSolicitud> builder)
    {
        builder.ToTable("TiposSolicitud");

        builder.HasKey(t => t.Id);

        builder.Property(t => t.Nombre)
            .IsRequired()
            .HasMaxLength(100);

        builder.Property(t => t.Descripcion)
            .HasMaxLength(500);

        builder.Property(t => t.FechaCreacion)
            .IsRequired();

        builder.Property(t => t.Activo)
            .IsRequired()
            .HasDefaultValue(true);

        // Índice único en Nombre
        builder.HasIndex(t => t.Nombre)
            .IsUnique()
            .HasDatabaseName("IX_TiposSolicitud_Nombre");
    }
}
