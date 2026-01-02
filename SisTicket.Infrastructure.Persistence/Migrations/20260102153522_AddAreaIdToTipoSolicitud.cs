using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SisTicket.Infrastructure.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class AddAreaIdToTipoSolicitud : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            // Paso 1: Eliminar el índice único anterior en Nombre
            migrationBuilder.DropIndex(
                name: "IX_TiposSolicitud_Nombre",
                table: "TiposSolicitud");

            // Paso 2: Agregar la columna AreaId como nullable temporalmente
            migrationBuilder.AddColumn<int>(
                name: "AreaId",
                table: "TiposSolicitud",
                type: "int",
                nullable: true);

            // Paso 3: Migrar datos existentes - Asignar AreaId basado en el nombre/tipo
            // Todos los tipos técnicos van al área de TI (AreaId = 1)
            migrationBuilder.Sql(@"
                UPDATE TiposSolicitud 
                SET AreaId = 1 
                WHERE Nombre IN ('Soporte Técnico', 'Mantenimiento', 'Nuevo Requerimiento', 'Incidencia', 'Acceso y Permisos')
            ");

            // Los tipos de consulta van a Recursos Humanos (AreaId = 2)
            migrationBuilder.Sql(@"
                UPDATE TiposSolicitud 
                SET AreaId = 2 
                WHERE Nombre = 'Consulta'
            ");

            // Para cualquier otro tipo no mapeado, asignarlo al área de TI por defecto
            migrationBuilder.Sql(@"
                UPDATE TiposSolicitud 
                SET AreaId = 1 
                WHERE AreaId IS NULL
            ");

            // Paso 4: Hacer la columna NOT NULL
            migrationBuilder.AlterColumn<int>(
                name: "AreaId",
                table: "TiposSolicitud",
                type: "int",
                nullable: false,
                oldClrType: typeof(int),
                oldType: "int",
                oldNullable: true);

            // Paso 5: Actualizar los datos semilla con los valores correctos
            migrationBuilder.UpdateData(
                table: "TiposSolicitud",
                keyColumn: "Id",
                keyValue: 1,
                column: "AreaId",
                value: 1);

            migrationBuilder.UpdateData(
                table: "TiposSolicitud",
                keyColumn: "Id",
                keyValue: 2,
                columns: new[] { "AreaId", "Descripcion" },
                values: new object[] { 1, "Solicitudes de mantenimiento preventivo o correctivo de equipos" });

            migrationBuilder.UpdateData(
                table: "TiposSolicitud",
                keyColumn: "Id",
                keyValue: 3,
                columns: new[] { "AreaId", "Descripcion" },
                values: new object[] { 1, "Solicitudes de nuevas funcionalidades o servicios tecnológicos" });

            migrationBuilder.UpdateData(
                table: "TiposSolicitud",
                keyColumn: "Id",
                keyValue: 4,
                columns: new[] { "AreaId", "Descripcion" },
                values: new object[] { 1, "Reporte de problemas o errores en sistemas" });

            migrationBuilder.UpdateData(
                table: "TiposSolicitud",
                keyColumn: "Id",
                keyValue: 5,
                columns: new[] { "AreaId", "Descripcion", "Nombre" },
                values: new object[] { 1, "Solicitudes relacionadas con accesos a sistemas o permisos", "Acceso y Permisos" });

            migrationBuilder.UpdateData(
                table: "TiposSolicitud",
                keyColumn: "Id",
                keyValue: 6,
                columns: new[] { "AreaId", "Descripcion", "Nombre" },
                values: new object[] { 2, "Solicitudes de información o asesoría de recursos humanos", "Consulta" });

            // Paso 6: Crear índice en AreaId
            migrationBuilder.CreateIndex(
                name: "IX_TiposSolicitud_AreaId",
                table: "TiposSolicitud",
                column: "AreaId");

            // Paso 7: Crear índice único compuesto en Nombre y AreaId
            migrationBuilder.CreateIndex(
                name: "IX_TiposSolicitud_Nombre_AreaId",
                table: "TiposSolicitud",
                columns: new[] { "Nombre", "AreaId" },
                unique: true);

            // Paso 8: Crear la clave foránea
            migrationBuilder.AddForeignKey(
                name: "FK_TiposSolicitud_Areas_AreaId",
                table: "TiposSolicitud",
                column: "AreaId",
                principalTable: "Areas",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            // Eliminar la clave foránea
            migrationBuilder.DropForeignKey(
                name: "FK_TiposSolicitud_Areas_AreaId",
                table: "TiposSolicitud");

            // Eliminar índices
            migrationBuilder.DropIndex(
                name: "IX_TiposSolicitud_AreaId",
                table: "TiposSolicitud");

            migrationBuilder.DropIndex(
                name: "IX_TiposSolicitud_Nombre_AreaId",
                table: "TiposSolicitud");

            // Eliminar la columna AreaId
            migrationBuilder.DropColumn(
                name: "AreaId",
                table: "TiposSolicitud");

            // Revertir cambios en datos semilla
            migrationBuilder.UpdateData(
                table: "TiposSolicitud",
                keyColumn: "Id",
                keyValue: 2,
                column: "Descripcion",
                value: "Solicitudes de mantenimiento preventivo o correctivo");

            migrationBuilder.UpdateData(
                table: "TiposSolicitud",
                keyColumn: "Id",
                keyValue: 3,
                column: "Descripcion",
                value: "Solicitudes de nuevas funcionalidades o servicios");

            migrationBuilder.UpdateData(
                table: "TiposSolicitud",
                keyColumn: "Id",
                keyValue: 4,
                column: "Descripcion",
                value: "Reporte de problemas o errores en sistemas o procesos");

            migrationBuilder.UpdateData(
                table: "TiposSolicitud",
                keyColumn: "Id",
                keyValue: 5,
                columns: new[] { "Descripcion", "Nombre" },
                values: new object[] { "Solicitudes de información o asesoría", "Consulta" });

            migrationBuilder.UpdateData(
                table: "TiposSolicitud",
                keyColumn: "Id",
                keyValue: 6,
                columns: new[] { "Descripcion", "Nombre" },
                values: new object[] { "Solicitudes relacionadas con accesos a sistemas o permisos", "Acceso y Permisos" });

            // Recrear el índice único anterior
            migrationBuilder.CreateIndex(
                name: "IX_TiposSolicitud_Nombre",
                table: "TiposSolicitud",
                column: "Nombre",
                unique: true);
        }
    }
}
