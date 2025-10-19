using System.ComponentModel.DataAnnotations;

namespace TImViecAPI.Model_Function.Dtos
{
    public class RegisterNTDDto
    {
        // Thông tin NguoiDung
        [Required(ErrorMessage = "Tên tài khoản là bắt buộc.")]
        [StringLength(255)]
        public string TkName { get; set; }

        [Required(ErrorMessage = "Số điện thoại là bắt buộc.")]
        [StringLength(10, MinimumLength = 10)]
        [RegularExpression(@"^\d{10}$", ErrorMessage = "Số điện thoại phải là 10 chữ số.")]
        public string Sdt { get; set; }

        [Required(ErrorMessage = "Email là bắt buộc.")]
        [EmailAddress]
        [StringLength(255)]
        public string Mail { get; set; }

        [Required(ErrorMessage = "Mật khẩu là bắt buộc.")]
        [MinLength(6)]
        [RegularExpression(@"^(?=.*[a-zA-Z])(?=.*\d).+$", ErrorMessage = "Mật khẩu phải chứa chữ và số.")]
        public string Password { get; set; }

        // Thông tin NhaTuyenDung
        [StringLength(255)]
        public string? NtdName { get; set; }  // Tùy chọn

        public int? CtID { get; set; }  // FK đến CongTy.ctid, tùy chọn
    }
}
