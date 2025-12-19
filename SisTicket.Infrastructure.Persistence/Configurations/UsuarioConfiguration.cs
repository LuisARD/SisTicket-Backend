using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SisTicket.Core.Domain.Entities;
using SisTicket.Core.Domain.Enums;

namespace SisTicket.Infrastructure.Persistence.Configurations;

public class UsuarioConfiguration : IEntityTypeConfiguration<Usuario>
{
    public void Configure(EntityTypeBuilder<Usuario> builder)
    {
        builder.ToTable("Usuarios");

        builder.HasKey(u => u.Id);

        builder.Property(u => u.NombreUsuario)
            .IsRequired()
            .HasMaxLength(50);

        builder.Property(u => u.Nombre)
            .IsRequired()
            .HasMaxLength(100);

        builder.Property(u => u.Apellido)
            .IsRequired()
            .HasMaxLength(100);

        builder.Property(u => u.Email)
            .IsRequired()
            .HasMaxLength(150);

        builder.Property(u => u.PasswordHash)
            .IsRequired()
            .HasMaxLength(500);

        builder.Property(u => u.Rol)
            .IsRequired()
            .HasConversion<int>();

        builder.Property(u => u.FechaCreacion)
            .IsRequired();

        builder.Property(u => u.Activo)
            .IsRequired()
            .HasDefaultValue(true);

        // Índice único en NombreUsuario
        builder.HasIndex(u => u.NombreUsuario)
            .IsUnique()
            .HasDatabaseName("IX_Usuarios_NombreUsuario");

        // Índice único en Email
        builder.HasIndex(u => u.Email)
            .IsUnique()
            .HasDatabaseName("IX_Usuarios_Email");

        // Relación con Área
        builder.HasOne(u => u.Area)
            .WithMany(a => a.Usuarios)
            .HasForeignKey(u => u.AreaId)
            .OnDelete(DeleteBehavior.SetNull);

        // Relación con Solicitudes Creadas
        builder.HasMany(u => u.SolicitudesCreadas)
            .WithOne(s => s.Solicitante)
            .HasForeignKey(s => s.SolicitanteId)
            .OnDelete(DeleteBehavior.Restrict);

        // Relación con Solicitudes Asignadas
        builder.HasMany(u => u.SolicitudesAsignadas)
            .WithOne(s => s.GestorAsignado)
            .HasForeignKey(s => s.GestorAsignadoId)
            .OnDelete(DeleteBehavior.SetNull);

        // Relación con Comentarios
        builder.HasMany(u => u.Comentarios)
            .WithOne(c => c.Usuario)
            .HasForeignKey(c => c.UsuarioId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}
