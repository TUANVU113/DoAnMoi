using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace TImViecAPI.Model
{
    public class UngTuyen
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int utid { get; set; }

        public DateTime? NgayNop { get; set; }

        [StringLength(255)]
        public string? TrangThai { get; set; }

        public int? tintuyendungid { get; set; }

        [ForeignKey("tintuyendungid")]
        public TInTuyenDung? TInTuyenDung { get; set; }
    }
}
