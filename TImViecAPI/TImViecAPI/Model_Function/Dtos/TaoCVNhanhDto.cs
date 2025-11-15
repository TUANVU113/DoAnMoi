using System.ComponentModel.DataAnnotations;

namespace TImViecAPI.Model_Function.Dtos
{
    public class TaoCVNhanhDto
    {
        [Required(ErrorMessage = "Tên hồ sơ là bắt buộc")]
        [StringLength(255)]
        public string TenHoSo { get; set; } = null!;

        [Required(ErrorMessage = "Tên ứng viên là bắt buộc")]
        [StringLength(255)]
        public string TenUngVien { get; set; } = null!;

        [Required]
        [Phone(ErrorMessage = "Số điện thoại không hợp lệ")]
        [StringLength(20)]
        public string PhoneHoSo { get; set; } = null!;

        [Required]
        [EmailAddress(ErrorMessage = "Email không hợp lệ")]
        [StringLength(255)]
        public string MailHoSo { get; set; } = null!;

        public string? HocVan { get; set; }

        [Range(0, 50, ErrorMessage = "Kinh nghiệm từ 0 đến 50 năm")]
        public int? NamKinhNghiemID { get; set; }

        [Range(0, 100, ErrorMessage = "Mức lương từ 0 đến 100 triệu")]
        public int? MucLuong { get; set; }

        public int? ChucDanhID { get; set; }
        public int? LoaiHinhLamViecID { get; set; }
        public int? LinhVucID { get; set; }
        public int? ViTriLamViecID { get; set; }


        public string? KyNang { get; set; }
        public string? MucTieu { get; set; }
        public string? ChucChi { get; set; }
        public string? Avata { get; set; }

        // AVATAR: File upload
        public IFormFile? AvatarFile { get; set; }
    }
}
