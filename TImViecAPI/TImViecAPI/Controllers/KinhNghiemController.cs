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
    public class KinhNghiemController : ControllerBase
    {
        private readonly AppDbContext _context;

        public KinhNghiemController(AppDbContext context)
        {
            _context = context;
        }

        public class KinhNghiemDto
        {
            [Required(ErrorMessage = "Tên kinh nghiệm là bắt buộc.")]
            [StringLength(255)]
            public string? knName { get; set; }
        }

        [HttpPost("add")]
        public async Task<IActionResult> Add([FromBody] KinhNghiemDto dto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            if (await _context.KinhNghiem.AnyAsync(kn => kn.knName == dto.knName))
            {
                return BadRequest(new { Message = "Tên kinh nghiệm đã tồn tại." });
            }

            var kinhNghiem = new KinhNghiem
            {
                knName = dto.knName
            };
            _context.KinhNghiem.Add(kinhNghiem);
            await _context.SaveChangesAsync();
            return Ok(new { Message = "Thêm kinh nghiệm thành công!", knid = kinhNghiem.knid });
        }

        [HttpGet("list")]
        public async Task<IActionResult> GetAllKinhNghiem()
        {
            var kinhNghiems = await _context.KinhNghiem
                .Select(kn => new { kn.knid, kn.knName })
                .ToListAsync();
            if (!kinhNghiems.Any())
            {
                return Ok(new { Message = "Không có kinh nghiệm nào trong hệ thống.", Data = new List<object>() });
            }
            return Ok(new
            {
                Message = "Lấy danh sách kinh nghiệm thành công!",
                Data = kinhNghiems
            });
        }

        [HttpPut("update/{id}")]
        public async Task<IActionResult> UpdateKinhNghiem(int id, [FromBody] KinhNghiemDto dto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var kinhNghiem = await _context.KinhNghiem.FindAsync(id);
            if (kinhNghiem == null)
            {
                return NotFound(new { Message = "Kinh nghiệm không tồn tại." });
            }

            if (await _context.KinhNghiem.AnyAsync(kn => kn.knName == dto.knName && kn.knid != id))
            {
                return BadRequest(new { Message = "Tên kinh nghiệm đã tồn tại." });
            }

            kinhNghiem.knName = dto.knName;
            await _context.SaveChangesAsync();
            return Ok(new { Message = "Cập nhật kinh nghiệm thành công!", knid = kinhNghiem.knid });
        }

        [HttpDelete("delete/{id}")]
        public async Task<IActionResult> DeleteKinhNghiem(int id)
        {
            using var transaction = await _context.Database.BeginTransactionAsync();
            try
            {
                var kinhNghiem = await _context.KinhNghiem.FindAsync(id);
                if (kinhNghiem == null)
                {
                    return NotFound(new { Message = "Kinh nghiệm không tồn tại." });
                }

                var usedInTinTuyenDung = await _context.TInTuyenDung.AnyAsync(ttd => ttd.kinhnghiemID == id);
                if (usedInTinTuyenDung)
                {
                    return BadRequest(new { Message = "Không thể xóa kinh nghiệm vì đang được sử dụng trong tin tuyển dụng." });
                }

                _context.KinhNghiem.Remove(kinhNghiem);
                await _context.SaveChangesAsync();
                await transaction.CommitAsync();
                return Ok(new { Message = "Xóa kinh nghiệm thành công!", knid = id });
            }
            catch (Exception ex)
            {
                await transaction.RollbackAsync();
                return StatusCode(500, new { Message = "Lỗi khi xóa: " + ex.Message });
            }
        }
    }
}