using System.ComponentModel.DataAnnotations;

namespace TImViecAPI.Model_Function.Dtos
{
    public class UngTuyenRequestDto
    {
        [Required] public int TinTuyenDungId { get; set; }  // ID tin tuyển dụng
        [Required] public int HoSoId { get; set; }         // ID hồ sơ ứng viên chọn

        // THÔNG TIN CÁ NHÂN (tùy chọn)
        public string? HoVaTen { get; set; }
        public string? GioiTinh { get; set; }
        public DateTime? NgaySinh { get; set; }
        public string? SDT { get; set; }
        public string? Email { get; set; }
        public string? QuocGia { get; set; }
        public string? Tinh { get; set; }
        public string? Huyen { get; set; }
        public string? DiaChi { get; set; }
        public string? CCCD { get; set; }
        public string? NoiSinh { get; set; }
    }
}