// DTOs/UngTuyenCoBanDto.cs
namespace TImViecAPI.Model_Function.Dtos
{
    public class UngTuyenCoBanDto
    {
        public int DonUngTuyenId { get; set; }
        public int UngVienId { get; set; }
        public string NgayNop { get; set; } = string.Empty;
        public string TrangThai { get; set; } = "Đang chờ duyệt";
    }
}