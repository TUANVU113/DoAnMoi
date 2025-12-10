using System;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace TImViecAPI.Migrations
{
    /// <inheritdoc />
    public partial class AddToCaoTin : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "ToCaoTin",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    ungvienID = table.Column<int>(type: "int", nullable: false),
                    ttdid = table.Column<int>(type: "int", nullable: false),
                    LyDo = table.Column<string>(type: "varchar(100)", maxLength: 100, nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    NoiDung = table.Column<string>(type: "longtext", nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    NgayToCao = table.Column<DateTime>(type: "datetime(6)", nullable: false),
                    TrangThai = table.Column<string>(type: "varchar(255)", nullable: false, defaultValue: "Chờ xử lý")
                        .Annotation("MySql:CharSet", "utf8mb4")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ToCaoTin", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ToCaoTin_TInTuyenDung_ttdid",
                        column: x => x.ttdid,
                        principalTable: "TInTuyenDung",
                        principalColumn: "ttdid",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_ToCaoTin_UngVien_ungvienID",
                        column: x => x.ungvienID,
                        principalTable: "UngVien",
                        principalColumn: "uvid",
                        onDelete: ReferentialAction.Cascade);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateIndex(
                name: "IX_ToCaoTin_TrangThai",
                table: "ToCaoTin",
                column: "TrangThai");

            migrationBuilder.CreateIndex(
                name: "IX_ToCaoTin_ttdid",
                table: "ToCaoTin",
                column: "ttdid");

            migrationBuilder.CreateIndex(
                name: "IX_ToCaoTin_ungvienID",
                table: "ToCaoTin",
                column: "ungvienID");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "ToCaoTin");
        }
    }
}
