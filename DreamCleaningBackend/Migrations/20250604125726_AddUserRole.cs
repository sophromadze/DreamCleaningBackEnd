using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DreamCleaningBackend.Migrations
{
    /// <inheritdoc />
    public partial class AddUserRole : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "Role",
                table: "Users",
                type: "int",
                nullable: false,
                defaultValue: 0);

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

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Role",
                table: "Users");

            migrationBuilder.UpdateData(
                table: "ExtraServices",
                keyColumn: "Id",
                keyValue: 1,
                column: "CreatedAt",
                value: new DateTime(2025, 6, 4, 10, 22, 21, 581, DateTimeKind.Utc).AddTicks(5625));

            migrationBuilder.UpdateData(
                table: "ExtraServices",
                keyColumn: "Id",
                keyValue: 2,
                column: "CreatedAt",
                value: new DateTime(2025, 6, 4, 10, 22, 21, 581, DateTimeKind.Utc).AddTicks(5629));

            migrationBuilder.UpdateData(
                table: "ExtraServices",
                keyColumn: "Id",
                keyValue: 3,
                column: "CreatedAt",
                value: new DateTime(2025, 6, 4, 10, 22, 21, 581, DateTimeKind.Utc).AddTicks(5632));

            migrationBuilder.UpdateData(
                table: "ExtraServices",
                keyColumn: "Id",
                keyValue: 4,
                column: "CreatedAt",
                value: new DateTime(2025, 6, 4, 10, 22, 21, 581, DateTimeKind.Utc).AddTicks(5635));

            migrationBuilder.UpdateData(
                table: "ExtraServices",
                keyColumn: "Id",
                keyValue: 5,
                column: "CreatedAt",
                value: new DateTime(2025, 6, 4, 10, 22, 21, 581, DateTimeKind.Utc).AddTicks(5678));

            migrationBuilder.UpdateData(
                table: "ExtraServices",
                keyColumn: "Id",
                keyValue: 6,
                column: "CreatedAt",
                value: new DateTime(2025, 6, 4, 10, 22, 21, 581, DateTimeKind.Utc).AddTicks(5680));

            migrationBuilder.UpdateData(
                table: "ExtraServices",
                keyColumn: "Id",
                keyValue: 7,
                column: "CreatedAt",
                value: new DateTime(2025, 6, 4, 10, 22, 21, 581, DateTimeKind.Utc).AddTicks(5683));

            migrationBuilder.UpdateData(
                table: "ExtraServices",
                keyColumn: "Id",
                keyValue: 8,
                column: "CreatedAt",
                value: new DateTime(2025, 6, 4, 10, 22, 21, 581, DateTimeKind.Utc).AddTicks(5685));

            migrationBuilder.UpdateData(
                table: "ExtraServices",
                keyColumn: "Id",
                keyValue: 9,
                column: "CreatedAt",
                value: new DateTime(2025, 6, 4, 10, 22, 21, 581, DateTimeKind.Utc).AddTicks(5687));

            migrationBuilder.UpdateData(
                table: "Frequencies",
                keyColumn: "Id",
                keyValue: 1,
                column: "CreatedAt",
                value: new DateTime(2025, 6, 4, 10, 22, 21, 581, DateTimeKind.Utc).AddTicks(5497));

            migrationBuilder.UpdateData(
                table: "Frequencies",
                keyColumn: "Id",
                keyValue: 2,
                column: "CreatedAt",
                value: new DateTime(2025, 6, 4, 10, 22, 21, 581, DateTimeKind.Utc).AddTicks(5500));

            migrationBuilder.UpdateData(
                table: "Frequencies",
                keyColumn: "Id",
                keyValue: 3,
                column: "CreatedAt",
                value: new DateTime(2025, 6, 4, 10, 22, 21, 581, DateTimeKind.Utc).AddTicks(5502));

            migrationBuilder.UpdateData(
                table: "Frequencies",
                keyColumn: "Id",
                keyValue: 4,
                column: "CreatedAt",
                value: new DateTime(2025, 6, 4, 10, 22, 21, 581, DateTimeKind.Utc).AddTicks(5504));

            migrationBuilder.UpdateData(
                table: "ServiceTypes",
                keyColumn: "Id",
                keyValue: 1,
                column: "CreatedAt",
                value: new DateTime(2025, 6, 4, 10, 22, 21, 581, DateTimeKind.Utc).AddTicks(5536));

            migrationBuilder.UpdateData(
                table: "ServiceTypes",
                keyColumn: "Id",
                keyValue: 2,
                column: "CreatedAt",
                value: new DateTime(2025, 6, 4, 10, 22, 21, 581, DateTimeKind.Utc).AddTicks(5539));

            migrationBuilder.UpdateData(
                table: "Services",
                keyColumn: "Id",
                keyValue: 1,
                column: "CreatedAt",
                value: new DateTime(2025, 6, 4, 10, 22, 21, 581, DateTimeKind.Utc).AddTicks(5569));

            migrationBuilder.UpdateData(
                table: "Services",
                keyColumn: "Id",
                keyValue: 2,
                column: "CreatedAt",
                value: new DateTime(2025, 6, 4, 10, 22, 21, 581, DateTimeKind.Utc).AddTicks(5573));

            migrationBuilder.UpdateData(
                table: "Services",
                keyColumn: "Id",
                keyValue: 3,
                column: "CreatedAt",
                value: new DateTime(2025, 6, 4, 10, 22, 21, 581, DateTimeKind.Utc).AddTicks(5577));

            migrationBuilder.UpdateData(
                table: "Services",
                keyColumn: "Id",
                keyValue: 4,
                column: "CreatedAt",
                value: new DateTime(2025, 6, 4, 10, 22, 21, 581, DateTimeKind.Utc).AddTicks(5598));

            migrationBuilder.UpdateData(
                table: "Services",
                keyColumn: "Id",
                keyValue: 5,
                column: "CreatedAt",
                value: new DateTime(2025, 6, 4, 10, 22, 21, 581, DateTimeKind.Utc).AddTicks(5601));
        }
    }
}
