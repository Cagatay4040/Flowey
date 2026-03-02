using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Flowey.DATACCESS.Migrations
{
    /// <inheritdoc />
    public partial class Entities_Changed : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "CreatedBy",
                table: "Roles");

            migrationBuilder.DropColumn(
                name: "CreatedDate",
                table: "Roles");

            migrationBuilder.DropColumn(
                name: "IsActive",
                table: "Roles");

            migrationBuilder.DropColumn(
                name: "ModifiedBy",
                table: "Roles");

            migrationBuilder.DropColumn(
                name: "ModifiedDate",
                table: "Roles");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<Guid>(
                name: "CreatedBy",
                table: "Roles",
                type: "uniqueidentifier",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.AddColumn<DateTime>(
                name: "CreatedDate",
                table: "Roles",
                type: "datetime2",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<bool>(
                name: "IsActive",
                table: "Roles",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<Guid>(
                name: "ModifiedBy",
                table: "Roles",
                type: "uniqueidentifier",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "ModifiedDate",
                table: "Roles",
                type: "datetime2",
                nullable: true);

            migrationBuilder.UpdateData(
                table: "Roles",
                keyColumn: "Id",
                keyValue: 1,
                columns: new[] { "CreatedBy", "CreatedDate", "IsActive", "ModifiedBy", "ModifiedDate" },
                values: new object[] { new Guid("00000000-0000-0000-0000-000000000000"), new DateTime(2026, 2, 28, 14, 18, 38, 806, DateTimeKind.Local).AddTicks(2878), true, null, null });

            migrationBuilder.UpdateData(
                table: "Roles",
                keyColumn: "Id",
                keyValue: 2,
                columns: new[] { "CreatedBy", "CreatedDate", "IsActive", "ModifiedBy", "ModifiedDate" },
                values: new object[] { new Guid("00000000-0000-0000-0000-000000000000"), new DateTime(2026, 2, 28, 14, 18, 38, 806, DateTimeKind.Local).AddTicks(2887), true, null, null });

            migrationBuilder.UpdateData(
                table: "Roles",
                keyColumn: "Id",
                keyValue: 3,
                columns: new[] { "CreatedBy", "CreatedDate", "IsActive", "ModifiedBy", "ModifiedDate" },
                values: new object[] { new Guid("00000000-0000-0000-0000-000000000000"), new DateTime(2026, 2, 28, 14, 18, 38, 806, DateTimeKind.Local).AddTicks(2888), true, null, null });
        }
    }
}
