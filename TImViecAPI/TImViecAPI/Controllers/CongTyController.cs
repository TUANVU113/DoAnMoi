using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
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

        [HttpGet("cong-ty-cua-toi")]
        [Authorize(Roles = "NhaTuyenDung")]
        public async Task<IActionResult> GetCongTyCuaToi()
        {
            var username = User.Identity?.Name;

            // Lấy NTD đang đăng nhập → lấy công ty
            var congTy = await _context.NhaTuyenDung
                .Where(ntd => ntd.NguoiDung.tkName == username)
                .Select(ntd => ntd.CongTy)
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
                    ct.sdtCongTy,
                    // Thêm trạng thái kê khai
                    DaKeKhai = !string.IsNullOrEmpty(ct.ctName) && ct.ctName != "Công ty chưa kê khai"
                })
                .FirstOrDefaultAsync();

            if (congTy == null)
            {
                return NotFound(new { Message = "Không tìm thấy công ty của bạn." });
            }

            // Nếu chưa kê khai → gợi ý
            string message = congTy.DaKeKhai
                ? "Lấy thông tin công ty thành công!"
                : "Bạn chưa kê khai thông tin công ty. Vui lòng cập nhật để hiển thị đầy đủ trên tin tuyển dụng!";

            return Ok(new
            {
                Message = message,
                Data = congTy
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


        [HttpPost("upload-logo")]
        public async Task<IActionResult> UploadLogo([FromForm] IFormFile file)
        {
            if (file == null || file.Length == 0)
                return BadRequest(new { Message = "Không có file nào được upload." });

            var uploadsFolder = Path.Combine(Directory.GetCurrentDirectory(), "Upload");

            if (!Directory.Exists(uploadsFolder))
                Directory.CreateDirectory(uploadsFolder);

            var uniqueFileName = Guid.NewGuid() + Path.GetExtension(file.FileName);
            var filePath = Path.Combine(uploadsFolder, uniqueFileName);

            using (var stream = new FileStream(filePath, FileMode.Create))
            {
                await file.CopyToAsync(stream);
            }

            // Trả về URL mà frontend có thể dùng
            var url = $"/Upload/{uniqueFileName}";
            return Ok(new { Url = url });  // ✅ phải là "Url"
        }

        [HttpGet("logo/{filename}")]
        [AllowAnonymous]
        public async Task<IActionResult> ViewLogo(string filename)
        {
            if (string.IsNullOrEmpty(filename))
                return BadRequest(new { Message = "Tên file không hợp lệ." });

            // Đường dẫn file logo trong folder Upload
            var filePath = Path.Combine(Directory.GetCurrentDirectory(), "Upload", filename);

            if (!System.IO.File.Exists(filePath))
                return NotFound(new { Message = "File logo không tồn tại." });

            // Xác định content type dựa theo phần mở rộng
            var fileExtension = Path.GetExtension(filePath).ToLower();
            string contentType = fileExtension switch
            {
                ".jpg" or ".jpeg" => "image/jpeg",
                ".png" => "image/png",
                ".gif" => "image/gif",
                _ => "application/octet-stream"
            };

            // Trả file kèm content type
            var fileBytes = await System.IO.File.ReadAllBytesAsync(filePath);
            return File(fileBytes, contentType, Path.GetFileName(filePath));
        }

        [HttpPut("cap-nhat-cong-ty")]
        [Authorize(Roles = "NhaTuyenDung")]
        public async Task<IActionResult> CapNhatCongTy([FromBody] CapNhatCongTyDto dto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var username = User.Identity?.Name;

            var ntd = await _context.NhaTuyenDung
                .Include(ntd => ntd.CongTy)
                .FirstOrDefaultAsync(ntd => ntd.NguoiDung.tkName == username);

            if (ntd == null || ntd.CongTy == null)
                return NotFound("Không tìm thấy công ty của bạn.");

            var currentCtId = ntd.CongTy.ctid;

            // 1. KIỂM TRA TRÙNG TÊN CÔNG TY (TRỪ BẢN THÂN)
            if (!string.IsNullOrEmpty(dto.CtName) && dto.CtName != ntd.CongTy.ctName)
            {
                if (await _context.CongTy.AnyAsync(ct => ct.ctName == dto.CtName && ct.ctid != currentCtId))
                {
                    return BadRequest(new { Message = "Tên công ty đã tồn tại." });
                }
            }

            // 2. KIỂM TRA TRÙNG MÃ THUẾ (TRỪ BẢN THÂN)
            if (!string.IsNullOrEmpty(dto.MaThue) && dto.MaThue != ntd.CongTy.MaThue)
            {
                if (await _context.CongTy.AnyAsync(ct => ct.MaThue == dto.MaThue && ct.ctid != currentCtId))
                {
                    return BadRequest(new { Message = "Mã thuế đã được sử dụng." });
                }
            }

            // CẬP NHẬT THÔNG TIN CÔNG TY
            ntd.CongTy.ctName = dto.CtName ?? ntd.CongTy.ctName;
            ntd.CongTy.DiaChi = dto.DiaChi ?? ntd.CongTy.DiaChi;
            ntd.CongTy.Logo = dto.Logo ?? ntd.CongTy.Logo;
            ntd.CongTy.MieuTa = dto.MieuTa ?? ntd.CongTy.MieuTa;
            ntd.CongTy.MoHinh = dto.MoHinh ?? ntd.CongTy.MoHinh;
            ntd.CongTy.SoNhanVien = dto.SoNhanVien ?? ntd.CongTy.SoNhanVien;
            ntd.CongTy.QuocGia = dto.QuocGia ?? ntd.CongTy.QuocGia;
            ntd.CongTy.NguoiLienHe = dto.NguoiLienHe ?? ntd.CongTy.NguoiLienHe;
            ntd.CongTy.sdtLienHe = dto.SdtLienHe ?? ntd.CongTy.sdtLienHe;
            ntd.CongTy.MaThue = dto.MaThue ?? ntd.CongTy.MaThue;
            ntd.CongTy.sdtCongTy = dto.SdtCongTy ?? ntd.CongTy.sdtCongTy;

            await _context.SaveChangesAsync();

            return Ok(new { Message = "Cập nhật thông tin công ty thành công!" });
        }


    }
}
