using System;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace TImViecAPI.Migrations
{
    /// <inheritdoc />
    public partial class initDB : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterDatabase()
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "BangCap",
                columns: table => new
                {
                    bcid = table.Column<int>(type: "int", nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    bcName = table.Column<string>(type: "varchar(255)", maxLength: 255, nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_BangCap", x => x.bcid);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "BaoCao",
                columns: table => new
                {
                    bcid = table.Column<int>(type: "int", nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    NoiDungBao = table.Column<string>(type: "varchar(255)", maxLength: 255, nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    NgayBao = table.Column<DateTime>(type: "datetime(6)", nullable: true),
                    TrangThai = table.Column<string>(type: "varchar(255)", maxLength: 255, nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_BaoCao", x => x.bcid);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "ChucDanh",
                columns: table => new
                {
                    cdid = table.Column<int>(type: "int", nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    cdName = table.Column<string>(type: "varchar(255)", maxLength: 255, nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ChucDanh", x => x.cdid);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "CongTy",
                columns: table => new
                {
                    ctid = table.Column<int>(type: "int", nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    ctName = table.Column<string>(type: "varchar(255)", maxLength: 255, nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    DiaChi = table.Column<string>(type: "varchar(255)", maxLength: 255, nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    Logo = table.Column<string>(type: "varchar(255)", maxLength: 255, nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    MieuTa = table.Column<string>(type: "varchar(255)", maxLength: 255, nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    MoHinh = table.Column<string>(type: "varchar(255)", maxLength: 255, nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    SoNhanVien = table.Column<int>(type: "int", nullable: true),
                    QuocGia = table.Column<string>(type: "varchar(255)", maxLength: 255, nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    NguoiLienHe = table.Column<string>(type: "varchar(255)", maxLength: 255, nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    sdtLienHe = table.Column<string>(type: "varchar(255)", maxLength: 255, nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    MaThue = table.Column<string>(type: "varchar(255)", maxLength: 255, nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    sdtCongTy = table.Column<string>(type: "varchar(255)", maxLength: 255, nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CongTy", x => x.ctid);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "KinhNghiem",
                columns: table => new
                {
                    knid = table.Column<int>(type: "int", nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    knName = table.Column<string>(type: "varchar(255)", maxLength: 255, nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_KinhNghiem", x => x.knid);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "KinhNgiem",
                columns: table => new
                {
                    knid = table.Column<int>(type: "int", nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    ChucVu = table.Column<string>(type: "varchar(255)", maxLength: 255, nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    TenCongTy = table.Column<string>(type: "varchar(255)", maxLength: 255, nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    TGBatDau = table.Column<DateTime>(type: "datetime(6)", nullable: true),
                    TGKetThuc = table.Column<DateTime>(type: "datetime(6)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_KinhNgiem", x => x.knid);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "LinhVuc",
                columns: table => new
                {
                    lvid = table.Column<int>(type: "int", nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    lvName = table.Column<string>(type: "varchar(255)", maxLength: 255, nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_LinhVuc", x => x.lvid);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "LoaiHinhLamViec",
                columns: table => new
                {
                    lhid = table.Column<int>(type: "int", nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    lhName = table.Column<string>(type: "varchar(255)", maxLength: 255, nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_LoaiHinhLamViec", x => x.lhid);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "NguoiDung",
                columns: table => new
                {
                    tkid = table.Column<int>(type: "int", nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    tkName = table.Column<string>(type: "varchar(255)", maxLength: 255, nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    sdt = table.Column<string>(type: "varchar(255)", maxLength: 255, nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    mail = table.Column<string>(type: "varchar(255)", maxLength: 255, nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    password = table.Column<string>(type: "varchar(255)", maxLength: 255, nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_NguoiDung", x => x.tkid);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "PhucLoi",
                columns: table => new
                {
                    plid = table.Column<int>(type: "int", nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    plName = table.Column<string>(type: "varchar(255)", maxLength: 255, nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PhucLoi", x => x.plid);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "ThongBao",
                columns: table => new
                {
                    tbid = table.Column<int>(type: "int", nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    NoiDung = table.Column<string>(type: "varchar(255)", maxLength: 255, nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    NgayBao = table.Column<DateTime>(type: "datetime(6)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ThongBao", x => x.tbid);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "ViTri",
                columns: table => new
                {
                    vtid = table.Column<int>(type: "int", nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    vtName = table.Column<string>(type: "varchar(255)", maxLength: 255, nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ViTri", x => x.vtid);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "NhaTuyenDung",
                columns: table => new
                {
                    ntdid = table.Column<int>(type: "int", nullable: false),
                    ntdName = table.Column<string>(type: "varchar(255)", maxLength: 255, nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    ctID = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_NhaTuyenDung", x => x.ntdid);
                    table.ForeignKey(
                        name: "FK_NhaTuyenDung_CongTy_ctID",
                        column: x => x.ctID,
                        principalTable: "CongTy",
                        principalColumn: "ctid");
                    table.ForeignKey(
                        name: "FK_NhaTuyenDung_NguoiDung_ntdid",
                        column: x => x.ntdid,
                        principalTable: "NguoiDung",
                        principalColumn: "tkid");
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "UngVien",
                columns: table => new
                {
                    uvid = table.Column<int>(type: "int", nullable: false),
                    uvName = table.Column<string>(type: "varchar(255)", maxLength: 255, nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    NgaySinh = table.Column<DateTime>(type: "datetime(6)", nullable: true),
                    QuocGia = table.Column<string>(type: "varchar(255)", maxLength: 255, nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    linhvucID = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_UngVien", x => x.uvid);
                    table.ForeignKey(
                        name: "FK_UngVien_LinhVuc_linhvucID",
                        column: x => x.linhvucID,
                        principalTable: "LinhVuc",
                        principalColumn: "lvid");
                    table.ForeignKey(
                        name: "FK_UngVien_NguoiDung_uvid",
                        column: x => x.uvid,
                        principalTable: "NguoiDung",
                        principalColumn: "tkid");
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "NguoiDung_ThongBao",
                columns: table => new
                {
                    nguoidungID = table.Column<int>(type: "int", nullable: false),
                    thongbaoID = table.Column<int>(type: "int", nullable: false),
                    DaXem = table.Column<bool>(type: "tinyint(1)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_NguoiDung_ThongBao", x => new { x.nguoidungID, x.thongbaoID });
                    table.ForeignKey(
                        name: "FK_NguoiDung_ThongBao_NguoiDung_nguoidungID",
                        column: x => x.nguoidungID,
                        principalTable: "NguoiDung",
                        principalColumn: "tkid");
                    table.ForeignKey(
                        name: "FK_NguoiDung_ThongBao_ThongBao_thongbaoID",
                        column: x => x.thongbaoID,
                        principalTable: "ThongBao",
                        principalColumn: "tbid");
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "TInTuyenDung",
                columns: table => new
                {
                    ttdid = table.Column<int>(type: "int", nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    TieuDe = table.Column<string>(type: "varchar(255)", maxLength: 255, nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    MieuTa = table.Column<string>(type: "varchar(255)", maxLength: 255, nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    DaDuyet = table.Column<bool>(type: "tinyint(1)", nullable: true),
                    TrangThai = table.Column<string>(type: "varchar(255)", maxLength: 255, nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    YeuCau = table.Column<int>(type: "int", nullable: true),
                    Tuoi = table.Column<int>(type: "int", nullable: true),
                    NgayDang = table.Column<DateTime>(type: "datetime(6)", nullable: true),
                    HanNop = table.Column<DateTime>(type: "datetime(6)", nullable: true),
                    loaihinhID = table.Column<int>(type: "int", nullable: true),
                    chucdanhID = table.Column<int>(type: "int", nullable: true),
                    kinhnghiemID = table.Column<int>(type: "int", nullable: true),
                    bangcapID = table.Column<int>(type: "int", nullable: true),
                    linhvucIID = table.Column<int>(type: "int", nullable: true),
                    vitriID = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TInTuyenDung", x => x.ttdid);
                    table.ForeignKey(
                        name: "FK_TInTuyenDung_BangCap_bangcapID",
                        column: x => x.bangcapID,
                        principalTable: "BangCap",
                        principalColumn: "bcid");
                    table.ForeignKey(
                        name: "FK_TInTuyenDung_ChucDanh_chucdanhID",
                        column: x => x.chucdanhID,
                        principalTable: "ChucDanh",
                        principalColumn: "cdid");
                    table.ForeignKey(
                        name: "FK_TInTuyenDung_KinhNghiem_kinhnghiemID",
                        column: x => x.kinhnghiemID,
                        principalTable: "KinhNghiem",
                        principalColumn: "knid");
                    table.ForeignKey(
                        name: "FK_TInTuyenDung_LinhVuc_linhvucIID",
                        column: x => x.linhvucIID,
                        principalTable: "LinhVuc",
                        principalColumn: "lvid");
                    table.ForeignKey(
                        name: "FK_TInTuyenDung_LoaiHinhLamViec_loaihinhID",
                        column: x => x.loaihinhID,
                        principalTable: "LoaiHinhLamViec",
                        principalColumn: "lhid");
                    table.ForeignKey(
                        name: "FK_TInTuyenDung_ViTri_vitriID",
                        column: x => x.vitriID,
                        principalTable: "ViTri",
                        principalColumn: "vtid");
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "HoSo",
                columns: table => new
                {
                    hsid = table.Column<int>(type: "int", nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    hsName = table.Column<string>(type: "varchar(255)", maxLength: 255, nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    ViTriFile = table.Column<string>(type: "varchar(255)", maxLength: 255, nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    ungvienID = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_HoSo", x => x.hsid);
                    table.ForeignKey(
                        name: "FK_HoSo_UngVien_ungvienID",
                        column: x => x.ungvienID,
                        principalTable: "UngVien",
                        principalColumn: "uvid");
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "UngVien_BaoCao",
                columns: table => new
                {
                    ungvienID = table.Column<int>(type: "int", nullable: false),
                    baocaoID = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_UngVien_BaoCao", x => new { x.ungvienID, x.baocaoID });
                    table.ForeignKey(
                        name: "FK_UngVien_BaoCao_BaoCao_baocaoID",
                        column: x => x.baocaoID,
                        principalTable: "BaoCao",
                        principalColumn: "bcid");
                    table.ForeignKey(
                        name: "FK_UngVien_BaoCao_UngVien_ungvienID",
                        column: x => x.ungvienID,
                        principalTable: "UngVien",
                        principalColumn: "uvid");
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "TInTuyenDung_PhucLoi",
                columns: table => new
                {
                    tintuyendungID = table.Column<int>(type: "int", nullable: false),
                    phucloiID = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TInTuyenDung_PhucLoi", x => new { x.tintuyendungID, x.phucloiID });
                    table.ForeignKey(
                        name: "FK_TInTuyenDung_PhucLoi_PhucLoi_phucloiID",
                        column: x => x.phucloiID,
                        principalTable: "PhucLoi",
                        principalColumn: "plid");
                    table.ForeignKey(
                        name: "FK_TInTuyenDung_PhucLoi_TInTuyenDung_tintuyendungID",
                        column: x => x.tintuyendungID,
                        principalTable: "TInTuyenDung",
                        principalColumn: "ttdid");
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "UngTuyen",
                columns: table => new
                {
                    utid = table.Column<int>(type: "int", nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    NgayNop = table.Column<DateTime>(type: "datetime(6)", nullable: true),
                    TrangThai = table.Column<string>(type: "varchar(255)", maxLength: 255, nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    tintuyendungid = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_UngTuyen", x => x.utid);
                    table.ForeignKey(
                        name: "FK_UngTuyen_TInTuyenDung_tintuyendungid",
                        column: x => x.tintuyendungid,
                        principalTable: "TInTuyenDung",
                        principalColumn: "ttdid");
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "NoiDungHoSo",
                columns: table => new
                {
                    ndid = table.Column<int>(type: "int", nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    TenUngVien = table.Column<string>(type: "varchar(255)", maxLength: 255, nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    PhoneHoSo = table.Column<string>(type: "varchar(255)", maxLength: 255, nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    MailHoSo = table.Column<string>(type: "varchar(255)", maxLength: 255, nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    HocVan = table.Column<string>(type: "varchar(255)", maxLength: 255, nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    NamKinhNghiem = table.Column<int>(type: "int", nullable: true),
                    hosoID = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_NoiDungHoSo", x => x.ndid);
                    table.ForeignKey(
                        name: "FK_NoiDungHoSo_HoSo_hosoID",
                        column: x => x.hosoID,
                        principalTable: "HoSo",
                        principalColumn: "hsid");
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "HoSo_UngTuyen",
                columns: table => new
                {
                    hosoID = table.Column<int>(type: "int", nullable: false),
                    ungtuyenID = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_HoSo_UngTuyen", x => new { x.hosoID, x.ungtuyenID });
                    table.ForeignKey(
                        name: "FK_HoSo_UngTuyen_HoSo_hosoID",
                        column: x => x.hosoID,
                        principalTable: "HoSo",
                        principalColumn: "hsid");
                    table.ForeignKey(
                        name: "FK_HoSo_UngTuyen_UngTuyen_ungtuyenID",
                        column: x => x.ungtuyenID,
                        principalTable: "UngTuyen",
                        principalColumn: "utid");
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "UngVien_UngTuyen",
                columns: table => new
                {
                    ungvienID = table.Column<int>(type: "int", nullable: false),
                    ungtuyenID = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_UngVien_UngTuyen", x => new { x.ungvienID, x.ungtuyenID });
                    table.ForeignKey(
                        name: "FK_UngVien_UngTuyen_UngTuyen_ungtuyenID",
                        column: x => x.ungtuyenID,
                        principalTable: "UngTuyen",
                        principalColumn: "utid");
                    table.ForeignKey(
                        name: "FK_UngVien_UngTuyen_UngVien_ungvienID",
                        column: x => x.ungvienID,
                        principalTable: "UngVien",
                        principalColumn: "uvid");
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "NoiDung_KinhNghiem",
                columns: table => new
                {
                    noidungID = table.Column<int>(type: "int", nullable: false),
                    kinhnghiemID = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_NoiDung_KinhNghiem", x => new { x.noidungID, x.kinhnghiemID });
                    table.ForeignKey(
                        name: "FK_NoiDung_KinhNghiem_KinhNgiem_kinhnghiemID",
                        column: x => x.kinhnghiemID,
                        principalTable: "KinhNgiem",
                        principalColumn: "knid");
                    table.ForeignKey(
                        name: "FK_NoiDung_KinhNghiem_NoiDungHoSo_noidungID",
                        column: x => x.noidungID,
                        principalTable: "NoiDungHoSo",
                        principalColumn: "ndid");
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateIndex(
                name: "IX_HoSo_ungvienID",
                table: "HoSo",
                column: "ungvienID");

            migrationBuilder.CreateIndex(
                name: "IX_HoSo_UngTuyen_ungtuyenID",
                table: "HoSo_UngTuyen",
                column: "ungtuyenID");

            migrationBuilder.CreateIndex(
                name: "IX_NguoiDung_ThongBao_thongbaoID",
                table: "NguoiDung_ThongBao",
                column: "thongbaoID");

            migrationBuilder.CreateIndex(
                name: "IX_NhaTuyenDung_ctID",
                table: "NhaTuyenDung",
                column: "ctID");

            migrationBuilder.CreateIndex(
                name: "IX_NoiDung_KinhNghiem_kinhnghiemID",
                table: "NoiDung_KinhNghiem",
                column: "kinhnghiemID");

            migrationBuilder.CreateIndex(
                name: "IX_NoiDungHoSo_hosoID",
                table: "NoiDungHoSo",
                column: "hosoID");

            migrationBuilder.CreateIndex(
                name: "IX_TInTuyenDung_bangcapID",
                table: "TInTuyenDung",
                column: "bangcapID");

            migrationBuilder.CreateIndex(
                name: "IX_TInTuyenDung_chucdanhID",
                table: "TInTuyenDung",
                column: "chucdanhID");

            migrationBuilder.CreateIndex(
                name: "IX_TInTuyenDung_kinhnghiemID",
                table: "TInTuyenDung",
                column: "kinhnghiemID");

            migrationBuilder.CreateIndex(
                name: "IX_TInTuyenDung_linhvucIID",
                table: "TInTuyenDung",
                column: "linhvucIID");

            migrationBuilder.CreateIndex(
                name: "IX_TInTuyenDung_loaihinhID",
                table: "TInTuyenDung",
                column: "loaihinhID");

            migrationBuilder.CreateIndex(
                name: "IX_TInTuyenDung_vitriID",
                table: "TInTuyenDung",
                column: "vitriID");

            migrationBuilder.CreateIndex(
                name: "IX_TInTuyenDung_PhucLoi_phucloiID",
                table: "TInTuyenDung_PhucLoi",
                column: "phucloiID");

            migrationBuilder.CreateIndex(
                name: "IX_UngTuyen_tintuyendungid",
                table: "UngTuyen",
                column: "tintuyendungid");

            migrationBuilder.CreateIndex(
                name: "IX_UngVien_linhvucID",
                table: "UngVien",
                column: "linhvucID");

            migrationBuilder.CreateIndex(
                name: "IX_UngVien_BaoCao_baocaoID",
                table: "UngVien_BaoCao",
                column: "baocaoID");

            migrationBuilder.CreateIndex(
                name: "IX_UngVien_UngTuyen_ungtuyenID",
                table: "UngVien_UngTuyen",
                column: "ungtuyenID");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "HoSo_UngTuyen");

            migrationBuilder.DropTable(
                name: "NguoiDung_ThongBao");

            migrationBuilder.DropTable(
                name: "NhaTuyenDung");

            migrationBuilder.DropTable(
                name: "NoiDung_KinhNghiem");

            migrationBuilder.DropTable(
                name: "TInTuyenDung_PhucLoi");

            migrationBuilder.DropTable(
                name: "UngVien_BaoCao");

            migrationBuilder.DropTable(
                name: "UngVien_UngTuyen");

            migrationBuilder.DropTable(
                name: "ThongBao");

            migrationBuilder.DropTable(
                name: "CongTy");

            migrationBuilder.DropTable(
                name: "KinhNgiem");

            migrationBuilder.DropTable(
                name: "NoiDungHoSo");

            migrationBuilder.DropTable(
                name: "PhucLoi");

            migrationBuilder.DropTable(
                name: "BaoCao");

            migrationBuilder.DropTable(
                name: "UngTuyen");

            migrationBuilder.DropTable(
                name: "HoSo");

            migrationBuilder.DropTable(
                name: "TInTuyenDung");

            migrationBuilder.DropTable(
                name: "UngVien");

            migrationBuilder.DropTable(
                name: "BangCap");

            migrationBuilder.DropTable(
                name: "ChucDanh");

            migrationBuilder.DropTable(
                name: "KinhNghiem");

            migrationBuilder.DropTable(
                name: "LoaiHinhLamViec");

            migrationBuilder.DropTable(
                name: "ViTri");

            migrationBuilder.DropTable(
                name: "LinhVuc");

            migrationBuilder.DropTable(
                name: "NguoiDung");
        }
    }
}
