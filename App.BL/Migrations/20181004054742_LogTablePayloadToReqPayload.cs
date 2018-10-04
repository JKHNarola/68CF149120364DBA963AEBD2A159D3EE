using Microsoft.EntityFrameworkCore.Migrations;

namespace App.BL.Migrations
{
    public partial class LogTablePayloadToReqPayload : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "Payload",
                table: "Logs",
                newName: "ReqPayload");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "ReqPayload",
                table: "Logs",
                newName: "Payload");
        }
    }
}
