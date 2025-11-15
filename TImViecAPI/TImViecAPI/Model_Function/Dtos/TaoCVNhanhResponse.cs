namespace TImViecAPI.Model_Function.Dtos
{
    public class TaoCVNhanhResponse
    {
        public int HoSoID { get; set; }
        public int NoiDungID { get; set; }
        public string TenHoSo { get; set; } = null!;
        public string Message { get; set; } = null!;
        public DateTime NgayTao { get; set; }
    }
}
