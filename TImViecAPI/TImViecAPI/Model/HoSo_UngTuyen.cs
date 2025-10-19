using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace TImViecAPI.Model
{
    public class HoSo_UngTuyen
    {
        [Key]
        [Column(Order = 1)]
        public int hosoID { get; set; }

        [Key]
        [Column(Order = 2)]
        public int ungtuyenID { get; set; }

        [ForeignKey("hosoID")]
        public HoSo HoSo { get; set; }

        [ForeignKey("ungtuyenID")]
        public UngTuyen UngTuyen { get; set; }
    }
}
