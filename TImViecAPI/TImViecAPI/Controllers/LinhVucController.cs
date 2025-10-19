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
    public class LinhVucController : ControllerBase
    {
        private readonly AppDbContext _context;

        public LinhVucController(AppDbContext context)
        {
            _context = context;
        }

        // DTO cho insert
        public class LinhVucDto
        {
            [Required(ErrorMessage = "Tên lĩnh vực là bắt buộc.")]
            [StringLength(255)]
            public string LvName { get; set; }
        }

        [HttpPost("add")]
        public async Task<IActionResult> Add([FromBody] LinhVucDto dto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            // Kiểm tra trùng tên lĩnh vực
            if (await _context.LinhVuc.AnyAsync(lv => lv.lvName == dto.LvName))
            {
                return BadRequest(new { Message = "Tên lĩnh vực đã tồn tại." });
            }

            var linhVuc = new LinhVuc
            {
                lvName = dto.LvName
            };

            _context.LinhVuc.Add(linhVuc);
            await _context.SaveChangesAsync();

            return Ok(new { Message = "Thêm lĩnh vực thành công!", Lvid = linhVuc.lvid });
        }
        [HttpGet("list")]
        public async Task<IActionResult> GetAllLinhVuc()
        {
            var linhVucs = await _context.LinhVuc
                .Select(lv => new { lv.lvid, lv.lvName })
                .ToListAsync();

            if (!linhVucs.Any())
            {
                return Ok(new { Message = "Không có lĩnh vực nào trong hệ thống.", Data = new List<object>() });
            }

            return Ok(new
            {
                Message = "Lấy danh sách lĩnh vực thành công!",
                Data = linhVucs
            });
        }
        [HttpPut("update/{id}")]
        public async Task<IActionResult> UpdateLinhVuc(int id, [FromBody] LinhVucDto dto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var linhVuc = await _context.LinhVuc.FindAsync(id);
            if (linhVuc == null)
            {
                return NotFound(new { Message = "Lĩnh vực không tồn tại." });
            }

            // Kiểm tra trùng tên (trừ bản thân)
            if (await _context.LinhVuc.AnyAsync(lv => lv.lvName == dto.LvName && lv.lvid != id))
            {
                return BadRequest(new { Message = "Tên lĩnh vực đã tồn tại." });
            }

            linhVuc.lvName = dto.LvName;
            await _context.SaveChangesAsync();

            return Ok(new { Message = "Cập nhật lĩnh vực thành công!", Lvid = linhVuc.lvid });
        }

        // DELETE: api/linhvuc/delete/{id} - Xóa theo lvid
        [HttpDelete("delete/{id}")]
        public async Task<IActionResult> DeleteLinhVuc(int id)
        {
            using var transaction = await _context.Database.BeginTransactionAsync();
            try
            {
                var linhVuc = await _context.LinhVuc.FindAsync(id);
                if (linhVuc == null)
                {
                    return NotFound(new { Message = "Lĩnh vực không tồn tại." });
                }

                // Kiểm tra ràng buộc FK (nếu có UngVien dùng linhvucID này)
                var usedInUngVien = await _context.UngVien.AnyAsync(uv => uv.linhvucID == id);
                if (usedInUngVien)
                {
                    return BadRequest(new { Message = "Không thể xóa lĩnh vực vì đang được sử dụng trong hồ sơ ứng viên." });
                }

                // Xóa (các ràng buộc khác nếu có)
                _context.LinhVuc.Remove(linhVuc);
                await _context.SaveChangesAsync();
                await transaction.CommitAsync();

                return Ok(new { Message = "Xóa lĩnh vực thành công!", Lvid = id });
            }
            catch (Exception ex)
            {
                await transaction.RollbackAsync();
                return StatusCode(500, new { Message = "Lỗi khi xóa: " + ex.Message });
            }
        }
    }
}
