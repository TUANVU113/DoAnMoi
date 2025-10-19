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
    public class BangCapController : ControllerBase
    {
        private readonly AppDbContext _context;

        public BangCapController(AppDbContext context)
        {
            _context = context;
        }

        public class BangCapDto
        {
            [Required(ErrorMessage = "Tên bằng cấp là bắt buộc.")]
            [StringLength(255)]
            public string? bcName { get; set; }
        }

        [HttpPost("add")]
        public async Task<IActionResult> Add([FromBody] BangCapDto dto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            if (await _context.BangCap.AnyAsync(bc => bc.bcName == dto.bcName))
            {
                return BadRequest(new { Message = "Tên bằng cấp đã tồn tại." });
            }

            var bangCap = new BangCap
            {
                bcName = dto.bcName
            };
            _context.BangCap.Add(bangCap);
            await _context.SaveChangesAsync();
            return Ok(new { Message = "Thêm bằng cấp thành công!", bcid = bangCap.bcid });
        }

        [HttpGet("list")]
        public async Task<IActionResult> GetAllBangCap()
        {
            var bangCaps = await _context.BangCap
                .Select(bc => new { bc.bcid, bc.bcName })
                .ToListAsync();
            if (!bangCaps.Any())
            {
                return Ok(new { Message = "Không có bằng cấp nào trong hệ thống.", Data = new List<object>() });
            }
            return Ok(new
            {
                Message = "Lấy danh sách bằng cấp thành công!",
                Data = bangCaps
            });
        }

        [HttpPut("update/{id}")]
        public async Task<IActionResult> UpdateBangCap(int id, [FromBody] BangCapDto dto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var bangCap = await _context.BangCap.FindAsync(id);
            if (bangCap == null)
            {
                return NotFound(new { Message = "Bằng cấp không tồn tại." });
            }

            if (await _context.BangCap.AnyAsync(bc => bc.bcName == dto.bcName && bc.bcid != id))
            {
                return BadRequest(new { Message = "Tên bằng cấp đã tồn tại." });
            }

            bangCap.bcName = dto.bcName;
            await _context.SaveChangesAsync();
            return Ok(new { Message = "Cập nhật bằng cấp thành công!", bcid = bangCap.bcid });
        }

        [HttpDelete("delete/{id}")]
        public async Task<IActionResult> DeleteBangCap(int id)
        {
            using var transaction = await _context.Database.BeginTransactionAsync();
            try
            {
                var bangCap = await _context.BangCap.FindAsync(id);
                if (bangCap == null)
                {
                    return NotFound(new { Message = "Bằng cấp không tồn tại." });
                }

                var usedInTinTuyenDung = await _context.TInTuyenDung.AnyAsync(ttd => ttd.bangcapID == id);
                if (usedInTinTuyenDung)
                {
                    return BadRequest(new { Message = "Không thể xóa bằng cấp vì đang được sử dụng trong tin tuyển dụng." });
                }

                _context.BangCap.Remove(bangCap);
                await _context.SaveChangesAsync();
                await transaction.CommitAsync();
                return Ok(new { Message = "Xóa bằng cấp thành công!", bcid = id });
            }
            catch (Exception ex)
            {
                await transaction.RollbackAsync();
                return StatusCode(500, new { Message = "Lỗi khi xóa: " + ex.Message });
            }
        }
    }
}