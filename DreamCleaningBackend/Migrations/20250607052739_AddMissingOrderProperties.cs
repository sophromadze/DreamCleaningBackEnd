using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DreamCleaningBackend.Migrations
{
    /// <inheritdoc />
    public partial class AddMissingOrderProperties : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "ServiceName",
                table: "OrderServices",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AlterColumn<string>(
                name: "ServiceTime",
                table: "Orders",
                type: "nvarchar(max)",
                nullable: false,
                oldClrType: typeof(TimeSpan),
                oldType: "time");

            migrationBuilder.AddColumn<string>(
                name: "FrequencyName",
                table: "Orders",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "ServiceTypeName",
                table: "Orders",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AlterColumn<int>(
                name: "Hours",
                table: "OrderExtraServices",
                type: "int",
                nullable: false,
                oldClrType: typeof(decimal),
                oldType: "decimal(18,2)");

            migrationBuilder.AddColumn<string>(
                name: "ExtraServiceName",
                table: "OrderExtraServices",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.UpdateData(
                table: "ExtraServices",
                keyColumn: "Id",
                keyValue: 1,
                column: "CreatedAt",
                value: new DateTime(2025, 6, 7, 5, 27, 38, 566, DateTimeKind.Utc).AddTicks(4909));

            migrationBuilder.UpdateData(
                table: "ExtraServices",
                keyColumn: "Id",
                keyValue: 2,
                column: "CreatedAt",
                value: new DateTime(2025, 6, 7, 5, 27, 38, 566, DateTimeKind.Utc).AddTicks(4913));

            migrationBuilder.UpdateData(
                table: "ExtraServices",
                keyColumn: "Id",
                keyValue: 3,
                column: "CreatedAt",
                value: new DateTime(2025, 6, 7, 5, 27, 38, 566, DateTimeKind.Utc).AddTicks(4915));

            migrationBuilder.UpdateData(
                table: "ExtraServices",
                keyColumn: "Id",
                keyValue: 4,
                column: "CreatedAt",
                value: new DateTime(2025, 6, 7, 5, 27, 38, 566, DateTimeKind.Utc).AddTicks(4918));

            migrationBuilder.UpdateData(
                table: "ExtraServices",
                keyColumn: "Id",
                keyValue: 5,
                column: "CreatedAt",
                value: new DateTime(2025, 6, 7, 5, 27, 38, 566, DateTimeKind.Utc).AddTicks(4920));

            migrationBuilder.UpdateData(
                table: "ExtraServices",
                keyColumn: "Id",
                keyValue: 6,
                column: "CreatedAt",
                value: new DateTime(2025, 6, 7, 5, 27, 38, 566, DateTimeKind.Utc).AddTicks(4922));

            migrationBuilder.UpdateData(
                table: "ExtraServices",
                keyColumn: "Id",
                keyValue: 7,
                column: "CreatedAt",
                value: new DateTime(2025, 6, 7, 5, 27, 38, 566, DateTimeKind.Utc).AddTicks(4924));

            migrationBuilder.UpdateData(
                table: "ExtraServices",
                keyColumn: "Id",
                keyValue: 8,
                column: "CreatedAt",
                value: new DateTime(2025, 6, 7, 5, 27, 38, 566, DateTimeKind.Utc).AddTicks(4926));

            migrationBuilder.UpdateData(
                table: "ExtraServices",
                keyColumn: "Id",
                keyValue: 9,
                column: "CreatedAt",
                value: new DateTime(2025, 6, 7, 5, 27, 38, 566, DateTimeKind.Utc).AddTicks(4930));

            migrationBuilder.UpdateData(
                table: "Frequencies",
                keyColumn: "Id",
                keyValue: 1,
                column: "CreatedAt",
                value: new DateTime(2025, 6, 7, 5, 27, 38, 566, DateTimeKind.Utc).AddTicks(4735));

            migrationBuilder.UpdateData(
                table: "Frequencies",
                keyColumn: "Id",
                keyValue: 2,
                column: "CreatedAt",
                value: new DateTime(2025, 6, 7, 5, 27, 38, 566, DateTimeKind.Utc).AddTicks(4739));

            migrationBuilder.UpdateData(
                table: "Frequencies",
                keyColumn: "Id",
                keyValue: 3,
                column: "CreatedAt",
                value: new DateTime(2025, 6, 7, 5, 27, 38, 566, DateTimeKind.Utc).AddTicks(4741));

            migrationBuilder.UpdateData(
                table: "Frequencies",
                keyColumn: "Id",
                keyValue: 4,
                column: "CreatedAt",
                value: new DateTime(2025, 6, 7, 5, 27, 38, 566, DateTimeKind.Utc).AddTicks(4742));

            migrationBuilder.UpdateData(
                table: "ServiceTypes",
                keyColumn: "Id",
                keyValue: 1,
                column: "CreatedAt",
                value: new DateTime(2025, 6, 7, 5, 27, 38, 566, DateTimeKind.Utc).AddTicks(4776));

            migrationBuilder.UpdateData(
                table: "ServiceTypes",
                keyColumn: "Id",
                keyValue: 2,
                column: "CreatedAt",
                value: new DateTime(2025, 6, 7, 5, 27, 38, 566, DateTimeKind.Utc).AddTicks(4778));

            migrationBuilder.UpdateData(
                table: "Services",
                keyColumn: "Id",
                keyValue: 1,
                column: "CreatedAt",
                value: new DateTime(2025, 6, 7, 5, 27, 38, 566, DateTimeKind.Utc).AddTicks(4847));

            migrationBuilder.UpdateData(
                table: "Services",
                keyColumn: "Id",
                keyValue: 2,
                column: "CreatedAt",
                value: new DateTime(2025, 6, 7, 5, 27, 38, 566, DateTimeKind.Utc).AddTicks(4854));

            migrationBuilder.UpdateData(
                table: "Services",
                keyColumn: "Id",
                keyValue: 3,
                column: "CreatedAt",
                value: new DateTime(2025, 6, 7, 5, 27, 38, 566, DateTimeKind.Utc).AddTicks(4858));

            migrationBuilder.UpdateData(
                table: "Services",
                keyColumn: "Id",
                keyValue: 4,
                column: "CreatedAt",
                value: new DateTime(2025, 6, 7, 5, 27, 38, 566, DateTimeKind.Utc).AddTicks(4878));

            migrationBuilder.UpdateData(
                table: "Services",
                keyColumn: "Id",
                keyValue: 5,
                column: "CreatedAt",
                value: new DateTime(2025, 6, 7, 5, 27, 38, 566, DateTimeKind.Utc).AddTicks(4881));
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ServiceName",
                table: "OrderServices");

            migrationBuilder.DropColumn(
                name: "FrequencyName",
                table: "Orders");

            migrationBuilder.DropColumn(
                name: "ServiceTypeName",
                table: "Orders");

            migrationBuilder.DropColumn(
                name: "ExtraServiceName",
                table: "OrderExtraServices");

            migrationBuilder.AlterColumn<TimeSpan>(
                name: "ServiceTime",
                table: "Orders",
                type: "time",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");

            migrationBuilder.AlterColumn<decimal>(
                name: "Hours",
                table: "OrderExtraServices",
                type: "decimal(18,2)",
                nullable: false,
                oldClrType: typeof(int),
                oldType: "int");

            migrationBuilder.UpdateData(
                table: "ExtraServices",
                keyColumn: "Id",
                keyValue: 1,
                column: "CreatedAt",
                value: new DateTime(2025, 6, 6, 14, 19, 1, 586, DateTimeKind.Utc).AddTicks(2552));

            migrationBuilder.UpdateData(
                table: "ExtraServices",
                keyColumn: "Id",
                keyValue: 2,
                column: "CreatedAt",
                value: new DateTime(2025, 6, 6, 14, 19, 1, 586, DateTimeKind.Utc).AddTicks(2557));

            migrationBuilder.UpdateData(
                table: "ExtraServices",
                keyColumn: "Id",
                keyValue: 3,
                column: "CreatedAt",
                value: new DateTime(2025, 6, 6, 14, 19, 1, 586, DateTimeKind.Utc).AddTicks(2560));

            migrationBuilder.UpdateData(
                table: "ExtraServices",
                keyColumn: "Id",
                keyValue: 4,
                column: "CreatedAt",
                value: new DateTime(2025, 6, 6, 14, 19, 1, 586, DateTimeKind.Utc).AddTicks(2562));

            migrationBuilder.UpdateData(
                table: "ExtraServices",
                keyColumn: "Id",
                keyValue: 5,
                column: "CreatedAt",
                value: new DateTime(2025, 6, 6, 14, 19, 1, 586, DateTimeKind.Utc).AddTicks(2631));

            migrationBuilder.UpdateData(
                table: "ExtraServices",
                keyColumn: "Id",
                keyValue: 6,
                column: "CreatedAt",
                value: new DateTime(2025, 6, 6, 14, 19, 1, 586, DateTimeKind.Utc).AddTicks(2634));

            migrationBuilder.UpdateData(
                table: "ExtraServices",
                keyColumn: "Id",
                keyValue: 7,
                column: "CreatedAt",
                value: new DateTime(2025, 6, 6, 14, 19, 1, 586, DateTimeKind.Utc).AddTicks(2636));

            migrationBuilder.UpdateData(
                table: "ExtraServices",
                keyColumn: "Id",
                keyValue: 8,
                column: "CreatedAt",
                value: new DateTime(2025, 6, 6, 14, 19, 1, 586, DateTimeKind.Utc).AddTicks(2638));

            migrationBuilder.UpdateData(
                table: "ExtraServices",
                keyColumn: "Id",
                keyValue: 9,
                column: "CreatedAt",
                value: new DateTime(2025, 6, 6, 14, 19, 1, 586, DateTimeKind.Utc).AddTicks(2640));

            migrationBuilder.UpdateData(
                table: "Frequencies",
                keyColumn: "Id",
                keyValue: 1,
                column: "CreatedAt",
                value: new DateTime(2025, 6, 6, 14, 19, 1, 586, DateTimeKind.Utc).AddTicks(2416));

            migrationBuilder.UpdateData(
                table: "Frequencies",
                keyColumn: "Id",
                keyValue: 2,
                column: "CreatedAt",
                value: new DateTime(2025, 6, 6, 14, 19, 1, 586, DateTimeKind.Utc).AddTicks(2419));

            migrationBuilder.UpdateData(
                table: "Frequencies",
                keyColumn: "Id",
                keyValue: 3,
                column: "CreatedAt",
                value: new DateTime(2025, 6, 6, 14, 19, 1, 586, DateTimeKind.Utc).AddTicks(2421));

            migrationBuilder.UpdateData(
                table: "Frequencies",
                keyColumn: "Id",
                keyValue: 4,
                column: "CreatedAt",
                value: new DateTime(2025, 6, 6, 14, 19, 1, 586, DateTimeKind.Utc).AddTicks(2423));

            migrationBuilder.UpdateData(
                table: "ServiceTypes",
                keyColumn: "Id",
                keyValue: 1,
                column: "CreatedAt",
                value: new DateTime(2025, 6, 6, 14, 19, 1, 586, DateTimeKind.Utc).AddTicks(2457));

            migrationBuilder.UpdateData(
                table: "ServiceTypes",
                keyColumn: "Id",
                keyValue: 2,
                column: "CreatedAt",
                value: new DateTime(2025, 6, 6, 14, 19, 1, 586, DateTimeKind.Utc).AddTicks(2460));

            migrationBuilder.UpdateData(
                table: "Services",
                keyColumn: "Id",
                keyValue: 1,
                column: "CreatedAt",
                value: new DateTime(2025, 6, 6, 14, 19, 1, 586, DateTimeKind.Utc).AddTicks(2490));

            migrationBuilder.UpdateData(
                table: "Services",
                keyColumn: "Id",
                keyValue: 2,
                column: "CreatedAt",
                value: new DateTime(2025, 6, 6, 14, 19, 1, 586, DateTimeKind.Utc).AddTicks(2494));

            migrationBuilder.UpdateData(
                table: "Services",
                keyColumn: "Id",
                keyValue: 3,
                column: "CreatedAt",
                value: new DateTime(2025, 6, 6, 14, 19, 1, 586, DateTimeKind.Utc).AddTicks(2498));

            migrationBuilder.UpdateData(
                table: "Services",
                keyColumn: "Id",
                keyValue: 4,
                column: "CreatedAt",
                value: new DateTime(2025, 6, 6, 14, 19, 1, 586, DateTimeKind.Utc).AddTicks(2521));

            migrationBuilder.UpdateData(
                table: "Services",
                keyColumn: "Id",
                keyValue: 5,
                column: "CreatedAt",
                value: new DateTime(2025, 6, 6, 14, 19, 1, 586, DateTimeKind.Utc).AddTicks(2523));
        }
    }
}
