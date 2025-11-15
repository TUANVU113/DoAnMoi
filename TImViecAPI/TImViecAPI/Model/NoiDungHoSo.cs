using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace TImViecAPI.Model
{
    [Table("NoiDungHoSo")]
    public class NoiDungHoSo
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int ndid { get; set; }

        [StringLength(255)]
        public string? TenUngVien { get; set; }

        [StringLength(255)]
        public string? PhoneHoSo { get; set; }

        [StringLength(255)]
        public string? MailHoSo { get; set; }

        public string? HocVan { get; set; } // longtext

        // === THAY ĐỔI: DÙNG ID + FK ===
        public int? NamKinhNghiemID { get; set; } // Thay cho NamKinhNghiem (varchar)

        public int? MucLuong { get; set; }

        public int? ChucDanhID { get; set; }
        public int? LoaiHinhLamViecID { get; set; }
        public int? LinhVucID { get; set; }
        public int? ViTriLamViecID { get; set; }

        public string? KyNang { get; set; }
        public string? MucTieu { get; set; } // longtext
        public string? ChucChi { get; set; } // longtext
        [StringLength(255)]
        public string? Avata { get; set; }

        // === FK CHÍNH ===
       
        public int? hosoID { get; set; }

        [ForeignKey("hosoID")]
        public HoSo? HoSo { get; set; }

        // === KHÓA NGOẠI MỚI (THÊM NAVIGATION) ===
        [ForeignKey("LinhVucID")]
        public LinhVuc? LinhVuc { get; set; }

        [ForeignKey("LoaiHinhLamViecID")]
        public LoaiHinhLamViec? LoaiHinhLamViec { get; set; }

        [ForeignKey("ChucDanhID")]
        public ChucDanh? ChucDanh { get; set; }

        [ForeignKey("ViTriLamViecID")]
        public ViTri? ViTriLamViec { get; set; }

        [ForeignKey("NamKinhNghiemID")]
        public KinhNghiem? KinhNghiem { get; set; } // Map với knName: "Dưới 1 năm", "1-3 năm",...
    }
}