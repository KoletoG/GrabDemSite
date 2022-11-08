using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace GrabDemSite.Data.Migrations
{
    public partial class Levels : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "Level",
                table: "AspNetUsers",
                newName: "InviteCount");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "InviteCount",
                table: "AspNetUsers",
                newName: "Level");
        }
    }
}
