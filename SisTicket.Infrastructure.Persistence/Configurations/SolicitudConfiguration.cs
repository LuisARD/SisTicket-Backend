using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SisTicket.Core.Domain.Entities;
using SisTicket.Core.Domain.Enums;

namespace SisTicket.Infrastructure.Persistence.Configurations;

public class SolicitudConfiguration : IEntityTypeConfiguration<Solicitud>
{
    public void Configure(EntityTypeBuilder<Solicitud> builder)
    {
        builder.ToTable("Solicitudes");

        builder.HasKey(s => s.Id);

        builder.Property(s => s.NumeroSolicitud)
            .IsRequired()
            .HasMaxLength(50);

        builder.Property(s => s.Titulo)
            .IsRequired()
            .HasMaxLength(200);

        builder.Property(s => s.Descripcion)
            .IsRequired()
            .HasMaxLength(2000);

        builder.Property(s => s.Estado)
            .IsRequired()
            .HasConversion<int>()
            .HasDefaultValue(EstadoSolicitud.Nueva);

        builder.Property(s => s.FechaCreacion)
            .IsRequired();

        builder.Property(s => s.Activo)
            .IsRequired()
            .HasDefaultValue(true);

        // Índice único en NumeroSolicitud
        builder.HasIndex(s => s.NumeroSolicitud)
            .IsUnique()
            .HasDatabaseName("IX_Solicitudes_NumeroSolicitud");

        // Índices para filtros comunes
        builder.HasIndex(s => s.Estado)
            .HasDatabaseName("IX_Solicitudes_Estado");

        builder.HasIndex(s => s.FechaCreacion)
            .HasDatabaseName("IX_Solicitudes_FechaCreacion");

        // Relación con Solicitante
        builder.HasOne(s => s.Solicitante)
            .WithMany(u => u.SolicitudesCreadas)
            .HasForeignKey(s => s.SolicitanteId)
            .OnDelete(DeleteBehavior.Restrict);

        // Relación con Gestor Asignado
        builder.HasOne(s => s.GestorAsignado)
            .WithMany(u => u.SolicitudesAsignadas)
            .HasForeignKey(s => s.GestorAsignadoId)
            .OnDelete(DeleteBehavior.SetNull);

        // Relación con TipoSolicitud
        builder.HasOne(s => s.TipoSolicitud)
            .WithMany(t => t.Solicitudes)
            .HasForeignKey(s => s.TipoSolicitudId)
            .OnDelete(DeleteBehavior.Restrict);

        // Relación con Prioridad
        builder.HasOne(s => s.Prioridad)
            .WithMany(p => p.Solicitudes)
            .HasForeignKey(s => s.PrioridadId)
            .OnDelete(DeleteBehavior.Restrict);

        // Relación con Area
        builder.HasOne(s => s.Area)
            .WithMany(a => a.Solicitudes)
            .HasForeignKey(s => s.AreaId)
            .OnDelete(DeleteBehavior.Restrict);

        // Relación con Comentarios
        builder.HasMany(s => s.Comentarios)
            .WithOne(c => c.Solicitud)
            .HasForeignKey(c => c.SolicitudId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}
