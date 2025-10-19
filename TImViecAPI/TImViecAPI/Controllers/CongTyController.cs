using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;
using TImViecAPI.Data;
using TImViecAPI.Model;
using TImViecAPI.Model_Function.Dtos;

namespace TImViecAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CongTyController : ControllerBase
    {
        private readonly AppDbContext _context;

        public CongTyController(AppDbContext context)
        {
            _context = context;
        }

        // POST: api/congty/add - Thêm công ty mới
        // DTO chung cho add/update
        public class CongTyDto
        {
            [Required(ErrorMessage = "Tên công ty là bắt buộc.")]
            [StringLength(255)]
            public string CtName { get; set; }

            [StringLength(255)]
            public string? DiaChi { get; set; }

            [StringLength(255)]
            public string? Logo { get; set; }

            [StringLength(255)]
            public string? MieuTa { get; set; }

            [StringLength(255)]
            public string? MoHinh { get; set; }

            public int? SoNhanVien { get; set; }

            [StringLength(255)]
            public string? QuocGia { get; set; }

            [StringLength(255)]
            public string? NguoiLienHe { get; set; }

            [StringLength(255)]
            public string? SdtLienHe { get; set; }

            [StringLength(255)]
            public string? MaThue { get; set; }

            [StringLength(255)]
            public string? SdtCongTy { get; set; }
        }

        // POST: api/congty/add - Thêm mới
        [HttpPost("add")]
        public async Task<IActionResult> AddCongTy([FromBody] CongTyDto dto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            // Kiểm tra trùng tên công ty
            if (await _context.CongTy.AnyAsync(ct => ct.ctName == dto.CtName))
            {
                return BadRequest(new { Message = "Tên công ty đã tồn tại." });
            }

            // Kiểm tra trùng mã thuế (nếu cung cấp)
            if (!string.IsNullOrEmpty(dto.MaThue) && await _context.CongTy.AnyAsync(ct => ct.MaThue == dto.MaThue))
            {
                return BadRequest(new { Message = "Mã thuế đã được sử dụng." });
            }

            var congTy = new CongTy
            {
                ctName = dto.CtName,
                DiaChi = dto.DiaChi,
                Logo = dto.Logo,
                MieuTa = dto.MieuTa,
                MoHinh = dto.MoHinh,
                SoNhanVien = dto.SoNhanVien,
                QuocGia = dto.QuocGia,
                NguoiLienHe = dto.NguoiLienHe,
                sdtLienHe = dto.SdtLienHe,
                MaThue = dto.MaThue,
                sdtCongTy = dto.SdtCongTy
            };

            _context.CongTy.Add(congTy);
            await _context.SaveChangesAsync();

            return Ok(new { Message = "Thêm công ty thành công!", CtId = congTy.ctid });
        }

        // GET: api/congty/list - Lấy hết thông tin tất cả công ty
        [HttpGet("list")]
        public async Task<IActionResult> GetAllCongTy()
        {
            var congTys = await _context.CongTy
                .Select(ct => new
                {
                    ct.ctid,
                    ct.ctName,
                    ct.DiaChi,
                    ct.Logo,
                    ct.MieuTa,
                    ct.MoHinh,
                    ct.SoNhanVien,
                    ct.QuocGia,
                    ct.NguoiLienHe,
                    ct.sdtLienHe,
                    ct.MaThue,
                    ct.sdtCongTy
                })
                .ToListAsync();

            if (!congTys.Any())
            {
                return Ok(new { Message = "Không có công ty nào trong hệ thống.", Data = new List<object>() });
            }

            return Ok(new
            {
                Message = "Lấy danh sách công ty thành công!",
                Data = congTys
            });
        }

        // PUT: api/congty/update/{id} - Sửa theo ctid
        [HttpPut("update/{id}")]
        public async Task<IActionResult> UpdateCongTy(int id, [FromBody] CongTyDto dto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var congTy = await _context.CongTy.FindAsync(id);
            if (congTy == null)
            {
                return NotFound(new { Message = "Công ty không tồn tại." });
            }

            // Kiểm tra trùng tên (trừ bản thân)
            if (await _context.CongTy.AnyAsync(ct => ct.ctName == dto.CtName && ct.ctid != id))
            {
                return BadRequest(new { Message = "Tên công ty đã tồn tại." });
            }

            // Kiểm tra trùng mã thuế (trừ bản thân)
            if (!string.IsNullOrEmpty(dto.MaThue) && await _context.CongTy.AnyAsync(ct => ct.MaThue == dto.MaThue && ct.ctid != id))
            {
                return BadRequest(new { Message = "Mã thuế đã được sử dụng." });
            }

            // Cập nhật các trường
            congTy.ctName = dto.CtName;
            congTy.DiaChi = dto.DiaChi;
            congTy.Logo = dto.Logo;
            congTy.MieuTa = dto.MieuTa;
            congTy.MoHinh = dto.MoHinh;
            congTy.SoNhanVien = dto.SoNhanVien;
            congTy.QuocGia = dto.QuocGia;
            congTy.NguoiLienHe = dto.NguoiLienHe;
            congTy.sdtLienHe = dto.SdtLienHe;
            congTy.MaThue = dto.MaThue;
            congTy.sdtCongTy = dto.SdtCongTy;

            await _context.SaveChangesAsync();

            return Ok(new { Message = "Cập nhật công ty thành công!", CtId = congTy.ctid });
        }

        // DELETE: api/congty/delete/{id} - Xóa theo ctid
        [HttpDelete("delete/{id}")]
        public async Task<IActionResult> DeleteCongTy(int id)
        {
            using var transaction = await _context.Database.BeginTransactionAsync();
            try
            {
                var congTy = await _context.CongTy.FindAsync(id);
                if (congTy == null)
                {
                    return NotFound(new { Message = "Công ty không tồn tại." });
                }

                // Kiểm tra ràng buộc FK (NhaTuyenDung.ctID)
                var usedInNhaTuyenDung = await _context.NhaTuyenDung.AnyAsync(ntd => ntd.ctID == id);
                if (usedInNhaTuyenDung)
                {
                    return BadRequest(new { Message = "Không thể xóa công ty vì đang được sử dụng trong hồ sơ nhà tuyển dụng." });
                }

                // Xóa (các ràng buộc khác nếu có)
                _context.CongTy.Remove(congTy);
                await _context.SaveChangesAsync();
                await transaction.CommitAsync();

                return Ok(new { Message = "Xóa công ty thành công!", CtId = id });
            }
            catch (Exception ex)
            {
                await transaction.RollbackAsync();
                return StatusCode(500, new { Message = "Lỗi khi xóa: " + ex.Message });
            }
        }
    }
}
