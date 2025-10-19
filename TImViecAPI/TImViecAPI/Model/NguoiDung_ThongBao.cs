using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace TImViecAPI.Model
{
    public class NguoiDung_ThongBao
    {
        [Key]
        [Column(Order = 1)]
        public int nguoidungID { get; set; }

        [Key]
        [Column(Order = 2)]
        public int thongbaoID { get; set; }

        [Required]
        public bool DaXem { get; set; } = false;

        [ForeignKey("nguoidungID")]
        public NguoiDung NguoiDung { get; set; }

        [ForeignKey("thongbaoID")]
        public ThongBao ThongBao { get; set; }
    }
}
