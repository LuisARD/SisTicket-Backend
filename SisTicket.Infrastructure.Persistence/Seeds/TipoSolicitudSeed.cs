using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SisTicket.Core.Domain.Entities;

namespace SisTicket.Infrastructure.Persistence.Seeds;

public class TipoSolicitudSeed : IEntityTypeConfiguration<TipoSolicitud>
{
    public void Configure(EntityTypeBuilder<TipoSolicitud> builder)
    {
        builder.HasData(
            // Tipos de Solicitud para Tecnología de la Información (AreaId = 1)
            new TipoSolicitud
            {
                Id = 1,
                Nombre = "Soporte Técnico",
                Descripcion = "Solicitudes relacionadas con problemas técnicos de hardware o software",
                AreaId = 1, // Tecnología de la Información
                FechaCreacion = new DateTime(2024, 1, 1, 0, 0, 0, DateTimeKind.Utc),
                Activo = true
            },
            new TipoSolicitud
            {
                Id = 2,
                Nombre = "Mantenimiento",
                Descripcion = "Solicitudes de mantenimiento preventivo o correctivo de equipos",
                AreaId = 1, // Tecnología de la Información
                FechaCreacion = new DateTime(2024, 1, 1, 0, 0, 0, DateTimeKind.Utc),
                Activo = true
            },
            new TipoSolicitud
            {
                Id = 3,
                Nombre = "Nuevo Requerimiento",
                Descripcion = "Solicitudes de nuevas funcionalidades o servicios tecnológicos",
                AreaId = 1, // Tecnología de la Información
                FechaCreacion = new DateTime(2024, 1, 1, 0, 0, 0, DateTimeKind.Utc),
                Activo = true
            },
            new TipoSolicitud
            {
                Id = 4,
                Nombre = "Incidencia",
                Descripcion = "Reporte de problemas o errores en sistemas",
                AreaId = 1, // Tecnología de la Información
                FechaCreacion = new DateTime(2024, 1, 1, 0, 0, 0, DateTimeKind.Utc),
                Activo = true
            },
            new TipoSolicitud
            {
                Id = 5,
                Nombre = "Acceso y Permisos",
                Descripcion = "Solicitudes relacionadas con accesos a sistemas o permisos",
                AreaId = 1, // Tecnología de la Información
                FechaCreacion = new DateTime(2024, 1, 1, 0, 0, 0, DateTimeKind.Utc),
                Activo = true
            },

            // Tipos de Solicitud para Recursos Humanos (AreaId = 2)
            new TipoSolicitud
            {
                Id = 6,
                Nombre = "Consulta",
                Descripcion = "Solicitudes de información o asesoría de recursos humanos",
                AreaId = 2, // Recursos Humanos
                FechaCreacion = new DateTime(2024, 1, 1, 0, 0, 0, DateTimeKind.Utc),
                Activo = true
            }
        );
    }
}
