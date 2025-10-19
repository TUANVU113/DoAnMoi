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
    public class ViTriController : ControllerBase
    {
        private readonly AppDbContext _context;

        public ViTriController(AppDbContext context)
        {
            _context = context;
        }

        public class ViTriDto
        {
            [Required(ErrorMessage = "Tên vị trí là bắt buộc.")]
            [StringLength(255)]
            public string? vtName { get; set; }
        }

        [HttpPost("add")]
        public async Task<IActionResult> Add([FromBody] ViTriDto dto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            if (await _context.ViTri.AnyAsync(vt => vt.vtName == dto.vtName))
            {
                return BadRequest(new { Message = "Tên vị trí đã tồn tại." });
            }

            var viTri = new ViTri
            {
                vtName = dto.vtName
            };
            _context.ViTri.Add(viTri);
            await _context.SaveChangesAsync();
            return Ok(new { Message = "Thêm vị trí thành công!", vtid = viTri.vtid });
        }

        [HttpGet("list")]
        public async Task<IActionResult> GetAllViTri()
        {
            var viTris = await _context.ViTri
                .Select(vt => new { vt.vtid, vt.vtName })
                .ToListAsync();
            if (!viTris.Any())
            {
                return Ok(new { Message = "Không có vị trí nào trong hệ thống.", Data = new List<object>() });
            }
            return Ok(new
            {
                Message = "Lấy danh sách vị trí thành công!",
                Data = viTris
            });
        }

        [HttpPut("update/{id}")]
        public async Task<IActionResult> UpdateViTri(int id, [FromBody] ViTriDto dto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var viTri = await _context.ViTri.FindAsync(id);
            if (viTri == null)
            {
                return NotFound(new { Message = "Vị trí không tồn tại." });
            }

            if (await _context.ViTri.AnyAsync(vt => vt.vtName == dto.vtName && vt.vtid != id))
            {
                return BadRequest(new { Message = "Tên vị trí đã tồn tại." });
            }

            viTri.vtName = dto.vtName;
            await _context.SaveChangesAsync();
            return Ok(new { Message = "Cập nhật vị trí thành công!", vtid = viTri.vtid });
        }

        [HttpDelete("delete/{id}")]
        public async Task<IActionResult> DeleteViTri(int id)
        {
            using var transaction = await _context.Database.BeginTransactionAsync();
            try
            {
                var viTri = await _context.ViTri.FindAsync(id);
                if (viTri == null)
                {
                    return NotFound(new { Message = "Vị trí không tồn tại." });
                }

                var usedInTinTuyenDung = await _context.TInTuyenDung.AnyAsync(ttd => ttd.vitriID == id);
                if (usedInTinTuyenDung)
                {
                    return BadRequest(new { Message = "Không thể xóa vị trí vì đang được sử dụng trong tin tuyển dụng." });
                }

                _context.ViTri.Remove(viTri);
                await _context.SaveChangesAsync();
                await transaction.CommitAsync();
                return Ok(new { Message = "Xóa vị trí thành công!", vtid = id });
            }
            catch (Exception ex)
            {
                await transaction.RollbackAsync();
                return StatusCode(500, new { Message = "Lỗi khi xóa: " + ex.Message });
            }
        }
    }
}