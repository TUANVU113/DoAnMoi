using System.ComponentModel.DataAnnotations;
using System.Text.RegularExpressions;

namespace TImViecAPI.Model_Function.Dtos
{
    public class ThongTinCaNhanCreateDto : IValidatableObject
    {
        [Required(ErrorMessage = "Họ và tên là bắt buộc")]
        [StringLength(255, ErrorMessage = "Họ và tên không được vượt quá 255 ký tự")]
        public string HoVaTen { get; set; } = string.Empty;

        [Required(ErrorMessage = "Giới tính là bắt buộc")]
        [StringLength(10, ErrorMessage = "Giới tính không được vượt quá 10 ký tự")]
        public string GioiTinh { get; set; } = string.Empty;

        [Required(ErrorMessage = "Ngày sinh là bắt buộc")]
        public DateTime NgaySinh { get; set; }

        [Required(ErrorMessage = "Số điện thoại là bắt buộc")]
        public string SDT { get; set; } = string.Empty;

        [Required(ErrorMessage = "Email là bắt buộc")]
        [EmailAddress(ErrorMessage = "Email không đúng định dạng")]
        [StringLength(255, ErrorMessage = "Email không được vượt quá 255 ký tự")]
        public string Email { get; set; } = string.Empty;

        [StringLength(100, ErrorMessage = "Quốc gia không được vượt quá 100 ký tự")]
        public string? QuocGia { get; set; } = "Việt Nam";

        [Required(ErrorMessage = "Tỉnh/Thành phố là bắt buộc")]
        [StringLength(100, ErrorMessage = "Tỉnh/Thành phố không được vượt quá 100 ký tự")]
        public string Tinh { get; set; } = string.Empty;

        [StringLength(100, ErrorMessage = "Huyện không được vượt quá 100 ký tự")]
        public string? Huyen { get; set; }

        [StringLength(500, ErrorMessage = "Địa chỉ không được vượt quá 500 ký tự")]
        public string? DiaChi { get; set; }

        [Required(ErrorMessage = "CCCD là bắt buộc")]
        [StringLength(12, MinimumLength = 12, ErrorMessage = "CCCD phải có đúng 12 chữ số")]
        [RegularExpression(@"^\d{12}$", ErrorMessage = "CCCD chỉ được chứa 12 chữ số")]
        public string CCCD { get; set; } = string.Empty;

        [StringLength(255, ErrorMessage = "Nơi sinh không được vượt quá 255 ký tự")]
        public string? NoiSinh { get; set; }

        //[Required(ErrorMessage = "ID ứng viên là bắt buộc")]
        //[Range(1, int.MaxValue, ErrorMessage = "ID ứng viên phải lớn hơn 0")]
        //public int ungvienID { get; set; }

        // RÀNG BUỘC TÙY CHỈNH: Tuổi >= 18 + SĐT đúng 10 chữ số
        public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
        {
            // 1. Tuổi >= 18
            var age = DateTime.Today.Year - NgaySinh.Year;
            if (NgaySinh.Date > DateTime.Today.AddYears(-age))
                age--; // Chưa đến sinh nhật

            if (age < 18)
            {
                yield return new ValidationResult(
                    "Ứng viên phải từ 18 tuổi trở lên.",
                    new[] { nameof(NgaySinh) }
                );
            }

            // 2. SĐT: chỉ cần đúng 10 chữ số (không cần đầu số)
            var cleanPhone = Regex.Replace(SDT.Trim(), @"[^0-9]", "");
            if (cleanPhone.Length != 10)
            {
                yield return new ValidationResult(
                    "Số điện thoại phải có đúng 10 chữ số.",
                    new[] { nameof(SDT) }
                );
            }
        }
    }
}