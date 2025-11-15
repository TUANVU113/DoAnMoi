using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace TImViecAPI.Migrations
{
    /// <inheritdoc />
    public partial class Add_ID_Fields_To_NoiDungHoSo : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_NoiDungHoSo_HoSo_hosoID",
                table: "NoiDungHoSo");

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

            migrationBuilder.RenameColumn(
                name: "Tinh",
                table: "NoiDungHoSo",
                newName: "Avata");

            migrationBuilder.AlterColumn<string>(
                name: "HocVan",
                table: "NoiDungHoSo",
                type: "longtext",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "varchar(255)",
                oldMaxLength: 255,
                oldNullable: true)
                .Annotation("MySql:CharSet", "utf8mb4")
                .OldAnnotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.AddColumn<string>(
                name: "ChucChi",
                table: "NoiDungHoSo",
                type: "longtext",
                nullable: true)
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.AddColumn<int>(
                name: "ChucDanhID",
                table: "NoiDungHoSo",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "LinhVucID",
                table: "NoiDungHoSo",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "LoaiHinhLamViecID",
                table: "NoiDungHoSo",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "MucLuong",
                table: "NoiDungHoSo",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "MucTieu",
                table: "NoiDungHoSo",
                type: "longtext",
                nullable: true)
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.AddColumn<int>(
                name: "NamKinhNghiemID",
                table: "NoiDungHoSo",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "ViTriLamViecID",
                table: "NoiDungHoSo",
                type: "int",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_NoiDungHoSo_ChucDanhID",
                table: "NoiDungHoSo",
                column: "ChucDanhID");

            migrationBuilder.CreateIndex(
                name: "IX_NoiDungHoSo_LinhVucID",
                table: "NoiDungHoSo",
                column: "LinhVucID");

            migrationBuilder.CreateIndex(
                name: "IX_NoiDungHoSo_LoaiHinhLamViecID",
                table: "NoiDungHoSo",
                column: "LoaiHinhLamViecID");

            migrationBuilder.CreateIndex(
                name: "IX_NoiDungHoSo_NamKinhNghiemID",
                table: "NoiDungHoSo",
                column: "NamKinhNghiemID");

            migrationBuilder.CreateIndex(
                name: "IX_NoiDungHoSo_ViTriLamViecID",
                table: "NoiDungHoSo",
                column: "ViTriLamViecID");

            migrationBuilder.AddForeignKey(
                name: "FK_NoiDungHoSo_ChucDanh",
                table: "NoiDungHoSo",
                column: "ChucDanhID",
                principalTable: "ChucDanh",
                principalColumn: "cdid",
                onDelete: ReferentialAction.SetNull);

            migrationBuilder.AddForeignKey(
                name: "FK_NoiDungHoSo_HoSo",
                table: "NoiDungHoSo",
                column: "hosoID",
                principalTable: "HoSo",
                principalColumn: "hsid",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_NoiDungHoSo_KinhNghiem",
                table: "NoiDungHoSo",
                column: "NamKinhNghiemID",
                principalTable: "KinhNghiem",
                principalColumn: "knid",
                onDelete: ReferentialAction.SetNull);

            migrationBuilder.AddForeignKey(
                name: "FK_NoiDungHoSo_LinhVuc",
                table: "NoiDungHoSo",
                column: "LinhVucID",
                principalTable: "LinhVuc",
                principalColumn: "lvid",
                onDelete: ReferentialAction.SetNull);

            migrationBuilder.AddForeignKey(
                name: "FK_NoiDungHoSo_LoaiHinhLamViec",
                table: "NoiDungHoSo",
                column: "LoaiHinhLamViecID",
                principalTable: "LoaiHinhLamViec",
                principalColumn: "lhid",
                onDelete: ReferentialAction.SetNull);

            migrationBuilder.AddForeignKey(
                name: "FK_NoiDungHoSo_ViTri",
                table: "NoiDungHoSo",
                column: "ViTriLamViecID",
                principalTable: "ViTri",
                principalColumn: "vtid",
                onDelete: ReferentialAction.SetNull);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_NoiDungHoSo_ChucDanh",
                table: "NoiDungHoSo");

            migrationBuilder.DropForeignKey(
                name: "FK_NoiDungHoSo_HoSo",
                table: "NoiDungHoSo");

            migrationBuilder.DropForeignKey(
                name: "FK_NoiDungHoSo_KinhNghiem",
                table: "NoiDungHoSo");

            migrationBuilder.DropForeignKey(
                name: "FK_NoiDungHoSo_LinhVuc",
                table: "NoiDungHoSo");

            migrationBuilder.DropForeignKey(
                name: "FK_NoiDungHoSo_LoaiHinhLamViec",
                table: "NoiDungHoSo");

            migrationBuilder.DropForeignKey(
                name: "FK_NoiDungHoSo_ViTri",
                table: "NoiDungHoSo");

            migrationBuilder.DropIndex(
                name: "IX_NoiDungHoSo_ChucDanhID",
                table: "NoiDungHoSo");

            migrationBuilder.DropIndex(
                name: "IX_NoiDungHoSo_LinhVucID",
                table: "NoiDungHoSo");

            migrationBuilder.DropIndex(
                name: "IX_NoiDungHoSo_LoaiHinhLamViecID",
                table: "NoiDungHoSo");

            migrationBuilder.DropIndex(
                name: "IX_NoiDungHoSo_NamKinhNghiemID",
                table: "NoiDungHoSo");

            migrationBuilder.DropIndex(
                name: "IX_NoiDungHoSo_ViTriLamViecID",
                table: "NoiDungHoSo");

            migrationBuilder.DropColumn(
                name: "ChucChi",
                table: "NoiDungHoSo");

            migrationBuilder.DropColumn(
                name: "ChucDanhID",
                table: "NoiDungHoSo");

            migrationBuilder.DropColumn(
                name: "LinhVucID",
                table: "NoiDungHoSo");

            migrationBuilder.DropColumn(
                name: "LoaiHinhLamViecID",
                table: "NoiDungHoSo");

            migrationBuilder.DropColumn(
                name: "MucLuong",
                table: "NoiDungHoSo");

            migrationBuilder.DropColumn(
                name: "MucTieu",
                table: "NoiDungHoSo");

            migrationBuilder.DropColumn(
                name: "NamKinhNghiemID",
                table: "NoiDungHoSo");

            migrationBuilder.DropColumn(
                name: "ViTriLamViecID",
                table: "NoiDungHoSo");

            migrationBuilder.RenameColumn(
                name: "Avata",
                table: "NoiDungHoSo",
                newName: "Tinh");

            migrationBuilder.AlterColumn<string>(
                name: "HocVan",
                table: "NoiDungHoSo",
                type: "varchar(255)",
                maxLength: 255,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "longtext",
                oldNullable: true)
                .Annotation("MySql:CharSet", "utf8mb4")
                .OldAnnotation("MySql:CharSet", "utf8mb4");

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

            migrationBuilder.AddForeignKey(
                name: "FK_NoiDungHoSo_HoSo_hosoID",
                table: "NoiDungHoSo",
                column: "hosoID",
                principalTable: "HoSo",
                principalColumn: "hsid");
        }
    }
}
