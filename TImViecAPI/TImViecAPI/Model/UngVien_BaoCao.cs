using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace TImViecAPI.Model
{
    public class UngVien_BaoCao
    {
        [Key]
        [Column(Order = 1)]
        public int ungvienID { get; set; }

        [Key]
        [Column(Order = 2)]
        public int baocaoID { get; set; }

        [ForeignKey("ungvienID")]
        public UngVien UngVien { get; set; }

        [ForeignKey("baocaoID")]
        public BaoCao BaoCao { get; set; }
    }
}
