using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DreamCleaningBackend.Migrations
{
    /// <inheritdoc />
    public partial class UpdateGiftCardStructure : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_GiftCards_Users_UsedByUserId",
                table: "GiftCards");

            migrationBuilder.DropIndex(
                name: "IX_GiftCards_UsedByUserId",
                table: "GiftCards");

            migrationBuilder.DropColumn(
                name: "IsUsed",
                table: "GiftCards");

            migrationBuilder.DropColumn(
                name: "UsedAt",
                table: "GiftCards");

            migrationBuilder.DropColumn(
                name: "UsedByUserId",
                table: "GiftCards");

            migrationBuilder.AddColumn<int>(
                name: "UserId",
                table: "GiftCardUsages",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.UpdateData(
                table: "ExtraServices",
                keyColumn: "Id",
                keyValue: 1,
                column: "CreatedAt",
                value: new DateTime(2025, 6, 14, 14, 34, 1, 707, DateTimeKind.Utc).AddTicks(5767));

            migrationBuilder.UpdateData(
                table: "ExtraServices",
                keyColumn: "Id",
                keyValue: 2,
                column: "CreatedAt",
                value: new DateTime(2025, 6, 14, 14, 34, 1, 707, DateTimeKind.Utc).AddTicks(5770));

            migrationBuilder.UpdateData(
                table: "ExtraServices",
                keyColumn: "Id",
                keyValue: 3,
                column: "CreatedAt",
                value: new DateTime(2025, 6, 14, 14, 34, 1, 707, DateTimeKind.Utc).AddTicks(5774));

            migrationBuilder.UpdateData(
                table: "ExtraServices",
                keyColumn: "Id",
                keyValue: 4,
                column: "CreatedAt",
                value: new DateTime(2025, 6, 14, 14, 34, 1, 707, DateTimeKind.Utc).AddTicks(5777));

            migrationBuilder.UpdateData(
                table: "ExtraServices",
                keyColumn: "Id",
                keyValue: 5,
                column: "CreatedAt",
                value: new DateTime(2025, 6, 14, 14, 34, 1, 707, DateTimeKind.Utc).AddTicks(5779));

            migrationBuilder.UpdateData(
                table: "ExtraServices",
                keyColumn: "Id",
                keyValue: 6,
                column: "CreatedAt",
                value: new DateTime(2025, 6, 14, 14, 34, 1, 707, DateTimeKind.Utc).AddTicks(5782));

            migrationBuilder.UpdateData(
                table: "ExtraServices",
                keyColumn: "Id",
                keyValue: 7,
                column: "CreatedAt",
                value: new DateTime(2025, 6, 14, 14, 34, 1, 707, DateTimeKind.Utc).AddTicks(5785));

            migrationBuilder.UpdateData(
                table: "ExtraServices",
                keyColumn: "Id",
                keyValue: 8,
                column: "CreatedAt",
                value: new DateTime(2025, 6, 14, 14, 34, 1, 707, DateTimeKind.Utc).AddTicks(5787));

            migrationBuilder.UpdateData(
                table: "ExtraServices",
                keyColumn: "Id",
                keyValue: 9,
                column: "CreatedAt",
                value: new DateTime(2025, 6, 14, 14, 34, 1, 707, DateTimeKind.Utc).AddTicks(5789));

            migrationBuilder.UpdateData(
                table: "ServiceTypes",
                keyColumn: "Id",
                keyValue: 1,
                column: "CreatedAt",
                value: new DateTime(2025, 6, 14, 14, 34, 1, 706, DateTimeKind.Utc).AddTicks(5928));

            migrationBuilder.UpdateData(
                table: "ServiceTypes",
                keyColumn: "Id",
                keyValue: 2,
                column: "CreatedAt",
                value: new DateTime(2025, 6, 14, 14, 34, 1, 706, DateTimeKind.Utc).AddTicks(5931));

            migrationBuilder.UpdateData(
                table: "Services",
                keyColumn: "Id",
                keyValue: 1,
                column: "CreatedAt",
                value: new DateTime(2025, 6, 14, 14, 34, 1, 707, DateTimeKind.Utc).AddTicks(5685));

            migrationBuilder.UpdateData(
                table: "Services",
                keyColumn: "Id",
                keyValue: 2,
                column: "CreatedAt",
                value: new DateTime(2025, 6, 14, 14, 34, 1, 707, DateTimeKind.Utc).AddTicks(5689));

            migrationBuilder.UpdateData(
                table: "Services",
                keyColumn: "Id",
                keyValue: 3,
                column: "CreatedAt",
                value: new DateTime(2025, 6, 14, 14, 34, 1, 707, DateTimeKind.Utc).AddTicks(5694));

            migrationBuilder.UpdateData(
                table: "Services",
                keyColumn: "Id",
                keyValue: 4,
                column: "CreatedAt",
                value: new DateTime(2025, 6, 14, 14, 34, 1, 707, DateTimeKind.Utc).AddTicks(5730));

            migrationBuilder.UpdateData(
                table: "Services",
                keyColumn: "Id",
                keyValue: 5,
                column: "CreatedAt",
                value: new DateTime(2025, 6, 14, 14, 34, 1, 707, DateTimeKind.Utc).AddTicks(5733));

            migrationBuilder.UpdateData(
                table: "Subscriptions",
                keyColumn: "Id",
                keyValue: 1,
                column: "CreatedAt",
                value: new DateTime(2025, 6, 14, 14, 34, 1, 706, DateTimeKind.Utc).AddTicks(5713));

            migrationBuilder.UpdateData(
                table: "Subscriptions",
                keyColumn: "Id",
                keyValue: 2,
                column: "CreatedAt",
                value: new DateTime(2025, 6, 14, 14, 34, 1, 706, DateTimeKind.Utc).AddTicks(5719));

            migrationBuilder.UpdateData(
                table: "Subscriptions",
                keyColumn: "Id",
                keyValue: 3,
                column: "CreatedAt",
                value: new DateTime(2025, 6, 14, 14, 34, 1, 706, DateTimeKind.Utc).AddTicks(5720));

            migrationBuilder.UpdateData(
                table: "Subscriptions",
                keyColumn: "Id",
                keyValue: 4,
                column: "CreatedAt",
                value: new DateTime(2025, 6, 14, 14, 34, 1, 706, DateTimeKind.Utc).AddTicks(5722));

            migrationBuilder.CreateIndex(
                name: "IX_GiftCardUsages_UserId",
                table: "GiftCardUsages",
                column: "UserId");

            migrationBuilder.AddForeignKey(
                name: "FK_GiftCardUsages_Users_UserId",
                table: "GiftCardUsages",
                column: "UserId",
                principalTable: "Users",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_GiftCardUsages_Users_UserId",
                table: "GiftCardUsages");

            migrationBuilder.DropIndex(
                name: "IX_GiftCardUsages_UserId",
                table: "GiftCardUsages");

            migrationBuilder.DropColumn(
                name: "UserId",
                table: "GiftCardUsages");

            migrationBuilder.AddColumn<bool>(
                name: "IsUsed",
                table: "GiftCards",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<DateTime>(
                name: "UsedAt",
                table: "GiftCards",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "UsedByUserId",
                table: "GiftCards",
                type: "int",
                nullable: true);

            migrationBuilder.UpdateData(
                table: "ExtraServices",
                keyColumn: "Id",
                keyValue: 1,
                column: "CreatedAt",
                value: new DateTime(2025, 6, 14, 13, 0, 44, 507, DateTimeKind.Utc).AddTicks(4843));

            migrationBuilder.UpdateData(
                table: "ExtraServices",
                keyColumn: "Id",
                keyValue: 2,
                column: "CreatedAt",
                value: new DateTime(2025, 6, 14, 13, 0, 44, 507, DateTimeKind.Utc).AddTicks(4853));

            migrationBuilder.UpdateData(
                table: "ExtraServices",
                keyColumn: "Id",
                keyValue: 3,
                column: "CreatedAt",
                value: new DateTime(2025, 6, 14, 13, 0, 44, 507, DateTimeKind.Utc).AddTicks(4861));

            migrationBuilder.UpdateData(
                table: "ExtraServices",
                keyColumn: "Id",
                keyValue: 4,
                column: "CreatedAt",
                value: new DateTime(2025, 6, 14, 13, 0, 44, 507, DateTimeKind.Utc).AddTicks(4867));

            migrationBuilder.UpdateData(
                table: "ExtraServices",
                keyColumn: "Id",
                keyValue: 5,
                column: "CreatedAt",
                value: new DateTime(2025, 6, 14, 13, 0, 44, 507, DateTimeKind.Utc).AddTicks(4874));

            migrationBuilder.UpdateData(
                table: "ExtraServices",
                keyColumn: "Id",
                keyValue: 6,
                column: "CreatedAt",
                value: new DateTime(2025, 6, 14, 13, 0, 44, 507, DateTimeKind.Utc).AddTicks(4880));

            migrationBuilder.UpdateData(
                table: "ExtraServices",
                keyColumn: "Id",
                keyValue: 7,
                column: "CreatedAt",
                value: new DateTime(2025, 6, 14, 13, 0, 44, 507, DateTimeKind.Utc).AddTicks(4887));

            migrationBuilder.UpdateData(
                table: "ExtraServices",
                keyColumn: "Id",
                keyValue: 8,
                column: "CreatedAt",
                value: new DateTime(2025, 6, 14, 13, 0, 44, 507, DateTimeKind.Utc).AddTicks(4893));

            migrationBuilder.UpdateData(
                table: "ExtraServices",
                keyColumn: "Id",
                keyValue: 9,
                column: "CreatedAt",
                value: new DateTime(2025, 6, 14, 13, 0, 44, 507, DateTimeKind.Utc).AddTicks(4898));

            migrationBuilder.UpdateData(
                table: "ServiceTypes",
                keyColumn: "Id",
                keyValue: 1,
                column: "CreatedAt",
                value: new DateTime(2025, 6, 14, 13, 0, 44, 504, DateTimeKind.Utc).AddTicks(7686));

            migrationBuilder.UpdateData(
                table: "ServiceTypes",
                keyColumn: "Id",
                keyValue: 2,
                column: "CreatedAt",
                value: new DateTime(2025, 6, 14, 13, 0, 44, 504, DateTimeKind.Utc).AddTicks(7694));

            migrationBuilder.UpdateData(
                table: "Services",
                keyColumn: "Id",
                keyValue: 1,
                column: "CreatedAt",
                value: new DateTime(2025, 6, 14, 13, 0, 44, 507, DateTimeKind.Utc).AddTicks(4641));

            migrationBuilder.UpdateData(
                table: "Services",
                keyColumn: "Id",
                keyValue: 2,
                column: "CreatedAt",
                value: new DateTime(2025, 6, 14, 13, 0, 44, 507, DateTimeKind.Utc).AddTicks(4654));

            migrationBuilder.UpdateData(
                table: "Services",
                keyColumn: "Id",
                keyValue: 3,
                column: "CreatedAt",
                value: new DateTime(2025, 6, 14, 13, 0, 44, 507, DateTimeKind.Utc).AddTicks(4667));

            migrationBuilder.UpdateData(
                table: "Services",
                keyColumn: "Id",
                keyValue: 4,
                column: "CreatedAt",
                value: new DateTime(2025, 6, 14, 13, 0, 44, 507, DateTimeKind.Utc).AddTicks(4765));

            migrationBuilder.UpdateData(
                table: "Services",
                keyColumn: "Id",
                keyValue: 5,
                column: "CreatedAt",
                value: new DateTime(2025, 6, 14, 13, 0, 44, 507, DateTimeKind.Utc).AddTicks(4773));

            migrationBuilder.UpdateData(
                table: "Subscriptions",
                keyColumn: "Id",
                keyValue: 1,
                column: "CreatedAt",
                value: new DateTime(2025, 6, 14, 13, 0, 44, 504, DateTimeKind.Utc).AddTicks(7230));

            migrationBuilder.UpdateData(
                table: "Subscriptions",
                keyColumn: "Id",
                keyValue: 2,
                column: "CreatedAt",
                value: new DateTime(2025, 6, 14, 13, 0, 44, 504, DateTimeKind.Utc).AddTicks(7240));

            migrationBuilder.UpdateData(
                table: "Subscriptions",
                keyColumn: "Id",
                keyValue: 3,
                column: "CreatedAt",
                value: new DateTime(2025, 6, 14, 13, 0, 44, 504, DateTimeKind.Utc).AddTicks(7244));

            migrationBuilder.UpdateData(
                table: "Subscriptions",
                keyColumn: "Id",
                keyValue: 4,
                column: "CreatedAt",
                value: new DateTime(2025, 6, 14, 13, 0, 44, 504, DateTimeKind.Utc).AddTicks(7248));

            migrationBuilder.CreateIndex(
                name: "IX_GiftCards_UsedByUserId",
                table: "GiftCards",
                column: "UsedByUserId");

            migrationBuilder.AddForeignKey(
                name: "FK_GiftCards_Users_UsedByUserId",
                table: "GiftCards",
                column: "UsedByUserId",
                principalTable: "Users",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
