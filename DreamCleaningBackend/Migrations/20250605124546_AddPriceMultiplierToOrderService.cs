using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DreamCleaningBackend.Migrations
{
    /// <inheritdoc />
    public partial class AddPriceMultiplierToOrderService : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.UpdateData(
                table: "ExtraServices",
                keyColumn: "Id",
                keyValue: 1,
                column: "CreatedAt",
                value: new DateTime(2025, 6, 5, 12, 45, 45, 423, DateTimeKind.Utc).AddTicks(572));

            migrationBuilder.UpdateData(
                table: "ExtraServices",
                keyColumn: "Id",
                keyValue: 2,
                column: "CreatedAt",
                value: new DateTime(2025, 6, 5, 12, 45, 45, 423, DateTimeKind.Utc).AddTicks(577));

            migrationBuilder.UpdateData(
                table: "ExtraServices",
                keyColumn: "Id",
                keyValue: 3,
                column: "CreatedAt",
                value: new DateTime(2025, 6, 5, 12, 45, 45, 423, DateTimeKind.Utc).AddTicks(620));

            migrationBuilder.UpdateData(
                table: "ExtraServices",
                keyColumn: "Id",
                keyValue: 4,
                column: "CreatedAt",
                value: new DateTime(2025, 6, 5, 12, 45, 45, 423, DateTimeKind.Utc).AddTicks(623));

            migrationBuilder.UpdateData(
                table: "ExtraServices",
                keyColumn: "Id",
                keyValue: 5,
                column: "CreatedAt",
                value: new DateTime(2025, 6, 5, 12, 45, 45, 423, DateTimeKind.Utc).AddTicks(625));

            migrationBuilder.UpdateData(
                table: "ExtraServices",
                keyColumn: "Id",
                keyValue: 6,
                column: "CreatedAt",
                value: new DateTime(2025, 6, 5, 12, 45, 45, 423, DateTimeKind.Utc).AddTicks(627));

            migrationBuilder.UpdateData(
                table: "ExtraServices",
                keyColumn: "Id",
                keyValue: 7,
                column: "CreatedAt",
                value: new DateTime(2025, 6, 5, 12, 45, 45, 423, DateTimeKind.Utc).AddTicks(630));

            migrationBuilder.UpdateData(
                table: "ExtraServices",
                keyColumn: "Id",
                keyValue: 8,
                column: "CreatedAt",
                value: new DateTime(2025, 6, 5, 12, 45, 45, 423, DateTimeKind.Utc).AddTicks(631));

            migrationBuilder.UpdateData(
                table: "ExtraServices",
                keyColumn: "Id",
                keyValue: 9,
                column: "CreatedAt",
                value: new DateTime(2025, 6, 5, 12, 45, 45, 423, DateTimeKind.Utc).AddTicks(633));

            migrationBuilder.UpdateData(
                table: "Frequencies",
                keyColumn: "Id",
                keyValue: 1,
                column: "CreatedAt",
                value: new DateTime(2025, 6, 5, 12, 45, 45, 423, DateTimeKind.Utc).AddTicks(437));

            migrationBuilder.UpdateData(
                table: "Frequencies",
                keyColumn: "Id",
                keyValue: 2,
                column: "CreatedAt",
                value: new DateTime(2025, 6, 5, 12, 45, 45, 423, DateTimeKind.Utc).AddTicks(441));

            migrationBuilder.UpdateData(
                table: "Frequencies",
                keyColumn: "Id",
                keyValue: 3,
                column: "CreatedAt",
                value: new DateTime(2025, 6, 5, 12, 45, 45, 423, DateTimeKind.Utc).AddTicks(443));

            migrationBuilder.UpdateData(
                table: "Frequencies",
                keyColumn: "Id",
                keyValue: 4,
                column: "CreatedAt",
                value: new DateTime(2025, 6, 5, 12, 45, 45, 423, DateTimeKind.Utc).AddTicks(444));

            migrationBuilder.UpdateData(
                table: "ServiceTypes",
                keyColumn: "Id",
                keyValue: 1,
                column: "CreatedAt",
                value: new DateTime(2025, 6, 5, 12, 45, 45, 423, DateTimeKind.Utc).AddTicks(481));

            migrationBuilder.UpdateData(
                table: "ServiceTypes",
                keyColumn: "Id",
                keyValue: 2,
                column: "CreatedAt",
                value: new DateTime(2025, 6, 5, 12, 45, 45, 423, DateTimeKind.Utc).AddTicks(484));

            migrationBuilder.UpdateData(
                table: "Services",
                keyColumn: "Id",
                keyValue: 1,
                column: "CreatedAt",
                value: new DateTime(2025, 6, 5, 12, 45, 45, 423, DateTimeKind.Utc).AddTicks(513));

            migrationBuilder.UpdateData(
                table: "Services",
                keyColumn: "Id",
                keyValue: 2,
                column: "CreatedAt",
                value: new DateTime(2025, 6, 5, 12, 45, 45, 423, DateTimeKind.Utc).AddTicks(516));

            migrationBuilder.UpdateData(
                table: "Services",
                keyColumn: "Id",
                keyValue: 3,
                column: "CreatedAt",
                value: new DateTime(2025, 6, 5, 12, 45, 45, 423, DateTimeKind.Utc).AddTicks(520));

            migrationBuilder.UpdateData(
                table: "Services",
                keyColumn: "Id",
                keyValue: 4,
                column: "CreatedAt",
                value: new DateTime(2025, 6, 5, 12, 45, 45, 423, DateTimeKind.Utc).AddTicks(543));

            migrationBuilder.UpdateData(
                table: "Services",
                keyColumn: "Id",
                keyValue: 5,
                column: "CreatedAt",
                value: new DateTime(2025, 6, 5, 12, 45, 45, 423, DateTimeKind.Utc).AddTicks(545));
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
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
    }
}
