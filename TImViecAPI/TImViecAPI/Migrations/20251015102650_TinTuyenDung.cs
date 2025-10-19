using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace TImViecAPI.Migrations
{
    /// <inheritdoc />
    public partial class TinTuyenDung : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "nhaTuyenDungID",
                table: "TInTuyenDung",
                type: "int",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_TInTuyenDung_nhaTuyenDungID",
                table: "TInTuyenDung",
                column: "nhaTuyenDungID");

            migrationBuilder.AddForeignKey(
                name: "FK_TInTuyenDung_NhaTuyenDung_nhaTuyenDungID",
                table: "TInTuyenDung",
                column: "nhaTuyenDungID",
                principalTable: "NhaTuyenDung",
                principalColumn: "ntdid");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_TInTuyenDung_NhaTuyenDung_nhaTuyenDungID",
                table: "TInTuyenDung");

            migrationBuilder.DropIndex(
                name: "IX_TInTuyenDung_nhaTuyenDungID",
                table: "TInTuyenDung");

            migrationBuilder.DropColumn(
                name: "nhaTuyenDungID",
                table: "TInTuyenDung");
        }
    }
}
