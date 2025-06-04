using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DreamCleaningBackend.Migrations
{
    /// <inheritdoc />
    public partial class SeedAdminUser : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.UpdateData(
                table: "ExtraServices",
                keyColumn: "Id",
                keyValue: 1,
                column: "CreatedAt",
                value: new DateTime(2025, 6, 4, 13, 3, 39, 180, DateTimeKind.Utc).AddTicks(5695));

            migrationBuilder.UpdateData(
                table: "ExtraServices",
                keyColumn: "Id",
                keyValue: 2,
                column: "CreatedAt",
                value: new DateTime(2025, 6, 4, 13, 3, 39, 180, DateTimeKind.Utc).AddTicks(5700));

            migrationBuilder.UpdateData(
                table: "ExtraServices",
                keyColumn: "Id",
                keyValue: 3,
                column: "CreatedAt",
                value: new DateTime(2025, 6, 4, 13, 3, 39, 180, DateTimeKind.Utc).AddTicks(5702));

            migrationBuilder.UpdateData(
                table: "ExtraServices",
                keyColumn: "Id",
                keyValue: 4,
                column: "CreatedAt",
                value: new DateTime(2025, 6, 4, 13, 3, 39, 180, DateTimeKind.Utc).AddTicks(5705));

            migrationBuilder.UpdateData(
                table: "ExtraServices",
                keyColumn: "Id",
                keyValue: 5,
                column: "CreatedAt",
                value: new DateTime(2025, 6, 4, 13, 3, 39, 180, DateTimeKind.Utc).AddTicks(5709));

            migrationBuilder.UpdateData(
                table: "ExtraServices",
                keyColumn: "Id",
                keyValue: 6,
                column: "CreatedAt",
                value: new DateTime(2025, 6, 4, 13, 3, 39, 180, DateTimeKind.Utc).AddTicks(5711));

            migrationBuilder.UpdateData(
                table: "ExtraServices",
                keyColumn: "Id",
                keyValue: 7,
                column: "CreatedAt",
                value: new DateTime(2025, 6, 4, 13, 3, 39, 180, DateTimeKind.Utc).AddTicks(5715));

            migrationBuilder.UpdateData(
                table: "ExtraServices",
                keyColumn: "Id",
                keyValue: 8,
                column: "CreatedAt",
                value: new DateTime(2025, 6, 4, 13, 3, 39, 180, DateTimeKind.Utc).AddTicks(5717));

            migrationBuilder.UpdateData(
                table: "ExtraServices",
                keyColumn: "Id",
                keyValue: 9,
                column: "CreatedAt",
                value: new DateTime(2025, 6, 4, 13, 3, 39, 180, DateTimeKind.Utc).AddTicks(5719));

            migrationBuilder.UpdateData(
                table: "Frequencies",
                keyColumn: "Id",
                keyValue: 1,
                column: "CreatedAt",
                value: new DateTime(2025, 6, 4, 13, 3, 39, 180, DateTimeKind.Utc).AddTicks(5496));

            migrationBuilder.UpdateData(
                table: "Frequencies",
                keyColumn: "Id",
                keyValue: 2,
                column: "CreatedAt",
                value: new DateTime(2025, 6, 4, 13, 3, 39, 180, DateTimeKind.Utc).AddTicks(5499));

            migrationBuilder.UpdateData(
                table: "Frequencies",
                keyColumn: "Id",
                keyValue: 3,
                column: "CreatedAt",
                value: new DateTime(2025, 6, 4, 13, 3, 39, 180, DateTimeKind.Utc).AddTicks(5501));

            migrationBuilder.UpdateData(
                table: "Frequencies",
                keyColumn: "Id",
                keyValue: 4,
                column: "CreatedAt",
                value: new DateTime(2025, 6, 4, 13, 3, 39, 180, DateTimeKind.Utc).AddTicks(5560));

            migrationBuilder.UpdateData(
                table: "ServiceTypes",
                keyColumn: "Id",
                keyValue: 1,
                column: "CreatedAt",
                value: new DateTime(2025, 6, 4, 13, 3, 39, 180, DateTimeKind.Utc).AddTicks(5600));

            migrationBuilder.UpdateData(
                table: "ServiceTypes",
                keyColumn: "Id",
                keyValue: 2,
                column: "CreatedAt",
                value: new DateTime(2025, 6, 4, 13, 3, 39, 180, DateTimeKind.Utc).AddTicks(5603));

            migrationBuilder.UpdateData(
                table: "Services",
                keyColumn: "Id",
                keyValue: 1,
                column: "CreatedAt",
                value: new DateTime(2025, 6, 4, 13, 3, 39, 180, DateTimeKind.Utc).AddTicks(5632));

            migrationBuilder.UpdateData(
                table: "Services",
                keyColumn: "Id",
                keyValue: 2,
                column: "CreatedAt",
                value: new DateTime(2025, 6, 4, 13, 3, 39, 180, DateTimeKind.Utc).AddTicks(5636));

            migrationBuilder.UpdateData(
                table: "Services",
                keyColumn: "Id",
                keyValue: 3,
                column: "CreatedAt",
                value: new DateTime(2025, 6, 4, 13, 3, 39, 180, DateTimeKind.Utc).AddTicks(5640));

            migrationBuilder.UpdateData(
                table: "Services",
                keyColumn: "Id",
                keyValue: 4,
                column: "CreatedAt",
                value: new DateTime(2025, 6, 4, 13, 3, 39, 180, DateTimeKind.Utc).AddTicks(5662));

            migrationBuilder.UpdateData(
                table: "Services",
                keyColumn: "Id",
                keyValue: 5,
                column: "CreatedAt",
                value: new DateTime(2025, 6, 4, 13, 3, 39, 180, DateTimeKind.Utc).AddTicks(5666));
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.UpdateData(
                table: "ExtraServices",
                keyColumn: "Id",
                keyValue: 1,
                column: "CreatedAt",
                value: new DateTime(2025, 6, 4, 13, 2, 12, 510, DateTimeKind.Utc).AddTicks(4622));

            migrationBuilder.UpdateData(
                table: "ExtraServices",
                keyColumn: "Id",
                keyValue: 2,
                column: "CreatedAt",
                value: new DateTime(2025, 6, 4, 13, 2, 12, 510, DateTimeKind.Utc).AddTicks(4625));

            migrationBuilder.UpdateData(
                table: "ExtraServices",
                keyColumn: "Id",
                keyValue: 3,
                column: "CreatedAt",
                value: new DateTime(2025, 6, 4, 13, 2, 12, 510, DateTimeKind.Utc).AddTicks(4628));

            migrationBuilder.UpdateData(
                table: "ExtraServices",
                keyColumn: "Id",
                keyValue: 4,
                column: "CreatedAt",
                value: new DateTime(2025, 6, 4, 13, 2, 12, 510, DateTimeKind.Utc).AddTicks(4630));

            migrationBuilder.UpdateData(
                table: "ExtraServices",
                keyColumn: "Id",
                keyValue: 5,
                column: "CreatedAt",
                value: new DateTime(2025, 6, 4, 13, 2, 12, 510, DateTimeKind.Utc).AddTicks(4632));

            migrationBuilder.UpdateData(
                table: "ExtraServices",
                keyColumn: "Id",
                keyValue: 6,
                column: "CreatedAt",
                value: new DateTime(2025, 6, 4, 13, 2, 12, 510, DateTimeKind.Utc).AddTicks(4634));

            migrationBuilder.UpdateData(
                table: "ExtraServices",
                keyColumn: "Id",
                keyValue: 7,
                column: "CreatedAt",
                value: new DateTime(2025, 6, 4, 13, 2, 12, 510, DateTimeKind.Utc).AddTicks(4637));

            migrationBuilder.UpdateData(
                table: "ExtraServices",
                keyColumn: "Id",
                keyValue: 8,
                column: "CreatedAt",
                value: new DateTime(2025, 6, 4, 13, 2, 12, 510, DateTimeKind.Utc).AddTicks(4638));

            migrationBuilder.UpdateData(
                table: "ExtraServices",
                keyColumn: "Id",
                keyValue: 9,
                column: "CreatedAt",
                value: new DateTime(2025, 6, 4, 13, 2, 12, 510, DateTimeKind.Utc).AddTicks(4640));

            migrationBuilder.UpdateData(
                table: "Frequencies",
                keyColumn: "Id",
                keyValue: 1,
                column: "CreatedAt",
                value: new DateTime(2025, 6, 4, 13, 2, 12, 510, DateTimeKind.Utc).AddTicks(4449));

            migrationBuilder.UpdateData(
                table: "Frequencies",
                keyColumn: "Id",
                keyValue: 2,
                column: "CreatedAt",
                value: new DateTime(2025, 6, 4, 13, 2, 12, 510, DateTimeKind.Utc).AddTicks(4452));

            migrationBuilder.UpdateData(
                table: "Frequencies",
                keyColumn: "Id",
                keyValue: 3,
                column: "CreatedAt",
                value: new DateTime(2025, 6, 4, 13, 2, 12, 510, DateTimeKind.Utc).AddTicks(4454));

            migrationBuilder.UpdateData(
                table: "Frequencies",
                keyColumn: "Id",
                keyValue: 4,
                column: "CreatedAt",
                value: new DateTime(2025, 6, 4, 13, 2, 12, 510, DateTimeKind.Utc).AddTicks(4455));

            migrationBuilder.UpdateData(
                table: "ServiceTypes",
                keyColumn: "Id",
                keyValue: 1,
                column: "CreatedAt",
                value: new DateTime(2025, 6, 4, 13, 2, 12, 510, DateTimeKind.Utc).AddTicks(4488));

            migrationBuilder.UpdateData(
                table: "ServiceTypes",
                keyColumn: "Id",
                keyValue: 2,
                column: "CreatedAt",
                value: new DateTime(2025, 6, 4, 13, 2, 12, 510, DateTimeKind.Utc).AddTicks(4490));

            migrationBuilder.UpdateData(
                table: "Services",
                keyColumn: "Id",
                keyValue: 1,
                column: "CreatedAt",
                value: new DateTime(2025, 6, 4, 13, 2, 12, 510, DateTimeKind.Utc).AddTicks(4523));

            migrationBuilder.UpdateData(
                table: "Services",
                keyColumn: "Id",
                keyValue: 2,
                column: "CreatedAt",
                value: new DateTime(2025, 6, 4, 13, 2, 12, 510, DateTimeKind.Utc).AddTicks(4526));

            migrationBuilder.UpdateData(
                table: "Services",
                keyColumn: "Id",
                keyValue: 3,
                column: "CreatedAt",
                value: new DateTime(2025, 6, 4, 13, 2, 12, 510, DateTimeKind.Utc).AddTicks(4529));

            migrationBuilder.UpdateData(
                table: "Services",
                keyColumn: "Id",
                keyValue: 4,
                column: "CreatedAt",
                value: new DateTime(2025, 6, 4, 13, 2, 12, 510, DateTimeKind.Utc).AddTicks(4550));

            migrationBuilder.UpdateData(
                table: "Services",
                keyColumn: "Id",
                keyValue: 5,
                column: "CreatedAt",
                value: new DateTime(2025, 6, 4, 13, 2, 12, 510, DateTimeKind.Utc).AddTicks(4593));
        }
    }
}
