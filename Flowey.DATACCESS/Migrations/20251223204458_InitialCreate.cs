using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Flowey.DATACCESS.Migrations
{
    public partial class InitialCreate : Migration
    {
        /// <summary>
        /// Değişiklikleri Veritabanına Uygula (UP)
        /// </summary>
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            // 1. Tasks Tablosuna CurrentStepId Ekle (Zorunlu Alan)
            migrationBuilder.AddColumn<Guid>(
                name: "CurrentStepId",
                table: "Tasks",
                type: "uniqueidentifier",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));
            // NOT: Eğer içeride veri varsa bu default değerle (Empty Guid) eklenir. 
            // Bu Guid Step tablosunda yoksa FK hatası alırsın! 
            // Veri varsa burayı nullable: true yapıp sonra güncellemen gerekir.

            // 2. Tasks Tablosuna UserId Ekle (Opsiyonel Alan - Nullable)
            migrationBuilder.AddColumn<Guid>(
                name: "UserId",
                table: "Tasks",
                type: "uniqueidentifier",
                nullable: true);

            // 3. Performans için Index Oluştur (Sorgularda çok hız kazandırır)
            migrationBuilder.CreateIndex(
                name: "IX_Tasks_CurrentStepId",
                table: "Tasks",
                column: "CurrentStepId");

            migrationBuilder.CreateIndex(
                name: "IX_Tasks_UserId",
                table: "Tasks",
                column: "UserId");

            // 4. Foreign Key (İlişki) Tanımları

            // Task -> Step İlişkisi
            migrationBuilder.AddForeignKey(
                name: "FK_Tasks_Steps_CurrentStepId",
                table: "Tasks",
                column: "CurrentStepId",
                principalTable: "Steps",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict); // Step silinirse Task silinmesin, hata versin (Güvenlik)

            // Task -> User İlişkisi
            migrationBuilder.AddForeignKey(
                name: "FK_Tasks_Users_UserId",
                table: "Tasks",
                column: "UserId",
                principalTable: "Users",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict); // User silinirse Task silinmesin
        }

        /// <summary>
        /// Değişiklikleri Geri Al (DOWN)
        /// </summary>
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            // Foreign Key'leri kaldır
            migrationBuilder.DropForeignKey(
                name: "FK_Tasks_Steps_CurrentStepId",
                table: "Tasks");

            migrationBuilder.DropForeignKey(
                name: "FK_Tasks_Users_UserId",
                table: "Tasks");

            // Index'leri kaldır
            migrationBuilder.DropIndex(
                name: "IX_Tasks_CurrentStepId",
                table: "Tasks");

            migrationBuilder.DropIndex(
                name: "IX_Tasks_UserId",
                table: "Tasks");

            // Kolonları kaldır
            migrationBuilder.DropColumn(
                name: "CurrentStepId",
                table: "Tasks");

            migrationBuilder.DropColumn(
                name: "UserId",
                table: "Tasks");
        }
    }
}