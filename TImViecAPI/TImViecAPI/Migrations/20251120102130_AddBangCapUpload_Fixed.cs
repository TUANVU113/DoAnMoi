using System;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace TImViecAPI.Migrations
{
    /// <inheritdoc />
    public partial class AddBangCapUpload_Fixed : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "BangCap_Upload",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    hosoID = table.Column<int>(type: "int", nullable: false),
                    TenBangCap = table.Column<string>(type: "varchar(255)", maxLength: 255, nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    Loai = table.Column<string>(type: "longtext", nullable: false, defaultValue: "BangCap")
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    FileUrl = table.Column<string>(type: "longtext", nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    NgayUpload = table.Column<DateTime>(type: "datetime(6)", nullable: false, defaultValueSql: "CURRENT_TIMESTAMP(6)")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_BangCap_Upload", x => x.Id);
                    table.ForeignKey(
                        name: "FK_BangCap_Upload_HoSo_hosoID",
                        column: x => x.hosoID,
                        principalTable: "HoSo",
                        principalColumn: "hsid",
                        onDelete: ReferentialAction.Cascade);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateIndex(
                name: "IX_BangCap_Upload_hosoID",
                table: "BangCap_Upload",
                column: "hosoID");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "BangCap_Upload");
        }
    }
}
