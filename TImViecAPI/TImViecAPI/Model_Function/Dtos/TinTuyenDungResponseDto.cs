namespace TImViecAPI.Model.Dto
{
    public class TinTuyenDungResponseDto
    {
        public int Id { get; set; }
        public string TieuDe { get; set; } = string.Empty;
        public string MieuTa { get; set; } = string.Empty;
        public int? LuongMin { get; set; }
        public int? LuongMax { get; set; }
        public string TrangThai { get; set; } = string.Empty;
        public DateTime? NgayDang { get; set; }
        public DateTime? HanNop { get; set; }

        // Thông tin liên quan
        public string? TenCongTy { get; set; }
        public string? LoaiHinh { get; set; }
        public string? ChucDanh { get; set; }
        public string? KinhNghiem { get; set; }
        public string? BangCap { get; set; }
        public string? LinhVuc { get; set; }
        public string? ViTri { get; set; }

        public int SoLuongUngTuyen { get; set; }
    }
}