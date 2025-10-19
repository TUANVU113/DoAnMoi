using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace TImViecAPI.Model
{
    public class HoSo
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int hsid { get; set; }

        [StringLength(255)]
        public string? hsName { get; set; }

        [StringLength(255)]
        public string? ViTriFile { get; set; }

        public int? ungvienID { get; set; }

        [ForeignKey("ungvienID")]
        public UngVien? UngVien { get; set; }
    }
}
