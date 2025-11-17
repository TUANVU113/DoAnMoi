using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using TImViecAPI.Model;

//namespace TImViecAPI.Models
//{
//    [Table("UngVien_Cluster")]
//    public class UngVien_Cluster
//    {
//        [Key]
//        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
//        public int ungvienID { get; set; }

//        public int ClusterID { get; set; }

//        public DateOnly NgayCapNhat { get; set; } = DateOnly.FromDateTime(DateTime.Now);

//        // Navigation
//        [ForeignKey("ungvienID")]
//        public UngVien UngVien { get; set; } = null!;
//    }
//}