using System.ComponentModel.DataAnnotations;

namespace TImViecAPI.Model_Function.Dtos
{
    public class NoiDungHoSoDto
    {
        [Required(ErrorMessage = "Tên ứng viên là bắt buộc.")]
        public string? TenUngVien { get; set; }
        [Required(ErrorMessage = "Giới tính là bắt buộc.")]
        public string? GioiTinh { get; set; }
        [Required(ErrorMessage = "Ngày sinh là bắt buộc.")]
        public string? NgaySinh { get; set; }

        [Required(ErrorMessage = "Số điện thoại là bắt buộc.")]
        [Phone(ErrorMessage = "Định dạng số điện thoại không hợp lệ.")]
        public string? PhoneHoSo { get; set; }

        [Required(ErrorMessage = "Email là bắt buộc.")]
        [EmailAddress(ErrorMessage = "Định dạng email không hợp lệ.")]
        public string? MailHoSo { get; set; }

        [Required(ErrorMessage = "Quốc gia là bắt buộc.")]
        public string? QuocGia { get; set; }

        [Required(ErrorMessage = "Tỉnh là bắt buộc.")]
        public string? Tinh { get; set; }
        [Required(ErrorMessage = "Quận huyện là bắt buộc.")]
        public string? QuanHuyen { get; set; }
        [Required(ErrorMessage = "Địa chỉ là bắt buộc.")]
        public string? DiaChi { get; set; }
        [Required(ErrorMessage = "Học vấn là bắt buộc.")]
        public string? HocVan { get; set; }

        [Required(ErrorMessage = "ID hồ sơ là bắt buộc.")]
        public int HosoID { get; set; }
    }
}
