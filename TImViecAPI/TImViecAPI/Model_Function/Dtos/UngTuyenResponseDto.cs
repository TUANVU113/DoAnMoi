namespace TImViecAPI.Model_Function.Dtos
{
    public class UngTuyenResponseDto
    {
        public int UngTuyenId { get; set; }
        public DateTime NgayNop { get; set; } = DateTime.Now; // Mặc định hoặc xử lý null
        public string TrangThai { get; set; } = "Đang chờ duyệt";
        public string TieuDeTin { get; set; }
        public string TenCongTy { get; set; }
    }
}
