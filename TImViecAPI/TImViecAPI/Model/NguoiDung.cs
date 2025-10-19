using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace TImViecAPI.Model
{
    public class NguoiDung
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int tkid { get; set; }

        [Required]
        [StringLength(255)]
        public string tkName { get; set; }

        [Required]
        [StringLength(255)]
        public string sdt { get; set; }

        [Required]
        [StringLength(255)]
        public string mail { get; set; }

        [Required]
        [StringLength(255)]
        public string password { get; set; }
    }
}
