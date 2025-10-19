using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace TImViecAPI.Model
{
    public class NoiDung_KinhNghiem
    {
        [Key]
        [Column(Order = 1)]
        public int noidungID { get; set; }

        [Key]
        [Column(Order = 2)]
        public int kinhnghiemID { get; set; }

        [ForeignKey("noidungID")]
        public NoiDungHoSo NoiDungHoSo { get; set; }

        [ForeignKey("kinhnghiemID")]
        public KinhNgiem KinhNgiem { get; set; }
    }
}
