using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace TImViecAPI.Migrations
{
    /// <inheritdoc />
    public partial class UpdateNoiDungHoSo : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "NamKinhNghiem",
                table: "NoiDungHoSo");

            migrationBuilder.AddColumn<string>(
                name: "DiaChi",
                table: "NoiDungHoSo",
                type: "varchar(255)",
                maxLength: 255,
                nullable: true)
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.AddColumn<string>(
                name: "GioiTinh",
                table: "NoiDungHoSo",
                type: "varchar(255)",
                maxLength: 255,
                nullable: true)
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.AddColumn<string>(
                name: "NgaySinh",
                table: "NoiDungHoSo",
                type: "varchar(255)",
                maxLength: 255,
                nullable: true)
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.AddColumn<string>(
                name: "QuanHuyen",
                table: "NoiDungHoSo",
                type: "varchar(255)",
                maxLength: 255,
                nullable: true)
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.AddColumn<string>(
                name: "QuocGia",
                table: "NoiDungHoSo",
                type: "varchar(255)",
                maxLength: 255,
                nullable: true)
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.AddColumn<string>(
                name: "Tinh",
                table: "NoiDungHoSo",
                type: "varchar(255)",
                maxLength: 255,
                nullable: true)
                .Annotation("MySql:CharSet", "utf8mb4");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "DiaChi",
                table: "NoiDungHoSo");

            migrationBuilder.DropColumn(
                name: "GioiTinh",
                table: "NoiDungHoSo");

            migrationBuilder.DropColumn(
                name: "NgaySinh",
                table: "NoiDungHoSo");

            migrationBuilder.DropColumn(
                name: "QuanHuyen",
                table: "NoiDungHoSo");

            migrationBuilder.DropColumn(
                name: "QuocGia",
                table: "NoiDungHoSo");

            migrationBuilder.DropColumn(
                name: "Tinh",
                table: "NoiDungHoSo");

            migrationBuilder.AddColumn<int>(
                name: "NamKinhNghiem",
                table: "NoiDungHoSo",
                type: "int",
                nullable: true);
        }
    }
}
