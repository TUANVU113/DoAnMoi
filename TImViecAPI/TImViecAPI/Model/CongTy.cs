using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace TImViecAPI.Model
{
    public class CongTy
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int ctid { get; set; }

        [StringLength(255)]
        public string? ctName { get; set; }

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
        public string? sdtLienHe { get; set; }

        [StringLength(255)]
        public string? MaThue { get; set; }

        [StringLength(255)]
        public string? sdtCongTy { get; set; }
    }
}
