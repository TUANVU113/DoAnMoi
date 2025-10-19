using System.ComponentModel.DataAnnotations;

namespace TImViecAPI.Model_Function.Dtos
{
    public class CongTyDto
    {
        [Required(ErrorMessage = "Tên công ty là bắt buộc.")]
        [StringLength(255)]
        public string CtName { get; set; }

        [StringLength(255)]
        public string? DiaChi { get; set; }

        [StringLength(255)]
        public string? Logo { get; set; }

        [StringLength(255)]
        public string? MieuTa { get; set; }

        [StringLength(255)]
        public string? MoHinh { get; set; }

        public int? SoNhanVien { get; set; }

        [StringLength(255)]
        public string? QuocGia { get; set; }

        [StringLength(255)]
        public string? NguoiLienHe { get; set; }

        [StringLength(255)]
        public string? SdtLienHe { get; set; }

        [StringLength(255)]
        public string? MaThue { get; set; }

        [StringLength(255)]
        public string? SdtCongTy { get; set; }
    }
}
