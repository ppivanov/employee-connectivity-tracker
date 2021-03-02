using Microsoft.EntityFrameworkCore.Migrations;

namespace EctBlazorApp.Server.Migrations
{
    public partial class AdditionalUsersToNotify : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "AdditionalUsersToNotifyAsAtring",
                table: "Teams",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "AdditionalUsersToNotifyAsAtring",
                table: "Teams");
        }
    }
}
