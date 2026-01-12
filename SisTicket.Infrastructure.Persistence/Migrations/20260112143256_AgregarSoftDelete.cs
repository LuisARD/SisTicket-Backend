using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SisTicket.Infrastructure.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class AgregarSoftDelete : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "Eliminado",
                table: "Usuarios",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<DateTime>(
                name: "FechaEliminacion",
                table: "Usuarios",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "Eliminado",
                table: "TiposSolicitud",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<DateTime>(
                name: "FechaEliminacion",
                table: "TiposSolicitud",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "Eliminado",
                table: "Solicitudes",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<DateTime>(
                name: "FechaEliminacion",
                table: "Solicitudes",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "Eliminado",
                table: "Prioridades",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<DateTime>(
                name: "FechaEliminacion",
                table: "Prioridades",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "Eliminado",
                table: "Comentarios",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<DateTime>(
                name: "FechaEliminacion",
                table: "Comentarios",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "Eliminado",
                table: "Areas",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<DateTime>(
                name: "FechaEliminacion",
                table: "Areas",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "Eliminado",
                table: "Adjuntos",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<DateTime>(
                name: "FechaEliminacion",
                table: "Adjuntos",
                type: "datetime2",
                nullable: true);

            migrationBuilder.UpdateData(
                table: "Areas",
                keyColumn: "Id",
                keyValue: 1,
                columns: new[] { "Eliminado", "FechaEliminacion" },
                values: new object[] { false, null });

            migrationBuilder.UpdateData(
                table: "Areas",
                keyColumn: "Id",
                keyValue: 2,
                columns: new[] { "Eliminado", "FechaEliminacion" },
                values: new object[] { false, null });

            migrationBuilder.UpdateData(
                table: "Areas",
                keyColumn: "Id",
                keyValue: 3,
                columns: new[] { "Eliminado", "FechaEliminacion" },
                values: new object[] { false, null });

            migrationBuilder.UpdateData(
                table: "Areas",
                keyColumn: "Id",
                keyValue: 4,
                columns: new[] { "Eliminado", "FechaEliminacion" },
                values: new object[] { false, null });

            migrationBuilder.UpdateData(
                table: "Areas",
                keyColumn: "Id",
                keyValue: 5,
                columns: new[] { "Eliminado", "FechaEliminacion" },
                values: new object[] { false, null });

            migrationBuilder.UpdateData(
                table: "Prioridades",
                keyColumn: "Id",
                keyValue: 1,
                columns: new[] { "Eliminado", "FechaEliminacion" },
                values: new object[] { false, null });

            migrationBuilder.UpdateData(
                table: "Prioridades",
                keyColumn: "Id",
                keyValue: 2,
                columns: new[] { "Eliminado", "FechaEliminacion" },
                values: new object[] { false, null });

            migrationBuilder.UpdateData(
                table: "Prioridades",
                keyColumn: "Id",
                keyValue: 3,
                columns: new[] { "Eliminado", "FechaEliminacion" },
                values: new object[] { false, null });

            migrationBuilder.UpdateData(
                table: "Prioridades",
                keyColumn: "Id",
                keyValue: 4,
                columns: new[] { "Eliminado", "FechaEliminacion" },
                values: new object[] { false, null });

            migrationBuilder.UpdateData(
                table: "TiposSolicitud",
                keyColumn: "Id",
                keyValue: 1,
                columns: new[] { "Eliminado", "FechaEliminacion" },
                values: new object[] { false, null });

            migrationBuilder.UpdateData(
                table: "TiposSolicitud",
                keyColumn: "Id",
                keyValue: 2,
                columns: new[] { "Eliminado", "FechaEliminacion" },
                values: new object[] { false, null });

            migrationBuilder.UpdateData(
                table: "TiposSolicitud",
                keyColumn: "Id",
                keyValue: 3,
                columns: new[] { "Eliminado", "FechaEliminacion" },
                values: new object[] { false, null });

            migrationBuilder.UpdateData(
                table: "TiposSolicitud",
                keyColumn: "Id",
                keyValue: 4,
                columns: new[] { "Eliminado", "FechaEliminacion" },
                values: new object[] { false, null });

            migrationBuilder.UpdateData(
                table: "TiposSolicitud",
                keyColumn: "Id",
                keyValue: 5,
                columns: new[] { "Eliminado", "FechaEliminacion" },
                values: new object[] { false, null });

            migrationBuilder.UpdateData(
                table: "TiposSolicitud",
                keyColumn: "Id",
                keyValue: 6,
                columns: new[] { "Eliminado", "FechaEliminacion" },
                values: new object[] { false, null });

            migrationBuilder.UpdateData(
                table: "Usuarios",
                keyColumn: "Id",
                keyValue: 1,
                columns: new[] { "Eliminado", "FechaEliminacion" },
                values: new object[] { false, null });

            migrationBuilder.UpdateData(
                table: "Usuarios",
                keyColumn: "Id",
                keyValue: 2,
                columns: new[] { "Eliminado", "FechaEliminacion" },
                values: new object[] { false, null });

            migrationBuilder.UpdateData(
                table: "Usuarios",
                keyColumn: "Id",
                keyValue: 3,
                columns: new[] { "Eliminado", "FechaEliminacion" },
                values: new object[] { false, null });

            migrationBuilder.UpdateData(
                table: "Usuarios",
                keyColumn: "Id",
                keyValue: 4,
                columns: new[] { "Eliminado", "FechaEliminacion" },
                values: new object[] { false, null });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Eliminado",
                table: "Usuarios");

            migrationBuilder.DropColumn(
                name: "FechaEliminacion",
                table: "Usuarios");

            migrationBuilder.DropColumn(
                name: "Eliminado",
                table: "TiposSolicitud");

            migrationBuilder.DropColumn(
                name: "FechaEliminacion",
                table: "TiposSolicitud");

            migrationBuilder.DropColumn(
                name: "Eliminado",
                table: "Solicitudes");

            migrationBuilder.DropColumn(
                name: "FechaEliminacion",
                table: "Solicitudes");

            migrationBuilder.DropColumn(
                name: "Eliminado",
                table: "Prioridades");

            migrationBuilder.DropColumn(
                name: "FechaEliminacion",
                table: "Prioridades");

            migrationBuilder.DropColumn(
                name: "Eliminado",
                table: "Comentarios");

            migrationBuilder.DropColumn(
                name: "FechaEliminacion",
                table: "Comentarios");

            migrationBuilder.DropColumn(
                name: "Eliminado",
                table: "Areas");

            migrationBuilder.DropColumn(
                name: "FechaEliminacion",
                table: "Areas");

            migrationBuilder.DropColumn(
                name: "Eliminado",
                table: "Adjuntos");

            migrationBuilder.DropColumn(
                name: "FechaEliminacion",
                table: "Adjuntos");
        }
    }
}
