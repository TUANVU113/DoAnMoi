namespace TImViecAPI.Model_Function.Dtos
{
    public class TinTuyenDungDto
    {
        public int TinId { get; set; }
        public string? TieuDe { get; set; }
        public string? CongTy { get; set; }
        public string? ChucDanh { get; set; }
        public string? DiaDiem { get; set; }
        public string? NgayDang { get; set; }
        public string? HanNop { get; set; }
        public double PhuHop { get; internal set; }
    }
}
