using System.ComponentModel.DataAnnotations;

namespace TImViecAPI.Model_Function.Dtos
{
    public class RegisterAdminDto
    {
        // Thông tin chung (NguoiDung)
        [Required(ErrorMessage = "Tên tài khoản là bắt buộc.")]
        [StringLength(255, MinimumLength = 1, ErrorMessage = "Tên tài khoản phải từ 1 đến 255 ký tự.")]
        public string TkName { get; set; }

        [Required(ErrorMessage = "Số điện thoại là bắt buộc.")]
        [StringLength(10, MinimumLength = 10, ErrorMessage = "Số điện thoại phải đúng 10 số.")]
        [RegularExpression(@"^\d{10}$", ErrorMessage = "Số điện thoại phải là 10 chữ số (ví dụ: 0123456789).")]
        public string Sdt { get; set; }

        [Required(ErrorMessage = "Email là bắt buộc.")]
        [StringLength(255)]
        [EmailAddress(ErrorMessage = "Email phải đúng định dạng (ví dụ: user@example.com).")]
        public string Mail { get; set; }

        [Required(ErrorMessage = "Mật khẩu là bắt buộc.")]
        [StringLength(255, MinimumLength = 6, ErrorMessage = "Mật khẩu phải có ít nhất 6 ký tự.")]
        [RegularExpression(@"^(?=.*[a-zA-Z])(?=.*\d).+$", ErrorMessage = "Mật khẩu phải chứa ít nhất một chữ cái và một số.")]
        public string Password { get; set; }
    }
}
