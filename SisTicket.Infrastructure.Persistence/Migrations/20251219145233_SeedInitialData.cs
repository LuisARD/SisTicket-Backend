using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace SisTicket.Infrastructure.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class SeedInitialData : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.InsertData(
                table: "Areas",
                columns: new[] { "Id", "Activo", "Descripcion", "FechaCreacion", "FechaModificacion", "Nombre" },
                values: new object[,]
                {
                    { 1, true, "Área encargada de soporte técnico, desarrollo y mantenimiento de sistemas", new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), null, "Tecnología de la Información" },
                    { 2, true, "Gestión de personal, nómina y desarrollo organizacional", new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), null, "Recursos Humanos" },
                    { 3, true, "Control presupuestario, contabilidad y tesorería", new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), null, "Finanzas" },
                    { 4, true, "Gestión de procesos operativos y logística", new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), null, "Operaciones" },
                    { 5, true, "Servicios generales y gestión administrativa", new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), null, "Administración" }
                });

            migrationBuilder.InsertData(
                table: "Prioridades",
                columns: new[] { "Id", "Activo", "Descripcion", "FechaCreacion", "FechaModificacion", "Nivel", "Nombre" },
                values: new object[,]
                {
                    { 1, true, "Solicitud sin urgencia, puede resolverse en tiempo extendido", new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), null, 1, "Baja" },
                    { 2, true, "Solicitud con prioridad normal, debe atenderse en tiempo razonable", new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), null, 2, "Media" },
                    { 3, true, "Solicitud urgente, requiere atención prioritaria", new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), null, 3, "Alta" },
                    { 4, true, "Solicitud crítica, requiere atención inmediata", new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), null, 4, "Crítica" }
                });

            migrationBuilder.InsertData(
                table: "TiposSolicitud",
                columns: new[] { "Id", "Activo", "Descripcion", "FechaCreacion", "FechaModificacion", "Nombre" },
                values: new object[,]
                {
                    { 1, true, "Solicitudes relacionadas con problemas técnicos de hardware o software", new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), null, "Soporte Técnico" },
                    { 2, true, "Solicitudes de mantenimiento preventivo o correctivo", new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), null, "Mantenimiento" },
                    { 3, true, "Solicitudes de nuevas funcionalidades o servicios", new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), null, "Nuevo Requerimiento" },
                    { 4, true, "Reporte de problemas o errores en sistemas o procesos", new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), null, "Incidencia" },
                    { 5, true, "Solicitudes de información o asesoría", new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), null, "Consulta" },
                    { 6, true, "Solicitudes relacionadas con accesos a sistemas o permisos", new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), null, "Acceso y Permisos" }
                });

            migrationBuilder.InsertData(
                table: "Usuarios",
                columns: new[] { "Id", "Activo", "Apellido", "AreaId", "Email", "FechaCreacion", "FechaModificacion", "Nombre", "NombreUsuario", "PasswordHash", "Rol" },
                values: new object[,]
                {
                    { 1, true, "Motos", 1, "cesar@gmail.com", new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), null, "Cesar", "cesar", "AQAAAAIAAYagAAAAECesarPasswordHashNeedsToBeReplacedWithRealHashInProduction", 4 },
                    { 2, true, "TI", 1, "admin@gmail.com", new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), null, "Administrador", "admin", "AQAAAAIAAYagAAAAEPasswordHashNeedsToBeReplacedWithRealHashInProduction", 3 },
                    { 3, true, "Técnico", 1, "gestor@gmail.com", new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), null, "Gestor", "gestor", "AQAAAAIAAYagAAAAEPasswordHashNeedsToBeReplacedWithRealHashInProduction", 2 },
                    { 4, true, "Demostración", 2, "usuario@gmail.com", new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), null, "Solicitante", "usuario", "AQAAAAIAAYagAAAAEPasswordHashNeedsToBeReplacedWithRealHashInProduction", 1 }
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "Areas",
                keyColumn: "Id",
                keyValue: 3);

            migrationBuilder.DeleteData(
                table: "Areas",
                keyColumn: "Id",
                keyValue: 4);

            migrationBuilder.DeleteData(
                table: "Areas",
                keyColumn: "Id",
                keyValue: 5);

            migrationBuilder.DeleteData(
                table: "Prioridades",
                keyColumn: "Id",
                keyValue: 1);

            migrationBuilder.DeleteData(
                table: "Prioridades",
                keyColumn: "Id",
                keyValue: 2);

            migrationBuilder.DeleteData(
                table: "Prioridades",
                keyColumn: "Id",
                keyValue: 3);

            migrationBuilder.DeleteData(
                table: "Prioridades",
                keyColumn: "Id",
                keyValue: 4);

            migrationBuilder.DeleteData(
                table: "TiposSolicitud",
                keyColumn: "Id",
                keyValue: 1);

            migrationBuilder.DeleteData(
                table: "TiposSolicitud",
                keyColumn: "Id",
                keyValue: 2);

            migrationBuilder.DeleteData(
                table: "TiposSolicitud",
                keyColumn: "Id",
                keyValue: 3);

            migrationBuilder.DeleteData(
                table: "TiposSolicitud",
                keyColumn: "Id",
                keyValue: 4);

            migrationBuilder.DeleteData(
                table: "TiposSolicitud",
                keyColumn: "Id",
                keyValue: 5);

            migrationBuilder.DeleteData(
                table: "TiposSolicitud",
                keyColumn: "Id",
                keyValue: 6);

            migrationBuilder.DeleteData(
                table: "Usuarios",
                keyColumn: "Id",
                keyValue: 1);

            migrationBuilder.DeleteData(
                table: "Usuarios",
                keyColumn: "Id",
                keyValue: 2);

            migrationBuilder.DeleteData(
                table: "Usuarios",
                keyColumn: "Id",
                keyValue: 3);

            migrationBuilder.DeleteData(
                table: "Usuarios",
                keyColumn: "Id",
                keyValue: 4);

            migrationBuilder.DeleteData(
                table: "Areas",
                keyColumn: "Id",
                keyValue: 1);

            migrationBuilder.DeleteData(
                table: "Areas",
                keyColumn: "Id",
                keyValue: 2);
        }
    }
}
