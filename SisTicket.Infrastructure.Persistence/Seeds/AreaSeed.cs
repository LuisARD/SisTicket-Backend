using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SisTicket.Core.Domain.Entities;

namespace SisTicket.Infrastructure.Persistence.Seeds;

public class AreaSeed : IEntityTypeConfiguration<Area>
{
    public void Configure(EntityTypeBuilder<Area> builder)
    {
        builder.HasData(
            new Area
            {
                Id = 1,
                Nombre = "Tecnología de la Información",
                Descripcion = "Área encargada de soporte técnico, desarrollo y mantenimiento de sistemas",
                FechaCreacion = new DateTime(2024, 1, 1, 0, 0, 0, DateTimeKind.Utc),
                Activo = true
            },
            new Area
            {
                Id = 2,
                Nombre = "Recursos Humanos",
                Descripcion = "Gestión de personal, nómina y desarrollo organizacional",
                FechaCreacion = new DateTime(2024, 1, 1, 0, 0, 0, DateTimeKind.Utc),
                Activo = true
            },
            new Area
            {
                Id = 3,
                Nombre = "Finanzas",
                Descripcion = "Control presupuestario, contabilidad y tesorería",
                FechaCreacion = new DateTime(2024, 1, 1, 0, 0, 0, DateTimeKind.Utc),
                Activo = true
            },
            new Area
            {
                Id = 4,
                Nombre = "Operaciones",
                Descripcion = "Gestión de procesos operativos y logística",
                FechaCreacion = new DateTime(2024, 1, 1, 0, 0, 0, DateTimeKind.Utc),
                Activo = true
            },
            new Area
            {
                Id = 5,
                Nombre = "Administración",
                Descripcion = "Servicios generales y gestión administrativa",
                FechaCreacion = new DateTime(2024, 1, 1, 0, 0, 0, DateTimeKind.Utc),
                Activo = true
            }
        );
    }
}
