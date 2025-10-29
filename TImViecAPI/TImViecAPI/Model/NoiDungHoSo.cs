using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace TImViecAPI.Model
{
    public class NoiDungHoSo
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int ndid { get; set; }

        [StringLength(255)]
        public string? TenUngVien { get; set; }

        [StringLength(255)]
        public string? GioiTinh { get; set; }

        [StringLength(255)]
        public string? NgaySinh { get; set; }

        [StringLength(255)]
        public string? PhoneHoSo { get; set; }

        [StringLength(255)]
        public string? MailHoSo { get; set; }

        [StringLength(255)]
        public string? QuocGia { get; set; }

        [StringLength(255)]
        public string? Tinh { get; set; }

        [StringLength(255)]
        public string? QuanHuyen { get; set; }

        [StringLength(255)]
        public string? DiaChi { get; set; }

        [StringLength(255)]
        public string? HocVan { get; set; }

       

        public int? hosoID { get; set; }

        [ForeignKey("hosoID")]
        public HoSo? HoSo { get; set; }
    }
}
