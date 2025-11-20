// DTOs/ChiTietUngVienDto.cs
using static TImViecAPI.Controllers.BangCapController;

namespace TImViecAPI.Model_Function.Dtos
{
    public class ChiTietUngVienDto
    {
        public ThongTinCaNhanDto ThongTinCaNhan { get; set; } = new();
        public HoSoDto HoSo { get; set; } = new();

        public int DonUngTuyenId { get; set; }
        public string NgayNop { get; set; } = string.Empty;
        public string TrangThai { get; set; } = "Đang chờ duyệt";
        // CŨ: CV tải lên (file)
        public HoSoDto? HoSoFile { get; set; }

        // MỚI: CV tạo nhanh (nội dung chi tiết)
        public HoSoDetailDto? HoSoChiTiet { get; set; }
        public List<BangCapResponseDto> BangCapList { get; set; } = new();
    }

}