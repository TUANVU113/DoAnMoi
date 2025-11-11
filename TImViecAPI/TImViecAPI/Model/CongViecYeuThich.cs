using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using TImViecAPI.Model;

namespace TImViecAPI.Models
{
    [Table("CongViecYeuThich")]
    public class CongViecYeuThich
    {
        [Key, Column(Order = 0)]
        public int ungvienID { get; set; }

        [Key, Column(Order = 1)]
        public int tintuyenID { get; set; }

        public DateOnly? NgayThem { get; set; } = DateOnly.FromDateTime(DateTime.Now);

        // Navigation Properties
        [ForeignKey("ungvienID")]
        public UngVien UngVien { get; set; } = null!;

        [ForeignKey("tintuyenID")]
        public TInTuyenDung TinTuyenDung { get; set; } = null!;
    }
}