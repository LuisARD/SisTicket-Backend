using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SisTicket.Core.Domain.Entities;

namespace SisTicket.Infrastructure.Persistence.Configurations;

public class AdjuntoConfiguration : IEntityTypeConfiguration<Adjunto>
{
    public void Configure(EntityTypeBuilder<Adjunto> builder)
    {
        builder.ToTable("Adjuntos");

        builder.HasKey(a => a.Id);

        builder.Property(a => a.NombreArchivo)
            .IsRequired()
            .HasMaxLength(255);

        builder.Property(a => a.NombreArchivoOriginal)
            .IsRequired()
            .HasMaxLength(255);

        builder.Property(a => a.RutaArchivo)
            .IsRequired()
            .HasMaxLength(500);

        builder.Property(a => a.TipoContenido)
            .IsRequired()
            .HasMaxLength(100);

        builder.Property(a => a.TamanoBytes)
            .IsRequired();

        builder.Property(a => a.CargadoPorId)
            .IsRequired();

        builder.Property(a => a.FechaCreacion)
            .IsRequired();

        builder.Property(a => a.Activo)
            .IsRequired()
            .HasDefaultValue(true);

        // Relación con Solicitud
        builder.HasOne(a => a.Solicitud)
            .WithMany(s => s.Adjuntos)
            .HasForeignKey(a => a.SolicitudId)
            .OnDelete(DeleteBehavior.Cascade);

        // Relación con Usuario (quien cargó el archivo)
        builder.HasOne(a => a.CargadoPor)
            .WithMany()
            .HasForeignKey(a => a.CargadoPorId)
            .OnDelete(DeleteBehavior.Restrict);

        // Índice en SolicitudId para mejorar consultas
        builder.HasIndex(a => a.SolicitudId)
            .HasDatabaseName("IX_Adjuntos_SolicitudId");

        // Índice en FechaCreacion
        builder.HasIndex(a => a.FechaCreacion)
            .HasDatabaseName("IX_Adjuntos_FechaCreacion");
    }
}
