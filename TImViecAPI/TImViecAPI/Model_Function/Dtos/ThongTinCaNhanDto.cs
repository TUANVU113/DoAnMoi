
using System.ComponentModel.DataAnnotations;
namespace TImViecAPI.Model_Function.Dtos

{
    public class ThongTinCaNhanDto
    {
        public string HoVaTen { get; set; } = "Chưa cung cấp";
        public string GioiTinh { get; set; } = "Chưa cung cấp";
        public string? NgaySinh { get; set; }  // "15/05/1998"
        public string SDT { get; set; } = "Chưa cung cấp";
        public string Email { get; set; } = "Chưa cung cấp";
        public string QuocGia { get; set; } = "Chưa cung cấp";
        public string Tinh { get; set; } = "Chưa cung cấp";
        public string Huyen { get; set; } = "Chưa cung cấp";
        public string DiaChi { get; set; } = "Chưa cung cấp";
        public string CCCD { get; set; } = "Chưa cung cấp";
        public string NoiSinh { get; set; } = "Chưa cung cấp";
    }
}
