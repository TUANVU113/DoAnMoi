using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using TImViecAPI.Data;
using TImViecAPI.Model_Function.Dtos;
using TImViecAPI.Model;
using Microsoft.EntityFrameworkCore;

namespace TImViecAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ThongTinCaNhanController : ControllerBase
    {
        private readonly AppDbContext _context;
        private readonly IWebHostEnvironment _environment;

        public ThongTinCaNhanController(AppDbContext context, IWebHostEnvironment environment)
        {
            _context = context;
            _environment = environment;
        }

        // POST: api/thongtincanhan/create
        [HttpPost("create")]
        [Authorize(Roles = "UngVien")]
        public async Task<ActionResult<ThongTinCaNhan>> CreateThongTinCaNhan(ThongTinCaNhanCreateDto dto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            // 
            string username = User.Identity.Name;
            if (username == null)
            {
                return Unauthorized(new { Message = "Người dùng không hợp lệ hoặc không phải Ưng viên." });
            }
            var ungVien = _context.NguoiDung.FirstOrDefault(ntd => ntd.tkName == username);

            if (ungVien == null)
            {
                return Unauthorized(new { Message = "Ứng viên không tồn tại ." });
            }

            // 3. Tạo mới ThongTinCaNhan
            var thongTin = new ThongTinCaNhan
            {
                HoVaTen = dto.HoVaTen,
                GioiTinh = dto.GioiTinh,
                NgaySinh = dto.NgaySinh,
                SDT = dto.SDT,
                Email = dto.Email,
                QuocGia = dto.QuocGia,
                Tinh = dto.Tinh,
                Huyen = dto.Huyen,
                DiaChi = dto.DiaChi,
                CCCD = dto.CCCD,
                NoiSinh = dto.NoiSinh,
                ungvienID = ungVien.tkid,
            };

            _context.thongTinCaNhans.Add(thongTin);
            await _context.SaveChangesAsync();

            // 4. Trả về thông tin vừa tạo (có thongtinid)
            return Ok(new { Message = "Thêm thông tin cá nhân thành công!", IdCaNhan = thongTin.thongtinid });
        }

        // GET: api/thongtincanhan/5 (tùy chọn)
        //[HttpGet("{id}")]
        //public async Task<ActionResult<ThongTinCaNhan>> GetThongTinCaNhan(int id)
        //{
        //    var info = await _context.thongTinCaNhans
        //        .Include(t => t.UngVien)
        //        .FirstOrDefaultAsync(t => t.thongtinid == id);

        //    if (info == null) return NotFound();

        //    return Ok(info);
        //}

        [HttpGet("thong-bao-cua-toi")]
        [Authorize]
        public async Task<IActionResult> LayThongBaoCuaToi()
        {
            // 1. LẤY USERNAME TỪ JWT
            string? username = User.Identity?.Name;
            if (string.IsNullOrEmpty(username))
                return Unauthorized(new { Message = "Vui lòng đăng nhập." });

            // 2. TÌM NGƯỜI DÙNG
            var nguoiDung = _context.NguoiDung
                .FirstOrDefault(nd => nd.tkName == username);

            if (nguoiDung == null)
                return Unauthorized(new { Message = "Người dùng không tồn tại." });

            int nguoiDungId = nguoiDung.tkid;

            // 3. LẤY DANH SÁCH THÔNG BÁO (SẮP XẾP THEO NGÀY GỐC)
            var danhSach = await _context.NguoiDung_ThongBao
                .Where(ntb => ntb.nguoidungID == nguoiDungId)
                .Join(
                    _context.ThongBao,
                    ntb => ntb.thongbaoID,
                    tb => tb.tbid,
                    (ntb, tb) => new { ntb, tb }
                )
                .OrderByDescending(x => x.tb.NgayBao) // ← SỬA Ở ĐÂY: SẮP XẾP THEO NGÀY GỐC
                .Select(x => new
                {
                    ThongBaoId = x.tb.tbid,
                    NoiDung = x.tb.NoiDung,
                    NgayBao = x.tb.NgayBao, // ← Giữ nguyên DateOnly
                    DaXem = x.ntb.DaXem
                })
                .ToListAsync();

            // 4. FORMAT NGÀY Ở ĐÂY (SAU KHI LẤY DỮ LIỆU)
            var ketQua = danhSach.Select(x => new
            {
                x.ThongBaoId,
                x.NoiDung,
                NgayBao = ((DateTime)x.NgayBao).ToString("dd/MM/yyyy"), // ← FORMAT Ở ĐÂY
                x.DaXem
            }).ToList();

            // 5. TRẢ VỀ
            return Ok(new
            {
                Message = ketQua.Any() ? "Lấy danh sách thông báo thành công!" : "Bạn chưa có thông báo nào.",
                NguoiDungId = nguoiDungId,
                TenDangNhap = nguoiDung.tkName,
                TongSo = ketQua.Count,
                DanhSachThongBao = ketQua
            });
        }
    }
}
