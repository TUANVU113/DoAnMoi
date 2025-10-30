using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using TImViecAPI.Data;
using TImViecAPI.DTO;
using TImViecAPI.Model;

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
    }
}
