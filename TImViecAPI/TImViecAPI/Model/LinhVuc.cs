using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace TImViecAPI.Model
{
    public class LinhVuc
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int lvid { get; set; }

        [StringLength(255)]
        public string? lvName { get; set; }
    }
}
