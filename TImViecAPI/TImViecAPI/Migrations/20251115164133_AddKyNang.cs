using Microsoft.EntityFrameworkCore.Migrations;
#nullable disable

namespace TImViecAPI.Migrations
{
    public partial class AddKyNang : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
         
            migrationBuilder.AddColumn<string>(
                name: "KyNang",
                table: "NoiDungHoSo",
                type: "longtext",
                nullable: true)
                .Annotation("MySql:CharSet", "utf8mb4");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            // CHỈ XÓA CỘT
            migrationBuilder.DropColumn(
                name: "KyNang",
                table: "NoiDungHoSo");
        }
    }
}