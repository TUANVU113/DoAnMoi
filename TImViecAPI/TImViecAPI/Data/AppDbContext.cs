using Microsoft.EntityFrameworkCore;
using TImViecAPI.Model;
using TImViecAPI.Models;

namespace TImViecAPI.Data
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { 

        }
        public DbSet<NguoiDung> NguoiDung { get; set; }
        public DbSet<UngVien> UngVien { get; set; }
        public DbSet<NhaTuyenDung> NhaTuyenDung { get; set; }
        public DbSet<CongTy> CongTy { get; set; }
        public DbSet<PhucLoi> PhucLoi { get; set; }
        public DbSet<TInTuyenDung_PhucLoi> TInTuyenDung_PhucLoi { get; set; }
        public DbSet<BangCap> BangCap { get; set; }
        public DbSet<ThongBao> ThongBao { get; set; }
        public DbSet<NguoiDung_ThongBao> NguoiDung_ThongBao { get; set; }
        public DbSet<UngTuyen> UngTuyen { get; set; }
        public DbSet<UngVien_UngTuyen> UngVien_UngTuyen { get; set; }
        public DbSet<TInTuyenDung> TInTuyenDung { get; set; }
        public DbSet<HoSo> HoSo { get; set; }
        public DbSet<LoaiHinhLamViec> LoaiHinhLamViec { get; set; }
        public DbSet<ChucDanh> ChucDanh { get; set; }
        public DbSet<ViTri> ViTri { get; set; }
        public DbSet<KinhNghiem> KinhNghiem { get; set; }
        public DbSet<LinhVuc> LinhVuc { get; set; }
        public DbSet<NoiDungHoSo> NoiDungHoSo { get; set; }
        public DbSet<HoSo_UngTuyen> HoSo_UngTuyen { get; set; }
        public DbSet<KinhNgiem> KinhNgiem { get; set; }
        public DbSet<NoiDung_KinhNghiem> NoiDung_KinhNghiem { get; set; }
        public DbSet<BaoCao> BaoCao { get; set; }
        public DbSet<UngVien_BaoCao> UngVien_BaoCao { get; set; }
        public DbSet<PasswordResetToken> PasswordResetTokens { get; set; }
        public DbSet<ThongTinCaNhan> thongTinCaNhans { get; set; }
        public DbSet<CongViecYeuThich> CongViecYeuThich { get; set; } = null!;


        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            // NguoiDung - UngVien (1:1)
            modelBuilder.Entity<UngVien>()
                .HasOne(uv => uv.NguoiDung)
                .WithOne()
                .HasForeignKey<UngVien>(uv => uv.uvid)
                .OnDelete(DeleteBehavior.NoAction);

            // NguoiDung - NhaTuyenDung (1:1)
            modelBuilder.Entity<NhaTuyenDung>()
                .HasOne(ntd => ntd.NguoiDung)
                .WithOne()
                .HasForeignKey<NhaTuyenDung>(ntd => ntd.ntdid)
                .OnDelete(DeleteBehavior.NoAction);

            // UngVien - LinhVuc (Nhiều-đến-Một)
            modelBuilder.Entity<UngVien>()
                .HasOne(uv => uv.LinhVuc)
                .WithMany()
                .HasForeignKey(uv => uv.linhvucID)
                .OnDelete(DeleteBehavior.NoAction);

            // NhaTuyenDung - CongTy (Nhiều-đến-Một)
            modelBuilder.Entity<NhaTuyenDung>()
                .HasOne(ntd => ntd.CongTy)
                .WithMany()
                .HasForeignKey(ntd => ntd.ctID)
                .OnDelete(DeleteBehavior.NoAction);

            // TInTuyenDung_PhucLoi (Nhiều-đến-Nhiều)
            modelBuilder.Entity<TInTuyenDung_PhucLoi>()
                .HasKey(t => new { t.tintuyendungID, t.phucloiID });

            modelBuilder.Entity<TInTuyenDung_PhucLoi>()
                .HasOne(t => t.TInTuyenDung)
                .WithMany()
                .HasForeignKey(t => t.tintuyendungID)
                .OnDelete(DeleteBehavior.NoAction);

            modelBuilder.Entity<TInTuyenDung_PhucLoi>()
                .HasOne(t => t.PhucLoi)
                .WithMany()
                .HasForeignKey(t => t.phucloiID)
                .OnDelete(DeleteBehavior.NoAction);

            // NguoiDung_ThongBao (Nhiều-đến-Nhiều)
            modelBuilder.Entity<NguoiDung_ThongBao>()
                .HasKey(t => new { t.nguoidungID, t.thongbaoID });

            modelBuilder.Entity<NguoiDung_ThongBao>()
                .HasOne(t => t.NguoiDung)
                .WithMany()
                .HasForeignKey(t => t.nguoidungID)
                .OnDelete(DeleteBehavior.NoAction);

            modelBuilder.Entity<NguoiDung_ThongBao>()
                .HasOne(t => t.ThongBao)
                .WithMany()
                .HasForeignKey(t => t.thongbaoID)
                .OnDelete(DeleteBehavior.NoAction);

            // UngTuyen - TInTuyenDung (Nhiều-đến-Một)
            modelBuilder.Entity<UngTuyen>()
                .HasOne(ut => ut.TInTuyenDung)
                .WithMany()
                .HasForeignKey(ut => ut.tintuyendungid)
                .OnDelete(DeleteBehavior.NoAction);

            // UngVien_UngTuyen (Nhiều-đến-Nhiều)
            modelBuilder.Entity<UngVien_UngTuyen>()
                .HasKey(t => new { t.ungvienID, t.ungtuyenID });

            modelBuilder.Entity<UngVien_UngTuyen>()
                .HasOne(t => t.UngVien)
                .WithMany()
                .HasForeignKey(t => t.ungvienID)
                .OnDelete(DeleteBehavior.NoAction);

            modelBuilder.Entity<UngVien_UngTuyen>()
                .HasOne(t => t.UngTuyen)
                .WithMany()
                .HasForeignKey(t => t.ungtuyenID)
                .OnDelete(DeleteBehavior.NoAction);

            // HoSo - UngVien (Nhiều-đến-Một)
            modelBuilder.Entity<HoSo>()
                .HasOne(hs => hs.UngVien)
                .WithMany()
                .HasForeignKey(hs => hs.ungvienID)
                .OnDelete(DeleteBehavior.NoAction);

            // TInTuyenDung - LoaiHinhLamViec (Nhiều-đến-Một)
            modelBuilder.Entity<TInTuyenDung>()
                .HasOne(ttd => ttd.LoaiHinhLamViec)
                .WithMany()
                .HasForeignKey(ttd => ttd.loaihinhID)
                .OnDelete(DeleteBehavior.NoAction);

            // TInTuyenDung - ChucDanh (Nhiều-đến-Một)
            modelBuilder.Entity<TInTuyenDung>()
                .HasOne(ttd => ttd.ChucDanh)
                .WithMany()
                .HasForeignKey(ttd => ttd.chucdanhID)
                .OnDelete(DeleteBehavior.NoAction);

            // TInTuyenDung - KinhNghiem (Nhiều-đến-Một)
            modelBuilder.Entity<TInTuyenDung>()
                .HasOne(ttd => ttd.KinhNghiem)
                .WithMany()
                .HasForeignKey(ttd => ttd.kinhnghiemID)
                .OnDelete(DeleteBehavior.NoAction);

            // TInTuyenDung - BangCap (Nhiều-đến-Một)
            modelBuilder.Entity<TInTuyenDung>()
                .HasOne(ttd => ttd.BangCap)
                .WithMany()
                .HasForeignKey(ttd => ttd.bangcapID)
                .OnDelete(DeleteBehavior.NoAction);

            // TInTuyenDung - LinhVuc (Nhiều-đến-Một)
            modelBuilder.Entity<TInTuyenDung>()
                .HasOne(ttd => ttd.LinhVuc)
                .WithMany()
                .HasForeignKey(ttd => ttd.linhvucIID)
                .OnDelete(DeleteBehavior.NoAction);

            // TInTuyenDung - ViTri (Nhiều-đến-Một)
            modelBuilder.Entity<TInTuyenDung>()
                .HasOne(ttd => ttd.ViTri)
                .WithMany()
                .HasForeignKey(ttd => ttd.vitriID)
                .OnDelete(DeleteBehavior.NoAction);

            modelBuilder.Entity<TInTuyenDung>()
                .HasOne(ttd => ttd.NhaTuyenDung)
                .WithMany()  // Thêm navigation ngược ở NhaTuyenDung nếu cần: 
                .HasForeignKey(ttd => ttd.nhaTuyenDungID)
                .OnDelete(DeleteBehavior.NoAction);

            // HoSo_UngTuyen (Nhiều-đến-Nhiều)
            modelBuilder.Entity<HoSo_UngTuyen>()
                .HasKey(t => new { t.hosoID, t.ungtuyenID });

            modelBuilder.Entity<HoSo_UngTuyen>()
                .HasOne(t => t.HoSo)
                .WithMany()
                .HasForeignKey(t => t.hosoID)
                .OnDelete(DeleteBehavior.NoAction);

            modelBuilder.Entity<HoSo_UngTuyen>()
                .HasOne(t => t.UngTuyen)
                .WithMany()
                .HasForeignKey(t => t.ungtuyenID)
                .OnDelete(DeleteBehavior.NoAction);

            // === NoiDungHoSo - HoSo (Nhiều-đến-Một) ===
          
            // SỬA LỖI JOIN SAI: HOÀN CHỈNH QUAN HỆ 1-1
            modelBuilder.Entity<HoSo>()
                .HasOne(h => h.NoiDungHoSo)
                .WithOne(nd => nd.HoSo)
                .HasForeignKey<NoiDungHoSo>(nd => nd.hosoID)
                .HasConstraintName("FK_NoiDungHoSo_HoSo")
                .OnDelete(DeleteBehavior.Cascade);

            // === NoiDungHoSo - CÁC KHÓA NGOẠI MỚI (THÊM MỚI) ===
            modelBuilder.Entity<NoiDungHoSo>(entity =>
            {
                entity.HasKey(e => e.ndid);

                entity.Property(e => e.TenUngVien).HasMaxLength(255);
                entity.Property(e => e.PhoneHoSo).HasMaxLength(255);
                entity.Property(e => e.MailHoSo).HasMaxLength(255);
                entity.Property(e => e.Avata).HasMaxLength(255);

                // LinhVuc
                entity.HasOne(nd => nd.LinhVuc)
                      .WithMany()
                      .HasForeignKey(nd => nd.LinhVucID)
                      .OnDelete(DeleteBehavior.SetNull)
                      .HasConstraintName("FK_NoiDungHoSo_LinhVuc");

                // LoaiHinhLamViec
                entity.HasOne(nd => nd.LoaiHinhLamViec)
                      .WithMany()
                      .HasForeignKey(nd => nd.LoaiHinhLamViecID)
                      .OnDelete(DeleteBehavior.SetNull)
                      .HasConstraintName("FK_NoiDungHoSo_LoaiHinhLamViec");

                // ChucDanh
                entity.HasOne(nd => nd.ChucDanh)
                      .WithMany()
                      .HasForeignKey(nd => nd.ChucDanhID)
                      .OnDelete(DeleteBehavior.SetNull)
                      .HasConstraintName("FK_NoiDungHoSo_ChucDanh");

                // ViTriLamViec
                entity.HasOne(nd => nd.ViTriLamViec)
                      .WithMany()
                      .HasForeignKey(nd => nd.ViTriLamViecID)
                      .OnDelete(DeleteBehavior.SetNull)
                      .HasConstraintName("FK_NoiDungHoSo_ViTri");

                // NamKinhNghiemID → KinhNghiem
                entity.HasOne(nd => nd.KinhNghiem)
                      .WithMany()
                      .HasForeignKey(nd => nd.NamKinhNghiemID)
                      .OnDelete(DeleteBehavior.SetNull)
                      .HasConstraintName("FK_NoiDungHoSo_KinhNghiem");
            });

            // NoiDung_KinhNghiem (Nhiều-đến-Nhiều)
            modelBuilder.Entity<NoiDung_KinhNghiem>()
                .HasKey(t => new { t.noidungID, t.kinhnghiemID });

            modelBuilder.Entity<NoiDung_KinhNghiem>()
                .HasOne(t => t.NoiDungHoSo)
                .WithMany()
                .HasForeignKey(t => t.noidungID)
                .OnDelete(DeleteBehavior.NoAction);

            modelBuilder.Entity<NoiDung_KinhNghiem>()
                .HasOne(t => t.KinhNgiem)
                .WithMany()
                .HasForeignKey(t => t.kinhnghiemID)
                .OnDelete(DeleteBehavior.NoAction);

            // UngVien_BaoCao (Nhiều-đến-Nhiều)
            modelBuilder.Entity<UngVien_BaoCao>()
                .HasKey(t => new { t.ungvienID, t.baocaoID });

            modelBuilder.Entity<UngVien_BaoCao>()
                .HasOne(t => t.UngVien)
                .WithMany()
                .HasForeignKey(t => t.ungvienID)
                .OnDelete(DeleteBehavior.NoAction);

            modelBuilder.Entity<UngVien_BaoCao>()
                .HasOne(t => t.BaoCao)
                .WithMany()
                .HasForeignKey(t => t.baocaoID)
                .OnDelete(DeleteBehavior.NoAction);

            modelBuilder.Entity<PasswordResetToken>()
                .HasOne(pt => pt.NguoiDung)
                .WithMany() // NguoiDung có thể có nhiều PasswordResetToken
                .HasForeignKey(pt => pt.TkId)
                .OnDelete(DeleteBehavior.Cascade); // Xóa token khi NguoiDung bị xóa
                                                  
            // Cấu hình quan hệ 1-1 giữa UngVien và ThongTinCaNhan
            modelBuilder.Entity<ThongTinCaNhan>()
                .HasOne(tt => tt.UngVien)           // ThongTinCaNhan có 1 UngVien
                .WithOne(uv => uv.ThongTinCaNhan)   // UngVien có 1 ThongTinCaNhan (nếu bạn thêm navigation ngược)
                .HasForeignKey<ThongTinCaNhan>(tt => tt.ungvienID)  // Khóa ngoại ở ThongTinCaNhan
                .OnDelete(DeleteBehavior.Cascade);

            // === CongViecYeuThich (Nhiều-đến-Nhiều: Ứng viên lưu nhiều tin, 1 tin được nhiều ứng viên lưu) ===
            modelBuilder.Entity<CongViecYeuThich>()
                .HasKey(cv => new { cv.ungvienID, cv.tintuyenID });

            modelBuilder.Entity<CongViecYeuThich>()
                .HasOne(cv => cv.UngVien)
                .WithMany(uv => uv.CongViecYeuThichs) // ← Đảm bảo bạn có navigation này trong UngVien.cs
                .HasForeignKey(cv => cv.ungvienID)
                .OnDelete(DeleteBehavior.Cascade); // Xóa yêu thích nếu xóa ứng viên

            modelBuilder.Entity<CongViecYeuThich>()
                .HasOne(cv => cv.TinTuyenDung)
                .WithMany(t => t.CongViecYeuThichs) // ← Đảm bảo bạn có navigation này trong TInTuyenDung.cs
                .HasForeignKey(cv => cv.tintuyenID)
                .OnDelete(DeleteBehavior.Cascade); // Xóa yêu thích nếu xóa tin

        }
    }
}
