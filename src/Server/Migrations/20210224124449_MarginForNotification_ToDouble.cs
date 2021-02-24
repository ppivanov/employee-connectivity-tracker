using Microsoft.EntityFrameworkCore.Migrations;

namespace EctBlazorApp.Server.Migrations
{
    public partial class MarginForNotification_ToDouble : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<double>(
                name: "MarginForNotification",
                table: "Teams",
                nullable: false,
                oldClrType: typeof(int),
                oldType: "int");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<int>(
                name: "MarginForNotification",
                table: "Teams",
                type: "int",
                nullable: false,
                oldClrType: typeof(double));
        }
    }
}
