using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SisTicket.Core.Domain.Entities;
using SisTicket.Core.Domain.Enums;

namespace SisTicket.Infrastructure.Persistence.Seeds;

public class UsuarioSeed : IEntityTypeConfiguration<Usuario>
{
    public void Configure(EntityTypeBuilder<Usuario> builder)
    {
        builder.HasData(
            // SuperAdmin - Usuario principal del sistema
            new Usuario
            {
                Id = 1,
                NombreUsuario = "cesar",
                Nombre = "Cesar",
                Apellido = "Motos",
                Email = "cesar@gmail.com",
                // Password: "cesar"
                PasswordHash = "AQAAAAIAAYagAAAAECesarPasswordHashNeedsToBeReplacedWithRealHashInProduction",
                Rol = Rol.SuperAdmin,
                AreaId = 1, // TI
                FechaCreacion = new DateTime(2024, 1, 1, 0, 0, 0, DateTimeKind.Utc),
                Activo = true
            },
            // Admin de TI
            new Usuario
            {
                Id = 2,
                NombreUsuario = "admin",
                Nombre = "Administrador",
                Apellido = "TI",
                Email = "admin@gmail.com",
                // Password: "password"
                PasswordHash = "AQAAAAIAAYagAAAAEPasswordHashNeedsToBeReplacedWithRealHashInProduction",
                Rol = Rol.Admin,
                AreaId = 1, // TI
                FechaCreacion = new DateTime(2024, 1, 1, 0, 0, 0, DateTimeKind.Utc),
                Activo = true
            },
            // Gestor de TI
            new Usuario
            {
                Id = 3,
                NombreUsuario = "gestor",
                Nombre = "Gestor",
                Apellido = "Técnico",
                Email = "gestor@gmail.com",
                // Password: "password"
                PasswordHash = "AQAAAAIAAYagAAAAEPasswordHashNeedsToBeReplacedWithRealHashInProduction",
                Rol = Rol.Gestor,
                AreaId = 1, // TI
                FechaCreacion = new DateTime(2024, 1, 1, 0, 0, 0, DateTimeKind.Utc),
                Activo = true
            },
            // Solicitante de ejemplo
            new Usuario
            {
                Id = 4,
                NombreUsuario = "usuario",
                Nombre = "Solicitante",
                Apellido = "Demostración",
                Email = "usuario@gmail.com",
                // Password: "password"
                PasswordHash = "AQAAAAIAAYagAAAAEPasswordHashNeedsToBeReplacedWithRealHashInProduction",
                Rol = Rol.Solicitante,
                AreaId = 2, // RRHH
                FechaCreacion = new DateTime(2024, 1, 1, 0, 0, 0, DateTimeKind.Utc),
                Activo = true
            }
        );
    }
}
