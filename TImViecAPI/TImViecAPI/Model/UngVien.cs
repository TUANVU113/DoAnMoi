using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace TImViecAPI.Model
{
    public class UngVien
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        [ForeignKey("NguoiDung")]
        public int uvid { get; set; }

        [StringLength(255)]
        public string? uvName { get; set; }

        public DateTime? NgaySinh { get; set; }

        [StringLength(255)]
        public string? QuocGia { get; set; }

        public int? linhvucID { get; set; }

        [ForeignKey("linhvucID")]
        public LinhVuc? LinhVuc { get; set; }

        public NguoiDung NguoiDung { get; set; }
    }

}
