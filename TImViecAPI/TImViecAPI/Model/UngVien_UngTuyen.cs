using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace TImViecAPI.Model
{
    // Bảng UngVien_UngTuyen
    public class UngVien_UngTuyen
    {
        [Key]
        [Column(Order = 1)]
        public int ungvienID { get; set; }

        [Key]
        [Column(Order = 2)]
        public int ungtuyenID { get; set; }

        [ForeignKey("ungvienID")]
        public UngVien UngVien { get; set; }

        [ForeignKey("ungtuyenID")]
        public UngTuyen UngTuyen { get; set; }
    }
}
