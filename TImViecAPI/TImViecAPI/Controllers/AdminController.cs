using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using TImViecAPI.Data;
using TImViecAPI.Model_Function.Dtos;

namespace TImViecAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AdminController : ControllerBase
    {
        private readonly AppDbContext _context;

        public AdminController(AppDbContext context)
        {
            _context = context;
        }
        [HttpGet("tin-tuyen-dung")]
        [Authorize(Roles = "Admin")]
        public async Task<ActionResult<ThongKeTuNgayDto>> ThongKeTinTuyenDung()
        {
            var now = DateTime.Today;
            var startOfWeek = now.AddDays(-(int)now.DayOfWeek);
            var startOfMonth = new DateTime(now.Year, now.Month, 1);
            var startOfYear = new DateTime(now.Year, 1, 1);
            var startDate = new DateTime(2025, 9, 20);

            var result = new ThongKeTuNgayDto
            {
                TuNgay = await _context.TInTuyenDung.CountAsync(t => t.NgayDang >= startDate),
                Ngay = await _context.TInTuyenDung.CountAsync(t => t.NgayDang >= now),
                Tuan = await _context.TInTuyenDung.CountAsync(t => t.NgayDang >= startOfWeek),
                Thang = await _context.TInTuyenDung.CountAsync(t => t.NgayDang >= startOfMonth),
                Nam = await _context.TInTuyenDung.CountAsync(t => t.NgayDang >= startOfYear)
            };

            return Ok(result);
        }
        [HttpGet("tin-tuyen-dung-theo-ngay")]
        [Authorize(Roles = "Admin")]
        public async Task<ActionResult> ThongKeTheoNgay([FromQuery] DateTime fromDate, [FromQuery] DateTime toDate)
        {
            // Kiểm tra từ ngày <= đến ngày
            if (fromDate > toDate)
                return BadRequest("Ngày bắt đầu không được lớn hơn ngày kết thúc.");

            var data = await _context.TInTuyenDung
                .Where(t => t.NgayDang.HasValue &&
                            t.NgayDang.Value.Date >= fromDate.Date &&
                            t.NgayDang.Value.Date <= toDate.Date)
                .GroupBy(t => t.NgayDang.Value.Date) // Lấy ngày bỏ phần thời gian
                .Select(g => new
                {
                    Ngay = g.Key,
                    SoLuong = g.Count()
                })
                .OrderBy(x => x.Ngay)
                .ToListAsync();

            return Ok(data);
        }


        [HttpGet("ung-tuyen")]
        [Authorize(Roles = "Admin")]
        public async Task<ActionResult<ThongKeUngTuyenFullDto>> ThongKeUngTuyen()
        {
            var now = DateTime.Today;
            var startOfWeek = now.AddDays(-(int)now.DayOfWeek);
            var startOfMonth = new DateTime(now.Year, now.Month, 1);
            var startOfYear = new DateTime(now.Year, 1, 1);
            var startDate = new DateTime(2025, 9, 20);

            // ========== 1. Thống kê số lượng ==========
            int tuNgay = await _context.UngTuyen.CountAsync(t => t.NgayNop >= startDate);
            int ngay = await _context.UngTuyen.CountAsync(t => t.NgayNop >= now);
            int tuan = await _context.UngTuyen.CountAsync(t => t.NgayNop >= startOfWeek);
            int thang = await _context.UngTuyen.CountAsync(t => t.NgayNop >= startOfMonth);
            int nam = await _context.UngTuyen.CountAsync(t => t.NgayNop >= startOfYear);


            // ========== 2. Tin được ứng tuyển nhiều nhất theo mốc ==========
            async Task<object> TopTin(DateTime from)
            {
                return await _context.UngTuyen
                    .Where(t => t.NgayNop >= from)
                    .GroupBy(t => t.tintuyendungid)
                    .Select(g => new
                    {
                        TinId = g.Key,
                        SoLuong = g.Count()
                    })
                    .OrderByDescending(x => x.SoLuong)
                    .FirstOrDefaultAsync();
            }

            var tinNhieuNhat = new
            {
                TuNgay = await TopTin(startDate),
                Ngay = await TopTin(now),
                Tuan = await TopTin(startOfWeek),
                Thang = await TopTin(startOfMonth),
                Nam = await TopTin(startOfYear)
            };


            // ========== 3. Thống kê trạng thái ==========
            async Task<object> ThongKeTrangThai(DateTime from)
            {
                return await _context.UngTuyen
                    .Where(t => t.NgayNop >= from)
                    .GroupBy(t => t.TrangThai)
                    .Select(g => new
                    {
                        TrangThai = g.Key,
                        SoLuong = g.Count()
                    })
                    .ToListAsync();
            }

            var trangThai = new
            {
                TuNgay = await ThongKeTrangThai(startDate),
                Ngay = await ThongKeTrangThai(now),
                Tuan = await ThongKeTrangThai(startOfWeek),
                Thang = await ThongKeTrangThai(startOfMonth),
                Nam = await ThongKeTrangThai(startOfYear)
            };


            // ========== 4. Trả về ==========
            var result = new ThongKeUngTuyenFullDto
            {
                TuNgay = tuNgay,
                Ngay = ngay,
                Tuan = tuan,
                Thang = thang,
                Nam = nam,
                TinNhieuNhat = tinNhieuNhat,
                TrangThai = trangThai
            };

            return Ok(result);
        }


    }
}
