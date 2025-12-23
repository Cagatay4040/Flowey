using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Flowey.DATACCESS.Migrations
{
    /// <inheritdoc />
    public partial class RenameUserIdToAssigneeId : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "UserId",
                table: "Tasks",
                newName: "AssigneeId");

            migrationBuilder.RenameIndex(
                name: "IX_Tasks_UserId",
                table: "Tasks",
                newName: "IX_Tasks_AssigneeId");

            migrationBuilder.UpdateData(
                table: "Roles",
                keyColumn: "Id",
                keyValue: 1,
                column: "CreatedDate",
                value: new DateTime(2025, 12, 23, 23, 58, 50, 854, DateTimeKind.Local).AddTicks(4058));

            migrationBuilder.UpdateData(
                table: "Roles",
                keyColumn: "Id",
                keyValue: 2,
                column: "CreatedDate",
                value: new DateTime(2025, 12, 23, 23, 58, 50, 854, DateTimeKind.Local).AddTicks(4067));

            migrationBuilder.UpdateData(
                table: "Roles",
                keyColumn: "Id",
                keyValue: 3,
                column: "CreatedDate",
                value: new DateTime(2025, 12, 23, 23, 58, 50, 854, DateTimeKind.Local).AddTicks(4068));
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.UpdateData(
                table: "Roles",
                keyColumn: "Id",
                keyValue: 1,
                column: "CreatedDate",
                value: new DateTime(2025, 12, 23, 23, 44, 58, 569, DateTimeKind.Local).AddTicks(6334));

            migrationBuilder.UpdateData(
                table: "Roles",
                keyColumn: "Id",
                keyValue: 2,
                column: "CreatedDate",
                value: new DateTime(2025, 12, 23, 23, 44, 58, 569, DateTimeKind.Local).AddTicks(6342));

            migrationBuilder.UpdateData(
                table: "Roles",
                keyColumn: "Id",
                keyValue: 3,
                column: "CreatedDate",
                value: new DateTime(2025, 12, 23, 23, 44, 58, 569, DateTimeKind.Local).AddTicks(6343));
        }
    }
}
