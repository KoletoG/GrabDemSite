using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace GrabDemSite.Data.Migrations
{
    public partial class _14 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<double>(
                name: "PlayMoney",
                table: "AspNetUsers",
                type: "float",
                nullable: false,
                defaultValue: 0.0);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "PlayMoney",
                table: "AspNetUsers");
        }
    }
}
