namespace TImViecAPI.Model_Function.Dtos
{
    public class HoSoDetailDto
    {
        public int HoSoId { get; set; }
        public string HoSoName { get; set; } = null!;
        public string TenUngVien { get; set; } = null!;
        public string PhoneHoSo { get; set; } = null!;
        public string MailHoSo { get; set; } = null!;
        public string? HocVan { get; set; }
        public int? NamKinhNghiemID { get; set; }
        public int? MucLuong { get; set; }
        public int? ChucDanhID { get; set; }
        public int? LoaiHinhLamViecID { get; set; }
        public int? LinhVucID { get; set; }
        public int? ViTriLamViecID { get; set; }
        public string? MucTieu { get; set; }

        public string? KyNang { get; set; }
        public string? ChucChi { get; set; }
        public string? Avata { get; set; }
        public string? FileUrl { get; set; }
        public DateTime NgayTao { get; set; }
    }
}
