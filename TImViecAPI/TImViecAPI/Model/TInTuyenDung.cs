using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace TImViecAPI.Model
{
    public class TInTuyenDung
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int ttdid { get; set; }

        [StringLength(255)]
        public string? TieuDe { get; set; }

        [StringLength(255)]
        public string? MieuTa { get; set; }

        public bool? DaDuyet { get; set; }

        [StringLength(255)]
        public string? TrangThai { get; set; }

        public int? YeuCau { get; set; }

        public int? Tuoi { get; set; }

        public DateTime? NgayDang { get; set; }

        public DateTime? HanNop { get; set; }

        public int? loaihinhID { get; set; }

        public int? chucdanhID { get; set; }

        public int? kinhnghiemID { get; set; }

        public int? bangcapID { get; set; }

        public int? linhvucIID { get; set; }

        public int? vitriID { get; set; }

        public int? nhaTuyenDungID { get; set; }

        [ForeignKey("loaihinhID")]
        public LoaiHinhLamViec? LoaiHinhLamViec { get; set; }

        [ForeignKey("chucdanhID")]
        public ChucDanh? ChucDanh { get; set; }

        [ForeignKey("kinhnghiemID")]
        public KinhNghiem? KinhNghiem { get; set; }

        [ForeignKey("bangcapID")]
        public BangCap? BangCap { get; set; }

        [ForeignKey("linhvucIID")]
        public LinhVuc? LinhVuc { get; set; }

        [ForeignKey("vitriID")]
        public ViTri? ViTri { get; set; }

        [ForeignKey("nhaTuyenDungID")]
        public NhaTuyenDung? NhaTuyenDung { get; set; }

    }
}
