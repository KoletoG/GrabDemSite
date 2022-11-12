using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace GrabDemSite.Data.Migrations
{
    public partial class AddTaskDataModel : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "TaskDatas",
                columns: table => new
                {
                    Id = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    Count = table.Column<int>(type: "int", nullable: false),
                    UserId = table.Column<string>(type: "nvarchar(450)", nullable: true),
                    LevelOfTask = table.Column<int>(type: "int", nullable: false),
                    DateStarted = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TaskDatas", x => x.Id);
                    table.ForeignKey(
                        name: "FK_TaskDatas_AspNetUsers_UserId",
                        column: x => x.UserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateIndex(
                name: "IX_TaskDatas_UserId",
                table: "TaskDatas",
                column: "UserId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "TaskDatas");
        }
    }
}
