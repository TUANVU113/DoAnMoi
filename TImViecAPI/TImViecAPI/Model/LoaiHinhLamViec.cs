using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace TImViecAPI.Model
{
    public class LoaiHinhLamViec
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int lhid { get; set; }

        [StringLength(255)]
        public string? lhName { get; set; }
    }
}
