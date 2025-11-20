namespace TImViecAPI.Model_Function.Dtos
{
    public class BangCapResponseDto
    {
        public int Id { get; set; }
        public string TenBangCap { get; set; } = null!;
        public string Loai { get; set; } = null!;
        public string FileUrl { get; set; } = null!;
        public string NgayUpload { get; set; } = null!;
    }
}
