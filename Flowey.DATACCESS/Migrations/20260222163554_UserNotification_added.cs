using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Flowey.DATACCESS.Migrations
{
    /// <inheritdoc />
    public partial class UserNotification_added : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_UserSubscription_Users_UserId",
                table: "UserSubscription");

            migrationBuilder.DropPrimaryKey(
                name: "PK_UserSubscription",
                table: "UserSubscription");

            migrationBuilder.RenameTable(
                name: "UserSubscription",
                newName: "UserSubscriptions");

            migrationBuilder.RenameIndex(
                name: "IX_UserSubscription_UserId",
                table: "UserSubscriptions",
                newName: "IX_UserSubscriptions_UserId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_UserSubscriptions",
                table: "UserSubscriptions",
                column: "Id");

            migrationBuilder.CreateTable(
                name: "UserNotifications",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    UserId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Title = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Message = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    ActionUrl = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    IsRead = table.Column<bool>(type: "bit", nullable: false),
                    IsActive = table.Column<bool>(type: "bit", nullable: false),
                    CreatedDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CreatedBy = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    ModifiedDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    ModifiedBy = table.Column<Guid>(type: "uniqueidentifier", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_UserNotifications", x => x.Id);
                    table.ForeignKey(
                        name: "FK_UserNotifications_Users_UserId",
                        column: x => x.UserId,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.UpdateData(
                table: "Roles",
                keyColumn: "Id",
                keyValue: 1,
                column: "CreatedDate",
                value: new DateTime(2026, 2, 22, 19, 35, 54, 703, DateTimeKind.Local).AddTicks(1947));

            migrationBuilder.UpdateData(
                table: "Roles",
                keyColumn: "Id",
                keyValue: 2,
                column: "CreatedDate",
                value: new DateTime(2026, 2, 22, 19, 35, 54, 703, DateTimeKind.Local).AddTicks(1955));

            migrationBuilder.UpdateData(
                table: "Roles",
                keyColumn: "Id",
                keyValue: 3,
                column: "CreatedDate",
                value: new DateTime(2026, 2, 22, 19, 35, 54, 703, DateTimeKind.Local).AddTicks(1956));

            migrationBuilder.CreateIndex(
                name: "IX_UserNotifications_UserId",
                table: "UserNotifications",
                column: "UserId");

            migrationBuilder.AddForeignKey(
                name: "FK_UserSubscriptions_Users_UserId",
                table: "UserSubscriptions",
                column: "UserId",
                principalTable: "Users",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_UserSubscriptions_Users_UserId",
                table: "UserSubscriptions");

            migrationBuilder.DropTable(
                name: "UserNotifications");

            migrationBuilder.DropPrimaryKey(
                name: "PK_UserSubscriptions",
                table: "UserSubscriptions");

            migrationBuilder.RenameTable(
                name: "UserSubscriptions",
                newName: "UserSubscription");

            migrationBuilder.RenameIndex(
                name: "IX_UserSubscriptions_UserId",
                table: "UserSubscription",
                newName: "IX_UserSubscription_UserId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_UserSubscription",
                table: "UserSubscription",
                column: "Id");

            migrationBuilder.UpdateData(
                table: "Roles",
                keyColumn: "Id",
                keyValue: 1,
                column: "CreatedDate",
                value: new DateTime(2026, 2, 21, 13, 1, 39, 937, DateTimeKind.Local).AddTicks(7959));

            migrationBuilder.UpdateData(
                table: "Roles",
                keyColumn: "Id",
                keyValue: 2,
                column: "CreatedDate",
                value: new DateTime(2026, 2, 21, 13, 1, 39, 937, DateTimeKind.Local).AddTicks(7966));

            migrationBuilder.UpdateData(
                table: "Roles",
                keyColumn: "Id",
                keyValue: 3,
                column: "CreatedDate",
                value: new DateTime(2026, 2, 21, 13, 1, 39, 937, DateTimeKind.Local).AddTicks(7967));

            migrationBuilder.AddForeignKey(
                name: "FK_UserSubscription_Users_UserId",
                table: "UserSubscription",
                column: "UserId",
                principalTable: "Users",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
