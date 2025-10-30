// DTO/ThongTinCaNhanCreateDto.cs
namespace TImViecAPI.DTO
{
    public class ThongTinCaNhanCreateDto
    {
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
        public int ungvienID { get; set; } // Bắt buộc
    }
}