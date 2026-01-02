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

        builder.Property(t => t.AreaId)
            .IsRequired();

        builder.Property(t => t.FechaCreacion)
            .IsRequired();

        builder.Property(t => t.Activo)
            .IsRequired()
            .HasDefaultValue(true);

        // Relación con Area
        builder.HasOne(t => t.Area)
            .WithMany(a => a.TiposSolicitud)
            .HasForeignKey(t => t.AreaId)
            .OnDelete(DeleteBehavior.Restrict);

        // Índice único compuesto en Nombre y AreaId
        // Esto permite que diferentes áreas tengan tipos de solicitud con el mismo nombre
        builder.HasIndex(t => new { t.Nombre, t.AreaId })
            .IsUnique()
            .HasDatabaseName("IX_TiposSolicitud_Nombre_AreaId");

        // Índice en AreaId para mejorar el rendimiento de las consultas
        builder.HasIndex(t => t.AreaId)
            .HasDatabaseName("IX_TiposSolicitud_AreaId");
    }
}
