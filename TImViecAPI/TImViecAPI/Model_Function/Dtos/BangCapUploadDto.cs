namespace TImViecAPI.Model_Function.Dtos
{
    public class BangCapUploadDto
    {
        public int hoSoId { get; set; }
        public string tenBangCap { get; set; } = null!;
        public string loai { get; set; } = null!;
        public IFormFile file { get; set; } = null!;
    }
}
