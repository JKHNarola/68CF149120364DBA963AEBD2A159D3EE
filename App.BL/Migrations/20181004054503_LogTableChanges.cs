using Microsoft.EntityFrameworkCore.Migrations;

namespace App.BL.Migrations
{
    public partial class LogTableChanges : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Host",
                table: "Logs");

            migrationBuilder.RenameColumn(
                name: "User",
                table: "Logs",
                newName: "ReqPath");

            migrationBuilder.RenameColumn(
                name: "Path",
                table: "Logs",
                newName: "ReqIp");

            migrationBuilder.AddColumn<string>(
                name: "Payload",
                table: "Logs",
                maxLength: 5000,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "ReqHeaders",
                table: "Logs",
                maxLength: 5000,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "ReqMethod",
                table: "Logs",
                maxLength: 50,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "UserId",
                table: "Logs",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Logs_UserId",
                table: "Logs",
                column: "UserId");

            migrationBuilder.AddForeignKey(
                name: "FK_Logs_Users_UserId",
                table: "Logs",
                column: "UserId",
                principalTable: "Users",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Logs_Users_UserId",
                table: "Logs");

            migrationBuilder.DropIndex(
                name: "IX_Logs_UserId",
                table: "Logs");

            migrationBuilder.DropColumn(
                name: "Payload",
                table: "Logs");

            migrationBuilder.DropColumn(
                name: "ReqHeaders",
                table: "Logs");

            migrationBuilder.DropColumn(
                name: "ReqMethod",
                table: "Logs");

            migrationBuilder.DropColumn(
                name: "UserId",
                table: "Logs");

            migrationBuilder.RenameColumn(
                name: "ReqPath",
                table: "Logs",
                newName: "User");

            migrationBuilder.RenameColumn(
                name: "ReqIp",
                table: "Logs",
                newName: "Path");

            migrationBuilder.AddColumn<string>(
                name: "Host",
                table: "Logs",
                maxLength: 255,
                nullable: true);
        }
    }
}
