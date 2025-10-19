using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace TImViecAPI.Model
{
    public class KinhNgiem
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int knid { get; set; }

        [StringLength(255)]
        public string? ChucVu { get; set; }

        [StringLength(255)]
        public string? TenCongTy { get; set; }

        public DateTime? TGBatDau { get; set; }

        public DateTime? TGKetThuc { get; set; }
    }
}
