using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace TImViecAPI.Migrations
{
    /// <inheritdoc />
    public partial class Add_CongViecYeuThich : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<string>(
                name: "YeuCau",
                table: "TInTuyenDung",
                type: "longtext",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "int",
                oldNullable: true)
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "CongViecYeuThich",
                columns: table => new
                {
                    ungvienID = table.Column<int>(type: "int", nullable: false),
                    tintuyenID = table.Column<int>(type: "int", nullable: false),
                    NgayThem = table.Column<DateOnly>(type: "date", nullable: true),
                    NhaTuyenDungntdid = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CongViecYeuThich", x => new { x.ungvienID, x.tintuyenID });
                    table.ForeignKey(
                        name: "FK_CongViecYeuThich_NhaTuyenDung_NhaTuyenDungntdid",
                        column: x => x.NhaTuyenDungntdid,
                        principalTable: "NhaTuyenDung",
                        principalColumn: "ntdid");
                    table.ForeignKey(
                        name: "FK_CongViecYeuThich_TInTuyenDung_tintuyenID",
                        column: x => x.tintuyenID,
                        principalTable: "TInTuyenDung",
                        principalColumn: "ttdid",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_CongViecYeuThich_UngVien_ungvienID",
                        column: x => x.ungvienID,
                        principalTable: "UngVien",
                        principalColumn: "uvid",
                        onDelete: ReferentialAction.Cascade);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateIndex(
                name: "IX_CongViecYeuThich_NhaTuyenDungntdid",
                table: "CongViecYeuThich",
                column: "NhaTuyenDungntdid");

            migrationBuilder.CreateIndex(
                name: "IX_CongViecYeuThich_tintuyenID",
                table: "CongViecYeuThich",
                column: "tintuyenID");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "CongViecYeuThich");

            migrationBuilder.AlterColumn<int>(
                name: "YeuCau",
                table: "TInTuyenDung",
                type: "int",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "longtext",
                oldNullable: true)
                .OldAnnotation("MySql:CharSet", "utf8mb4");
        }
    }
}
