using System.ComponentModel.DataAnnotations;

namespace TImViecAPI.Model_Function.Dtos
{
    public class LoginDto
    {
        [Required(ErrorMessage = "Email là bắt buộc.")]
        [StringLength(255)]
        [EmailAddress(ErrorMessage = "Email phải đúng định dạng (ví dụ: user@example.com).")]
        public string Mail { get; set; }

        [Required(ErrorMessage = "Mật khẩu là bắt buộc.")]
        [StringLength(255, MinimumLength = 6, ErrorMessage = "Mật khẩu phải có ít nhất 6 ký tự.")]
        public string Password { get; set; }
    }
}
