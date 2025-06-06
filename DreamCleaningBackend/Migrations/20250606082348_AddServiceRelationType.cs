using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DreamCleaningBackend.Migrations
{
    /// <inheritdoc />
    public partial class AddServiceRelationType : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "ServiceRelationType",
                table: "Services",
                type: "nvarchar(20)",
                maxLength: 20,
                nullable: true);

            migrationBuilder.UpdateData(
                table: "ExtraServices",
                keyColumn: "Id",
                keyValue: 1,
                column: "CreatedAt",
                value: new DateTime(2025, 6, 6, 8, 23, 47, 622, DateTimeKind.Utc).AddTicks(7088));

            migrationBuilder.UpdateData(
                table: "ExtraServices",
                keyColumn: "Id",
                keyValue: 2,
                column: "CreatedAt",
                value: new DateTime(2025, 6, 6, 8, 23, 47, 622, DateTimeKind.Utc).AddTicks(7092));

            migrationBuilder.UpdateData(
                table: "ExtraServices",
                keyColumn: "Id",
                keyValue: 3,
                column: "CreatedAt",
                value: new DateTime(2025, 6, 6, 8, 23, 47, 622, DateTimeKind.Utc).AddTicks(7094));

            migrationBuilder.UpdateData(
                table: "ExtraServices",
                keyColumn: "Id",
                keyValue: 4,
                column: "CreatedAt",
                value: new DateTime(2025, 6, 6, 8, 23, 47, 622, DateTimeKind.Utc).AddTicks(7097));

            migrationBuilder.UpdateData(
                table: "ExtraServices",
                keyColumn: "Id",
                keyValue: 5,
                column: "CreatedAt",
                value: new DateTime(2025, 6, 6, 8, 23, 47, 622, DateTimeKind.Utc).AddTicks(7099));

            migrationBuilder.UpdateData(
                table: "ExtraServices",
                keyColumn: "Id",
                keyValue: 6,
                column: "CreatedAt",
                value: new DateTime(2025, 6, 6, 8, 23, 47, 622, DateTimeKind.Utc).AddTicks(7101));

            migrationBuilder.UpdateData(
                table: "ExtraServices",
                keyColumn: "Id",
                keyValue: 7,
                column: "CreatedAt",
                value: new DateTime(2025, 6, 6, 8, 23, 47, 622, DateTimeKind.Utc).AddTicks(7104));

            migrationBuilder.UpdateData(
                table: "ExtraServices",
                keyColumn: "Id",
                keyValue: 8,
                column: "CreatedAt",
                value: new DateTime(2025, 6, 6, 8, 23, 47, 622, DateTimeKind.Utc).AddTicks(7106));

            migrationBuilder.UpdateData(
                table: "ExtraServices",
                keyColumn: "Id",
                keyValue: 9,
                column: "CreatedAt",
                value: new DateTime(2025, 6, 6, 8, 23, 47, 622, DateTimeKind.Utc).AddTicks(7108));

            migrationBuilder.UpdateData(
                table: "Frequencies",
                keyColumn: "Id",
                keyValue: 1,
                column: "CreatedAt",
                value: new DateTime(2025, 6, 6, 8, 23, 47, 622, DateTimeKind.Utc).AddTicks(6946));

            migrationBuilder.UpdateData(
                table: "Frequencies",
                keyColumn: "Id",
                keyValue: 2,
                column: "CreatedAt",
                value: new DateTime(2025, 6, 6, 8, 23, 47, 622, DateTimeKind.Utc).AddTicks(6950));

            migrationBuilder.UpdateData(
                table: "Frequencies",
                keyColumn: "Id",
                keyValue: 3,
                column: "CreatedAt",
                value: new DateTime(2025, 6, 6, 8, 23, 47, 622, DateTimeKind.Utc).AddTicks(6951));

            migrationBuilder.UpdateData(
                table: "Frequencies",
                keyColumn: "Id",
                keyValue: 4,
                column: "CreatedAt",
                value: new DateTime(2025, 6, 6, 8, 23, 47, 622, DateTimeKind.Utc).AddTicks(6954));

            migrationBuilder.UpdateData(
                table: "ServiceTypes",
                keyColumn: "Id",
                keyValue: 1,
                column: "CreatedAt",
                value: new DateTime(2025, 6, 6, 8, 23, 47, 622, DateTimeKind.Utc).AddTicks(6989));

            migrationBuilder.UpdateData(
                table: "ServiceTypes",
                keyColumn: "Id",
                keyValue: 2,
                column: "CreatedAt",
                value: new DateTime(2025, 6, 6, 8, 23, 47, 622, DateTimeKind.Utc).AddTicks(6992));

            migrationBuilder.UpdateData(
                table: "Services",
                keyColumn: "Id",
                keyValue: 1,
                columns: new[] { "CreatedAt", "ServiceRelationType" },
                values: new object[] { new DateTime(2025, 6, 6, 8, 23, 47, 622, DateTimeKind.Utc).AddTicks(7024), null });

            migrationBuilder.UpdateData(
                table: "Services",
                keyColumn: "Id",
                keyValue: 2,
                columns: new[] { "CreatedAt", "ServiceRelationType" },
                values: new object[] { new DateTime(2025, 6, 6, 8, 23, 47, 622, DateTimeKind.Utc).AddTicks(7028), null });

            migrationBuilder.UpdateData(
                table: "Services",
                keyColumn: "Id",
                keyValue: 3,
                columns: new[] { "CreatedAt", "ServiceRelationType" },
                values: new object[] { new DateTime(2025, 6, 6, 8, 23, 47, 622, DateTimeKind.Utc).AddTicks(7032), null });

            migrationBuilder.UpdateData(
                table: "Services",
                keyColumn: "Id",
                keyValue: 4,
                columns: new[] { "CreatedAt", "ServiceRelationType" },
                values: new object[] { new DateTime(2025, 6, 6, 8, 23, 47, 622, DateTimeKind.Utc).AddTicks(7055), "cleaner" });

            migrationBuilder.UpdateData(
                table: "Services",
                keyColumn: "Id",
                keyValue: 5,
                columns: new[] { "CreatedAt", "ServiceRelationType" },
                values: new object[] { new DateTime(2025, 6, 6, 8, 23, 47, 622, DateTimeKind.Utc).AddTicks(7058), "hours" });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ServiceRelationType",
                table: "Services");

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
    }
}
