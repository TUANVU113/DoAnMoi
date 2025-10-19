using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;
using TImViecAPI.Data;
using TImViecAPI.Model;

namespace TImViecAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class LoaiHinhLamViecController : ControllerBase
    {
        private readonly AppDbContext _context;

        public LoaiHinhLamViecController(AppDbContext context)
        {
            _context = context;
        }

        public class LoaiHinhLamViecDto
        {
            [Required(ErrorMessage = "Tên loại hình là bắt buộc.")]
            [StringLength(255)]
            public string? lhName { get; set; }
        }

        [HttpPost("add")]
        public async Task<IActionResult> Add([FromBody] LoaiHinhLamViecDto dto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            if (await _context.LoaiHinhLamViec.AnyAsync(lh => lh.lhName == dto.lhName))
            {
                return BadRequest(new { Message = "Tên loại hình đã tồn tại." });
            }

            var loaiHinh = new LoaiHinhLamViec
            {
                lhName = dto.lhName
            };
            _context.LoaiHinhLamViec.Add(loaiHinh);
            await _context.SaveChangesAsync();
            return Ok(new { Message = "Thêm loại hình thành công!", lhid = loaiHinh.lhid });
        }

        [HttpGet("list")]
        public async Task<IActionResult> GetAllLoaiHinhLamViec()
        {
            var loaiHinhs = await _context.LoaiHinhLamViec
                .Select(lh => new { lh.lhid, lh.lhName })
                .ToListAsync();
            if (!loaiHinhs.Any())
            {
                return Ok(new { Message = "Không có loại hình nào trong hệ thống.", Data = new List<object>() });
            }
            return Ok(new
            {
                Message = "Lấy danh sách loại hình thành công!",
                Data = loaiHinhs
            });
        }

        [HttpPut("update/{id}")]
        public async Task<IActionResult> UpdateLoaiHinhLamViec(int id, [FromBody] LoaiHinhLamViecDto dto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var loaiHinh = await _context.LoaiHinhLamViec.FindAsync(id);
            if (loaiHinh == null)
            {
                return NotFound(new { Message = "Loại hình không tồn tại." });
            }

            if (await _context.LoaiHinhLamViec.AnyAsync(lh => lh.lhName == dto.lhName && lh.lhid != id))
            {
                return BadRequest(new { Message = "Tên loại hình đã tồn tại." });
            }

            loaiHinh.lhName = dto.lhName;
            await _context.SaveChangesAsync();
            return Ok(new { Message = "Cập nhật loại hình thành công!", lhid = loaiHinh.lhid });
        }

        [HttpDelete("delete/{id}")]
        public async Task<IActionResult> DeleteLoaiHinhLamViec(int id)
        {
            using var transaction = await _context.Database.BeginTransactionAsync();
            try
            {
                var loaiHinh = await _context.LoaiHinhLamViec.FindAsync(id);
                if (loaiHinh == null)
                {
                    return NotFound(new { Message = "Loại hình không tồn tại." });
                }

                // Kiểm tra ràng buộc FK (nếu TInTuyenDung dùng loaihinhID)
                var usedInTinTuyenDung = await _context.TInTuyenDung.AnyAsync(ttd => ttd.loaihinhID == id);
                if (usedInTinTuyenDung)
                {
                    return BadRequest(new { Message = "Không thể xóa loại hình vì đang được sử dụng trong tin tuyển dụng." });
                }

                _context.LoaiHinhLamViec.Remove(loaiHinh);
                await _context.SaveChangesAsync();
                await transaction.CommitAsync();
                return Ok(new { Message = "Xóa loại hình thành công!", lhid = id });
            }
            catch (Exception ex)
            {
                await transaction.RollbackAsync();
                return StatusCode(500, new { Message = "Lỗi khi xóa: " + ex.Message });
            }
        }
    }
}