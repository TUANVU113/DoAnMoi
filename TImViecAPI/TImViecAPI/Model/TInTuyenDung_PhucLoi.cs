using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace TImViecAPI.Model
{
    public class TInTuyenDung_PhucLoi
    {
        [Key]
        [Column(Order = 1)]
        public int tintuyendungID { get; set; }

        [Key]
        [Column(Order = 2)]
        public int phucloiID { get; set; }

        [ForeignKey("tintuyendungID")]
        public TInTuyenDung TInTuyenDung { get; set; }

        [ForeignKey("phucloiID")]
        public PhucLoi PhucLoi { get; set; }
    }
}
