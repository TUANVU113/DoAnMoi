using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace TImViecAPI.Model
{
    public class NhaTuyenDung
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        [ForeignKey("NguoiDung")]
        public int ntdid { get; set; }

        [StringLength(255)]
        public string? ntdName { get; set; }

        public int? ctID { get; set; }

        [ForeignKey("ctID")]
        public CongTy? CongTy { get; set; }

        public NguoiDung NguoiDung { get; set; }
    }
}
