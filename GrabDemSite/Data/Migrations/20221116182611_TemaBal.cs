using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace GrabDemSite.Data.Migrations
{
    public partial class TemaBal : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "Level",
                table: "AspNetUsers",
                type: "int",
                nullable: false,
                defaultValue: 0);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Level",
                table: "AspNetUsers");
        }
    }
}
