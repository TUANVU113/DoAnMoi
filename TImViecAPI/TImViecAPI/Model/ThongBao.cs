using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace TImViecAPI.Model
{
    public class ThongBao
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int tbid { get; set; }

        [StringLength(255)]
        public string? NoiDung { get; set; }

        public DateTime? NgayBao { get; set; }
    }
}
