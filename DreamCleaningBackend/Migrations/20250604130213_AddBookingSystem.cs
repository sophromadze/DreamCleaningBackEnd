using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DreamCleaningBackend.Migrations
{
    /// <inheritdoc />
    public partial class AddBookingSystem : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
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

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.UpdateData(
                table: "ExtraServices",
                keyColumn: "Id",
                keyValue: 1,
                column: "CreatedAt",
                value: new DateTime(2025, 6, 4, 12, 57, 25, 900, DateTimeKind.Utc).AddTicks(4370));

            migrationBuilder.UpdateData(
                table: "ExtraServices",
                keyColumn: "Id",
                keyValue: 2,
                column: "CreatedAt",
                value: new DateTime(2025, 6, 4, 12, 57, 25, 900, DateTimeKind.Utc).AddTicks(4374));

            migrationBuilder.UpdateData(
                table: "ExtraServices",
                keyColumn: "Id",
                keyValue: 3,
                column: "CreatedAt",
                value: new DateTime(2025, 6, 4, 12, 57, 25, 900, DateTimeKind.Utc).AddTicks(4376));

            migrationBuilder.UpdateData(
                table: "ExtraServices",
                keyColumn: "Id",
                keyValue: 4,
                column: "CreatedAt",
                value: new DateTime(2025, 6, 4, 12, 57, 25, 900, DateTimeKind.Utc).AddTicks(4379));

            migrationBuilder.UpdateData(
                table: "ExtraServices",
                keyColumn: "Id",
                keyValue: 5,
                column: "CreatedAt",
                value: new DateTime(2025, 6, 4, 12, 57, 25, 900, DateTimeKind.Utc).AddTicks(4381));

            migrationBuilder.UpdateData(
                table: "ExtraServices",
                keyColumn: "Id",
                keyValue: 6,
                column: "CreatedAt",
                value: new DateTime(2025, 6, 4, 12, 57, 25, 900, DateTimeKind.Utc).AddTicks(4383));

            migrationBuilder.UpdateData(
                table: "ExtraServices",
                keyColumn: "Id",
                keyValue: 7,
                column: "CreatedAt",
                value: new DateTime(2025, 6, 4, 12, 57, 25, 900, DateTimeKind.Utc).AddTicks(4385));

            migrationBuilder.UpdateData(
                table: "ExtraServices",
                keyColumn: "Id",
                keyValue: 8,
                column: "CreatedAt",
                value: new DateTime(2025, 6, 4, 12, 57, 25, 900, DateTimeKind.Utc).AddTicks(4387));

            migrationBuilder.UpdateData(
                table: "ExtraServices",
                keyColumn: "Id",
                keyValue: 9,
                column: "CreatedAt",
                value: new DateTime(2025, 6, 4, 12, 57, 25, 900, DateTimeKind.Utc).AddTicks(4389));

            migrationBuilder.UpdateData(
                table: "Frequencies",
                keyColumn: "Id",
                keyValue: 1,
                column: "CreatedAt",
                value: new DateTime(2025, 6, 4, 12, 57, 25, 900, DateTimeKind.Utc).AddTicks(4241));

            migrationBuilder.UpdateData(
                table: "Frequencies",
                keyColumn: "Id",
                keyValue: 2,
                column: "CreatedAt",
                value: new DateTime(2025, 6, 4, 12, 57, 25, 900, DateTimeKind.Utc).AddTicks(4244));

            migrationBuilder.UpdateData(
                table: "Frequencies",
                keyColumn: "Id",
                keyValue: 3,
                column: "CreatedAt",
                value: new DateTime(2025, 6, 4, 12, 57, 25, 900, DateTimeKind.Utc).AddTicks(4246));

            migrationBuilder.UpdateData(
                table: "Frequencies",
                keyColumn: "Id",
                keyValue: 4,
                column: "CreatedAt",
                value: new DateTime(2025, 6, 4, 12, 57, 25, 900, DateTimeKind.Utc).AddTicks(4248));

            migrationBuilder.UpdateData(
                table: "ServiceTypes",
                keyColumn: "Id",
                keyValue: 1,
                column: "CreatedAt",
                value: new DateTime(2025, 6, 4, 12, 57, 25, 900, DateTimeKind.Utc).AddTicks(4284));

            migrationBuilder.UpdateData(
                table: "ServiceTypes",
                keyColumn: "Id",
                keyValue: 2,
                column: "CreatedAt",
                value: new DateTime(2025, 6, 4, 12, 57, 25, 900, DateTimeKind.Utc).AddTicks(4286));

            migrationBuilder.UpdateData(
                table: "Services",
                keyColumn: "Id",
                keyValue: 1,
                column: "CreatedAt",
                value: new DateTime(2025, 6, 4, 12, 57, 25, 900, DateTimeKind.Utc).AddTicks(4314));

            migrationBuilder.UpdateData(
                table: "Services",
                keyColumn: "Id",
                keyValue: 2,
                column: "CreatedAt",
                value: new DateTime(2025, 6, 4, 12, 57, 25, 900, DateTimeKind.Utc).AddTicks(4318));

            migrationBuilder.UpdateData(
                table: "Services",
                keyColumn: "Id",
                keyValue: 3,
                column: "CreatedAt",
                value: new DateTime(2025, 6, 4, 12, 57, 25, 900, DateTimeKind.Utc).AddTicks(4322));

            migrationBuilder.UpdateData(
                table: "Services",
                keyColumn: "Id",
                keyValue: 4,
                column: "CreatedAt",
                value: new DateTime(2025, 6, 4, 12, 57, 25, 900, DateTimeKind.Utc).AddTicks(4342));

            migrationBuilder.UpdateData(
                table: "Services",
                keyColumn: "Id",
                keyValue: 5,
                column: "CreatedAt",
                value: new DateTime(2025, 6, 4, 12, 57, 25, 900, DateTimeKind.Utc).AddTicks(4344));
        }
    }
}
