using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace TImViecAPI.Model
{
    [Table("ToCaoTin")]
    public class ToCaoTin
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        public int ungvienID { get; set; }           // Ai tố cáo
        public int ttdid { get; set; }               // Tố cáo tin nào ← QUAN TRỌNG NHẤT

        [Required]
        public string LyDo { get; set; } = null!;    // "Lừa đảo", "Đa cấp", "Spam", "Nội dung sai"...

        [Required]
        public string NoiDung { get; set; } = null!; // Nội dung tố cáo chi tiết

        public DateTime NgayToCao { get; set; } = DateTime.Now;

        public string TrangThai { get; set; } = "Chờ xử lý"; // Chờ xử lý, Đã duyệt, Từ chối

        // Navigation
        [ForeignKey("ungvienID")]
        public UngVien UngVien { get; set; } = null!;

        [ForeignKey("ttdid")]
        public TInTuyenDung TinTuyenDung { get; set; } = null!;
    }
}
