// Models/BangCapUpload.cs
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using TImViecAPI.Model;

namespace TImViecAPI.Models
{
    [Table("BangCap_Upload")]
    public class BangCapUpload
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        public int hosoID { get; set; }

        [Required]
        [StringLength(255)]
        public string TenBangCap { get; set; } = null!;

        public string Loai { get; set; } = "BangCap"; // BangCap hoặc ChungChi

        [Required]
        public string FileUrl { get; set; } = null!;

        public DateTime NgayUpload { get; set; } = DateTime.Now;

        // Navigation
        [ForeignKey("hosoID")]
        public HoSo HoSo { get; set; } = null!;
    }
}