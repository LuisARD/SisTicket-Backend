using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SisTicket.Core.Domain.Entities;

namespace SisTicket.Infrastructure.Persistence.Configurations;

public class ComentarioConfiguration : IEntityTypeConfiguration<Comentario>
{
    public void Configure(EntityTypeBuilder<Comentario> builder)
    {
        builder.ToTable("Comentarios");

        builder.HasKey(c => c.Id);

        builder.Property(c => c.Texto)
            .IsRequired()
            .HasMaxLength(1000);

        builder.Property(c => c.FechaCreacion)
            .IsRequired();

        builder.Property(c => c.Activo)
            .IsRequired()
            .HasDefaultValue(true);

        // Índice en SolicitudId para consultas rápidas
        builder.HasIndex(c => c.SolicitudId)
            .HasDatabaseName("IX_Comentarios_SolicitudId");

        // Índice en FechaCreacion para ordenamiento
        builder.HasIndex(c => c.FechaCreacion)
            .HasDatabaseName("IX_Comentarios_FechaCreacion");

        // Relación con Solicitud
        builder.HasOne(c => c.Solicitud)
            .WithMany(s => s.Comentarios)
            .HasForeignKey(c => c.SolicitudId)
            .OnDelete(DeleteBehavior.Cascade);

        // Relación con Usuario
        builder.HasOne(c => c.Usuario)
            .WithMany(u => u.Comentarios)
            .HasForeignKey(c => c.UsuarioId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}
