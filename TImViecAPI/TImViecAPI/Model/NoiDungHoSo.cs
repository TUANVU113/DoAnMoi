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
        public string? PhoneHoSo { get; set; }

        [StringLength(255)]
        public string? MailHoSo { get; set; }

        [StringLength(255)]
        public string? HocVan { get; set; }

        public int? NamKinhNghiem { get; set; }

        public int? hosoID { get; set; }

        [ForeignKey("hosoID")]
        public HoSo? HoSo { get; set; }
    }
}
