using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace Flowey.DATACCESS.Migrations
{
    /// <inheritdoc />
    public partial class RemoveRoleTable : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ProjectUserRoles_Roles_RoleId",
                table: "ProjectUserRoles");

            migrationBuilder.DropTable(
                name: "Roles");

            migrationBuilder.DropIndex(
                name: "IX_ProjectUserRoles_RoleId",
                table: "ProjectUserRoles");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Roles",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Code = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Name = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Roles", x => x.Id);
                });

            migrationBuilder.InsertData(
                table: "Roles",
                columns: new[] { "Id", "Code", "Name" },
                values: new object[,]
                {
                    { 1, "ADMIN", "Admin" },
                    { 2, "EDITOR", "Editor" },
                    { 3, "MEMBER", "Member" }
                });

            migrationBuilder.CreateIndex(
                name: "IX_ProjectUserRoles_RoleId",
                table: "ProjectUserRoles",
                column: "RoleId");

            migrationBuilder.AddForeignKey(
                name: "FK_ProjectUserRoles_Roles_RoleId",
                table: "ProjectUserRoles",
                column: "RoleId",
                principalTable: "Roles",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
