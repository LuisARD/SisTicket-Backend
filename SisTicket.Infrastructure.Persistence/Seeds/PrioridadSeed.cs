using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SisTicket.Core.Domain.Entities;

namespace SisTicket.Infrastructure.Persistence.Seeds;

public class PrioridadSeed : IEntityTypeConfiguration<Prioridad>
{
    public void Configure(EntityTypeBuilder<Prioridad> builder)
    {
        builder.HasData(
            new Prioridad
            {
                Id = 1,
                Nombre = "Baja",
                Nivel = 1,
                Descripcion = "Solicitud sin urgencia, puede resolverse en tiempo extendido",
                FechaCreacion = new DateTime(2024, 1, 1, 0, 0, 0, DateTimeKind.Utc),
                Activo = true
            },
            new Prioridad
            {
                Id = 2,
                Nombre = "Media",
                Nivel = 2,
                Descripcion = "Solicitud con prioridad normal, debe atenderse en tiempo razonable",
                FechaCreacion = new DateTime(2024, 1, 1, 0, 0, 0, DateTimeKind.Utc),
                Activo = true
            },
            new Prioridad
            {
                Id = 3,
                Nombre = "Alta",
                Nivel = 3,
                Descripcion = "Solicitud urgente, requiere atención prioritaria",
                FechaCreacion = new DateTime(2024, 1, 1, 0, 0, 0, DateTimeKind.Utc),
                Activo = true
            },
            new Prioridad
            {
                Id = 4,
                Nombre = "Crítica",
                Nivel = 4,
                Descripcion = "Solicitud crítica, requiere atención inmediata",
                FechaCreacion = new DateTime(2024, 1, 1, 0, 0, 0, DateTimeKind.Utc),
                Activo = true
            }
        );
    }
}
