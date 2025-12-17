using System.ComponentModel.DataAnnotations;

namespace TImViecAPI.Model_Function.Dtos
{
    public class CapNhatCongTyDto
    {
        [Required(ErrorMessage = "Tên công ty là bắt buộc.")]
        [StringLength(255, MinimumLength = 5, ErrorMessage = "Tên công ty phải từ 5 đến 255 ký tự.")]
        public string CtName { get; set; } = null!;

        [Required(ErrorMessage = "Địa chỉ là bắt buộc.")]
        public string? DiaChi { get; set; }

        
        public string? Logo { get; set; }

        [StringLength(1000, ErrorMessage = "Mô tả công ty không được quá 1000 ký tự.")]
        public string? MieuTa { get; set; }

        [Required(ErrorMessage = "Mô hình công ty la bắt buộc.")]
        public string? MoHinh { get; set; }

        [Required(ErrorMessage  = "Số nhân viên là bắt buộc.")]
        public int? SoNhanVien { get; set; }

        [Required(ErrorMessage = "Quốc gia là bắt buộc.")]
        public string? QuocGia { get; set; }

        [Required(ErrorMessage = "Người liên hệ là bắt buộc.")]
        [StringLength(100, ErrorMessage = "Tên người liên hệ không được quá 100 ký tự.")]
        public string? NguoiLienHe { get; set; }

        [Required(ErrorMessage = "Số điện thoại liên hệ là bắt buộc.")]
        [Phone(ErrorMessage = "Số điện thoại liên hệ không hợp lệ.")]
        [StringLength(10, ErrorMessage = "Số điện thoại không được quá 10 ký tự.")]
        public string? SdtLienHe { get; set; }


        [Required(ErrorMessage = "Mã số thuế là bắt buộc.")]

        [RegularExpression(@"^\d{10}(\d{3})?$", ErrorMessage = "Mã số thuế phải gồm 10 hoặc 13 chữ số.")]
        public string? MaThue { get; set; }

        [Required(ErrorMessage = "Số điện thoại công ty là bắt buộc.")]
        [Phone(ErrorMessage = "Số điện thoại công ty không hợp lệ.")]
        [StringLength(10, ErrorMessage = "Số điện thoại không được quá 10 ký tự.")]
        public string? SdtCongTy { get; set; }
    }
}
