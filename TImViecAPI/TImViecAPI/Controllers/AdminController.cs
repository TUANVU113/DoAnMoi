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


    }
}
