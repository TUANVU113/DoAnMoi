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
    public class ChucDanhController : ControllerBase
    {
        private readonly AppDbContext _context;

        public ChucDanhController(AppDbContext context)
        {
            _context = context;
        }

        public class ChucDanhDto
        {
            [Required(ErrorMessage = "Tên chức danh là bắt buộc.")]
            [StringLength(255)]
            public string? cdName { get; set; }
        }

        [HttpPost("add")]
        public async Task<IActionResult> Add([FromBody] ChucDanhDto dto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            if (await _context.ChucDanh.AnyAsync(cd => cd.cdName == dto.cdName))
            {
                return BadRequest(new { Message = "Tên chức danh đã tồn tại." });
            }

            var chucDanh = new ChucDanh
            {
                cdName = dto.cdName
            };
            _context.ChucDanh.Add(chucDanh);
            await _context.SaveChangesAsync();
            return Ok(new { Message = "Thêm chức danh thành công!", cdid = chucDanh.cdid });
        }

        [HttpGet("list")]
        public async Task<IActionResult> GetAllChucDanh()
        {
            var chucDanhs = await _context.ChucDanh
                .Select(cd => new { cd.cdid, cd.cdName })
                .ToListAsync();
            if (!chucDanhs.Any())
            {
                return Ok(new { Message = "Không có chức danh nào trong hệ thống.", Data = new List<object>() });
            }
            return Ok(new
            {
                Message = "Lấy danh sách chức danh thành công!",
                Data = chucDanhs
            });
        }

        [HttpPut("update/{id}")]
        public async Task<IActionResult> UpdateChucDanh(int id, [FromBody] ChucDanhDto dto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var chucDanh = await _context.ChucDanh.FindAsync(id);
            if (chucDanh == null)
            {
                return NotFound(new { Message = "Chức danh không tồn tại." });
            }

            if (await _context.ChucDanh.AnyAsync(cd => cd.cdName == dto.cdName && cd.cdid != id))
            {
                return BadRequest(new { Message = "Tên chức danh đã tồn tại." });
            }

            chucDanh.cdName = dto.cdName;
            await _context.SaveChangesAsync();
            return Ok(new { Message = "Cập nhật chức danh thành công!", cdid = chucDanh.cdid });
        }

        [HttpDelete("delete/{id}")]
        public async Task<IActionResult> DeleteChucDanh(int id)
        {
            using var transaction = await _context.Database.BeginTransactionAsync();
            try
            {
                var chucDanh = await _context.ChucDanh.FindAsync(id);
                if (chucDanh == null)
                {
                    return NotFound(new { Message = "Chức danh không tồn tại." });
                }

                var usedInTinTuyenDung = await _context.TInTuyenDung.AnyAsync(ttd => ttd.chucdanhID == id);
                if (usedInTinTuyenDung)
                {
                    return BadRequest(new { Message = "Không thể xóa chức danh vì đang được sử dụng trong tin tuyển dụng." });
                }

                _context.ChucDanh.Remove(chucDanh);
                await _context.SaveChangesAsync();
                await transaction.CommitAsync();
                return Ok(new { Message = "Xóa chức danh thành công!", cdid = id });
            }
            catch (Exception ex)
            {
                await transaction.RollbackAsync();
                return StatusCode(500, new { Message = "Lỗi khi xóa: " + ex.Message });
            }
        }
    }
}