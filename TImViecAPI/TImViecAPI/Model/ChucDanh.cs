using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace TImViecAPI.Model
{
    public class ChucDanh
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int cdid { get; set; }

        [StringLength(255)]
        public string? cdName { get; set; }
    }
}
