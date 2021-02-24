using Microsoft.EntityFrameworkCore.Migrations;

namespace EctBlazorApp.Server.Migrations
{
    public partial class MarginForNotification : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "MarginForNotification",
                table: "Teams",
                nullable: false,
                defaultValue: 0);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "MarginForNotification",
                table: "Teams");
        }
    }
}
