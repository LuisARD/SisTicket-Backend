using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SisTicket.Infrastructure.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class SimplificarAuditoriaLog : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Activo",
                table: "AuditoriaLogs");

            migrationBuilder.DropColumn(
                name: "FechaCreacion",
                table: "AuditoriaLogs");

            migrationBuilder.DropColumn(
                name: "FechaModificacion",
                table: "AuditoriaLogs");

            migrationBuilder.DropColumn(
                name: "UserAgent",
                table: "AuditoriaLogs");

            migrationBuilder.RenameColumn(
                name: "FechaHoraUtc",
                table: "AuditoriaLogs",
                newName: "FechaHora");

            migrationBuilder.RenameIndex(
                name: "IX_AuditoriaLogs_FechaHoraUtc",
                table: "AuditoriaLogs",
                newName: "IX_AuditoriaLogs_FechaHora");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "FechaHora",
                table: "AuditoriaLogs",
                newName: "FechaHoraUtc");

            migrationBuilder.RenameIndex(
                name: "IX_AuditoriaLogs_FechaHora",
                table: "AuditoriaLogs",
                newName: "IX_AuditoriaLogs_FechaHoraUtc");

            migrationBuilder.AddColumn<bool>(
                name: "Activo",
                table: "AuditoriaLogs",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<DateTime>(
                name: "FechaCreacion",
                table: "AuditoriaLogs",
                type: "datetime2",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<DateTime>(
                name: "FechaModificacion",
                table: "AuditoriaLogs",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "UserAgent",
                table: "AuditoriaLogs",
                type: "nvarchar(500)",
                maxLength: 500,
                nullable: true);
        }
    }
}
