//using System.ComponentModel.DataAnnotations;
//using System.ComponentModel.DataAnnotations.Schema;
//using TImViecAPI.Model;

//namespace TImViecAPI.Models
//{
//    [Table("TinTuyenDung_Cluster")]
//    public class TinTuyenDung_Cluster
//    {
//        [Key]
//        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
//        public int ttdid { get; set; }

//        public int ClusterID { get; set; }

//        public DateOnly NgayCapNhat { get; set; } = DateOnly.FromDateTime(DateTime.Now);

//        // Navigation
//        [ForeignKey("ttdid")]
//        public TInTuyenDung TinTuyenDung { get; set; } = null!;
//    }
//}