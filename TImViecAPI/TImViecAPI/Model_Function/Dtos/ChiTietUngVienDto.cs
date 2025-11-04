// DTOs/ChiTietUngVienDto.cs
namespace TImViecAPI.Model_Function.Dtos
{
    public class ChiTietUngVienDto
    {
        public ThongTinCaNhanDto ThongTinCaNhan { get; set; } = new();
        public HoSoDto HoSo { get; set; } = new();

        public int DonUngTuyenId { get; set; }
        public string NgayNop { get; set; } = string.Empty;
        public string TrangThai { get; set; } = "Đang chờ duyệt";
    }

    // Dùng lại 2 DTO cũ
}