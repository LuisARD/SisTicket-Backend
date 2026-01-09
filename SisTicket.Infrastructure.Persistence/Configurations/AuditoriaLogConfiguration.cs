using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SisTicket.Core.Domain.Entities;

namespace SisTicket.Infrastructure.Persistence.Configurations;

public class AuditoriaLogConfiguration : IEntityTypeConfiguration<AuditoriaLog>
{
    public void Configure(EntityTypeBuilder<AuditoriaLog> builder)
    {
        builder.ToTable("AuditoriaLogs");

        builder.HasKey(a => a.Id);

        builder.Property(a => a.NombreUsuario)
            .HasMaxLength(100);

        builder.Property(a => a.Rol)
            .HasMaxLength(50);

        builder.Property(a => a.TipoAccion)
            .IsRequired();

        builder.Property(a => a.Entidad)
            .IsRequired()
            .HasMaxLength(100);

        builder.Property(a => a.Accion)
            .IsRequired()
            .HasMaxLength(500);

        builder.Property(a => a.Descripcion)
            .HasMaxLength(2000);

        builder.Property(a => a.IpAddress)
            .HasMaxLength(50);

        builder.Property(a => a.UserAgent)
            .HasMaxLength(500);

        builder.Property(a => a.Exitoso)
            .IsRequired();

        builder.Property(a => a.FechaHoraUtc)
            .IsRequired();

        // Relación con Usuario
        builder.HasOne(a => a.Usuario)
            .WithMany()
            .HasForeignKey(a => a.UsuarioId)
            .OnDelete(DeleteBehavior.SetNull);

        // Índices para mejorar performance de consultas
        builder.HasIndex(a => a.UsuarioId);
        builder.HasIndex(a => a.FechaHoraUtc);
        builder.HasIndex(a => a.TipoAccion);
        builder.HasIndex(a => new { a.Entidad, a.EntidadId });
    }
}
