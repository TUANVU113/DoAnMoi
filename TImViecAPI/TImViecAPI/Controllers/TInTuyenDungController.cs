using System.ComponentModel.DataAnnotations;
using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using TImViecAPI.Data;
using TImViecAPI.Model;

namespace TImViecAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class TInTuyenDungController : ControllerBase
    {
        private readonly AppDbContext _context;

        public TInTuyenDungController(AppDbContext context)
        {
            _context = context;
        }

        public class TInTuyenDungDto
        {
            [Required(ErrorMessage = "Tiêu đề là bắt buộc.")]
            [StringLength(255)]
            public string? TieuDe { get; set; }

            [StringLength(255)]
            public string? MieuTa { get; set; }

            public bool? DaDuyet { get; set; }

            [StringLength(255)]
            public string? TrangThai { get; set; }

            public int? YeuCau { get; set; }

            public int? Tuoi { get; set; }

            public DateTime? HanNop { get; set; }

            [Required(ErrorMessage = "Loại hình ID là bắt buộc.")]
            public int? loaihinhID { get; set; }

            [Required(ErrorMessage = "Chức danh ID là bắt buộc.")]
            public int? chucdanhID { get; set; }

            [Required(ErrorMessage = "Kinh nghiệm ID là bắt buộc.")]
            public int? kinhnghiemID { get; set; }

            [Required(ErrorMessage = "Bằng cấp ID là bắt buộc.")]
            public int? bangcapID { get; set; }

            [Required(ErrorMessage = "Lĩnh vực ID là bắt buộc.")]
            public int? linhvucIID { get; set; }

            [Required(ErrorMessage = "Vị trí ID là bắt buộc.")]
            public int? vitriID { get; set; }
        }

        [HttpPost("add")]
        [Authorize(Roles = "NhaTuyenDung")] // Chỉ NTD mới tạo tin
        public async Task<IActionResult> Add([FromBody] TInTuyenDungDto dto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            // Lấy nhaTuyenDungID từ cookie
            //var ntdId = User.FindFirst("NhaTuyenDungID")?.Value;
            string Users = User.Identity.Name;
            if (Users == null)
            {
                return Unauthorized(new { Message = "Người dùng không hợp lệ hoặc không phải NTD." });
            }
            var id_nhaTuyenDung = _context.NguoiDung.FirstOrDefault(ntd => ntd.tkName == Users);


            // Kiểm tra các FK tồn tại
            if (!await _context.LoaiHinhLamViec.AnyAsync(lh => lh.lhid == dto.loaihinhID) ||
                !await _context.ChucDanh.AnyAsync(cd => cd.cdid == dto.chucdanhID) ||
                !await _context.KinhNghiem.AnyAsync(kn => kn.knid == dto.kinhnghiemID) ||
                !await _context.BangCap.AnyAsync(bc => bc.bcid == dto.bangcapID) ||
                !await _context.LinhVuc.AnyAsync(lv => lv.lvid == dto.linhvucIID) ||
                !await _context.ViTri.AnyAsync(vt => vt.vtid == dto.vitriID))
            {
                return BadRequest(new { Message = "Một hoặc nhiều ID tham chiếu không tồn tại." });
            }

            var tinTuyenDung = new TInTuyenDung
            {
                TieuDe = dto.TieuDe,
                MieuTa = dto.MieuTa,
                DaDuyet = dto.DaDuyet ?? false, // Default false nếu null
                TrangThai = dto.TrangThai,
                YeuCau = dto.YeuCau,
                Tuoi = dto.Tuoi,
                NgayDang = DateTime.Now,
                HanNop = dto.HanNop,
                loaihinhID = dto.loaihinhID,
                chucdanhID = dto.chucdanhID,
                kinhnghiemID = dto.kinhnghiemID,
                bangcapID = dto.bangcapID,
                linhvucIID = dto.linhvucIID,
                vitriID = dto.vitriID,
                nhaTuyenDungID = id_nhaTuyenDung.tkid
            };

            _context.TInTuyenDung.Add(tinTuyenDung);
            await _context.SaveChangesAsync();
            return Ok(new { Message = "Thêm tin tuyển dụng thành công!", ttdid = tinTuyenDung.ttdid });
        }

        [HttpGet("list")]
        
        public async Task<IActionResult> GetAllTInTuyenDung()
        {
            var tins = await _context.TInTuyenDung
                .Select(ttd => new
                {
                    ttd.ttdid,
                    ttd.TieuDe,
                    ttd.MieuTa,
                    ttd.DaDuyet,
                    ttd.TrangThai,
                    ttd.YeuCau,
                    ttd.Tuoi,
                    ttd.NgayDang,
                    ttd.HanNop,
                    ttd.loaihinhID,
                    ttd.chucdanhID,
                    ttd.kinhnghiemID,
                    ttd.bangcapID,
                    ttd.linhvucIID,
                    ttd.vitriID,
                    ttd.nhaTuyenDungID
                })
                .ToListAsync();
            if (!tins.Any())
            {
                return Ok(new { Message = "Không có tin tuyển dụng nào trong hệ thống.", Data = new List<object>() });
            }
            return Ok(new
            {
                Message = "Lấy danh sách tin tuyển dụng thành công!",
                Data = tins
            });
        }




        [HttpGet("list-by-ntd")]
        [Authorize(Roles = "NhaTuyenDung")]
        public async Task<IActionResult> GetTinByNhaTuyenDung()
        {
            // Lấy thông tin người dùng từ JWT
            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

            if (string.IsNullOrEmpty(userIdClaim))
                return Unauthorized(new { Message = "Không tìm thấy thông tin người dùng trong token." });

            int tkid = int.Parse(userIdClaim);

            // Tìm NhaTuyenDung tương ứng
            var ntd = await _context.NhaTuyenDung.FirstOrDefaultAsync(x => x.ntdid == tkid);
            if (ntd == null)
                return Unauthorized(new { Message = "Không tìm thấy tài khoản nhà tuyển dụng." });

            // Lọc tin theo ID nhà tuyển dụng
            var tins = await _context.TInTuyenDung
                .Where(ttd => ttd.nhaTuyenDungID == ntd.ntdid)
                .Select(ttd => new
                {
                    ttd.ttdid,
                    ttd.TieuDe,
                    ttd.MieuTa,
                    ttd.DaDuyet,
                    ttd.TrangThai,
                    ttd.YeuCau,
                    ttd.Tuoi,
                    ttd.NgayDang,
                    ttd.HanNop
                })
                .ToListAsync();

            return Ok(new
            {
                Message = "Lấy danh sách tin của NTD thành công!",
                Data = tins
            });
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetTinTuyenDungById(int id)
        {
            var tin = await _context.TInTuyenDung
                .Include(t => t.LoaiHinhLamViec)
                .Include(t => t.ChucDanh)
                .Include(t => t.KinhNghiem)
                .Include(t => t.BangCap)
                .Include(t => t.LinhVuc)
                .Include(t => t.ViTri)
                .FirstOrDefaultAsync(t => t.ttdid == id);

            if (tin == null)
                return NotFound(new { Message = "Không tìm thấy tin tuyển dụng." });

            return Ok(new
            {
                Message = "Lấy thông tin tin tuyển dụng thành công!",
                Data = new
                {
                    tin.ttdid,
                    tin.TieuDe,
                    tin.MieuTa,
                    tin.DaDuyet,
                    tin.TrangThai,
                    tin.YeuCau,
                    tin.Tuoi,
                    tin.NgayDang,
                    tin.HanNop,
                    tin.loaihinhID,
                    tin.chucdanhID,
                    tin.kinhnghiemID,
                    tin.bangcapID,
                    tin.linhvucIID,
                    tin.vitriID,
                    tin.nhaTuyenDungID
                }
            });
        }

        [HttpPut("update/{id}")]
        [Authorize(Roles = "NhaTuyenDung")]
        public async Task<IActionResult> UpdateTInTuyenDung(int id, [FromBody] TInTuyenDungDto dto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var tin = await _context.TInTuyenDung.FindAsync(id);
            if (tin == null)
            {
                return NotFound(new { Message = "Tin tuyển dụng không tồn tại." });
            }

            // Kiểm tra nhaTuyenDungID từ cookie khớp với tin
            string Users = User.Identity.Name;
            if (User == null)
            {
                return Unauthorized(new { Message = "Bạn không có quyền chỉnh sửa tin này." });
            }
            var nhatuyndung = _context.NhaTuyenDung.FirstOrDefault(ntd => ntd.ntdName == Users); 

            // Kiểm tra các FK tồn tại
            if (!await _context.LoaiHinhLamViec.AnyAsync(lh => lh.lhid == dto.loaihinhID) ||
                !await _context.ChucDanh.AnyAsync(cd => cd.cdid == dto.chucdanhID) ||
                !await _context.KinhNghiem.AnyAsync(kn => kn.knid == dto.kinhnghiemID) ||
                !await _context.BangCap.AnyAsync(bc => bc.bcid == dto.bangcapID) ||
                !await _context.LinhVuc.AnyAsync(lv => lv.lvid == dto.linhvucIID) ||
                !await _context.ViTri.AnyAsync(vt => vt.vtid == dto.vitriID))
            {
                return BadRequest(new { Message = "Một hoặc nhiều ID tham chiếu không tồn tại." });
            }

            tin.TieuDe = dto.TieuDe;
            tin.MieuTa = dto.MieuTa;
            tin.DaDuyet = dto.DaDuyet ?? tin.DaDuyet; // Giữ nguyên nếu null
            tin.TrangThai = dto.TrangThai;
            tin.YeuCau = dto.YeuCau;
            tin.Tuoi = dto.Tuoi;
            tin.HanNop = dto.HanNop;
            tin.loaihinhID = dto.loaihinhID;
            tin.chucdanhID = dto.chucdanhID;
            tin.kinhnghiemID = dto.kinhnghiemID;
            tin.bangcapID = dto.bangcapID;
            tin.linhvucIID = dto.linhvucIID;
            tin.vitriID = dto.vitriID;

            await _context.SaveChangesAsync();
            return Ok(new { Message = "Cập nhật tin tuyển dụng thành công!", ttdid = tin.ttdid });
        }

        [HttpDelete("delete/{id}")]
        [Authorize(Roles = "NhaTuyenDung")]
        public async Task<IActionResult> DeleteTInTuyenDung(int id)
        {
            using var transaction = await _context.Database.BeginTransactionAsync();
            try
            {
                var tin = await _context.TInTuyenDung.FindAsync(id);
                if (tin == null)
                {
                    return NotFound(new { Message = "Tin tuyển dụng không tồn tại." });
                }

                

                _context.TInTuyenDung.Remove(tin);
                await _context.SaveChangesAsync();
                await transaction.CommitAsync();
                return Ok(new { Message = "Xóa tin tuyển dụng thành công!", ttdid = id });
            }
            catch (Exception ex)
            {
                await transaction.RollbackAsync();
                return StatusCode(500, new { Message = "Lỗi khi xóa: " + ex.Message });
            }
        }
    }
}