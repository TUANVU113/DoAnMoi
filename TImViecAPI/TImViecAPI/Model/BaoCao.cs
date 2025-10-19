using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace TImViecAPI.Model
{
    public class BaoCao
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int bcid { get; set; }

        [StringLength(255)]
        public string? NoiDungBao { get; set; }

        public DateTime? NgayBao { get; set; }

        [StringLength(255)]
        public string? TrangThai { get; set; }
    }
}
