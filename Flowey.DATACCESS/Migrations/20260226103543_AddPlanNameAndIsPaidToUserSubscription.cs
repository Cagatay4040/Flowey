using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Flowey.DATACCESS.Migrations
{
    /// <inheritdoc />
    public partial class AddPlanNameAndIsPaidToUserSubscription : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "IsPaid",
                table: "UserSubscriptions",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<string>(
                name: "PlanName",
                table: "UserSubscriptions",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.UpdateData(
                table: "Roles",
                keyColumn: "Id",
                keyValue: 1,
                column: "CreatedDate",
                value: new DateTime(2026, 2, 26, 13, 35, 42, 853, DateTimeKind.Local).AddTicks(1802));

            migrationBuilder.UpdateData(
                table: "Roles",
                keyColumn: "Id",
                keyValue: 2,
                column: "CreatedDate",
                value: new DateTime(2026, 2, 26, 13, 35, 42, 853, DateTimeKind.Local).AddTicks(1812));

            migrationBuilder.UpdateData(
                table: "Roles",
                keyColumn: "Id",
                keyValue: 3,
                column: "CreatedDate",
                value: new DateTime(2026, 2, 26, 13, 35, 42, 853, DateTimeKind.Local).AddTicks(1813));
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "IsPaid",
                table: "UserSubscriptions");

            migrationBuilder.DropColumn(
                name: "PlanName",
                table: "UserSubscriptions");

            migrationBuilder.UpdateData(
                table: "Roles",
                keyColumn: "Id",
                keyValue: 1,
                column: "CreatedDate",
                value: new DateTime(2026, 2, 25, 13, 32, 26, 229, DateTimeKind.Local).AddTicks(9014));

            migrationBuilder.UpdateData(
                table: "Roles",
                keyColumn: "Id",
                keyValue: 2,
                column: "CreatedDate",
                value: new DateTime(2026, 2, 25, 13, 32, 26, 229, DateTimeKind.Local).AddTicks(9025));

            migrationBuilder.UpdateData(
                table: "Roles",
                keyColumn: "Id",
                keyValue: 3,
                column: "CreatedDate",
                value: new DateTime(2026, 2, 25, 13, 32, 26, 229, DateTimeKind.Local).AddTicks(9026));
        }
    }
}
