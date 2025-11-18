namespace TImViecAPI.Model_Function.Dtos
{
    public class TinTuyenDungSearchDto
    {
        public string? Keyword { get; set; }
        public int? LoaiHinhId { get; set; }
        public int? ChucDanhId { get; set; }
        public int? KinhNghiemId { get; set; }
        public int? BangCapId { get; set; }
        public int? LinhVucId { get; set; }
        public int? ViTriId { get; set; }
        public int? NhaTuyenDungId { get; set; }
        public int? CongTyId { get; set; }

        public DateTime? FromDate { get; set; }
        public DateTime? ToDate { get; set; }


        // Phân trang
        public int Page { get; set; } = 1;
        public int PageSize { get; set; } = 10;

        // Sắp xếp
        public string? SortBy { get; set; } = "NgayDang"; // NgayDang, Luong, TieuDe, TenCongTy
        public string? SortOrder { get; set; } = "desc";  // asc / desc
    }
}