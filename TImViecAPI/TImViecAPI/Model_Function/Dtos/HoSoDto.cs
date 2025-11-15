namespace TImViecAPI.Model_Function.Dtos
{
    public class HoSoDto
    {
        public int HoSoId { get; set; }
        public string HoSoName { get; set; } = "Chưa đặt tên";
        public string? FileUrl { get; set; }
        public string TenUngVien { get; set; } = null!; // MỚI
        public string? Avata { get; set; } // MỚI: /Upload/xxx.jpg
        public DateTime NgayTao { get; set; } // MỚI
    }
}
