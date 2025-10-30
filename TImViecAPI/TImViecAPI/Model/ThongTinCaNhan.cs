using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace TImViecAPI.Model
{
    public class ThongTinCaNhan
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int thongtinid { get; set; }

        [StringLength(255)]
        public string? HoVaTen { get; set; }

        [StringLength(255)]
        public string? GioiTinh { get; set; }

        
        public DateTime? NgaySinh { get; set; }

        [StringLength(255)]
        public string? SDT { get; set; }

        [StringLength(255)]
        public string? Email { get; set; }

        [StringLength(255)]
        public string? QuocGia { get; set; }

        [StringLength(255)]
        public string? Tinh { get; set; }

        [StringLength(255)]
        public string? Huyen { get; set; }

        [StringLength(255)]
        public string? DiaChi { get; set; }

        [StringLength(255)]
        public string? CCCD { get; set; }
        [StringLength(255)]
        public string? NoiSinh { get; set; }

        public int? ungvienID { get; set; }

        [ForeignKey("ungvienID")]
        public UngVien? UngVien { get; set; }
    }
}
