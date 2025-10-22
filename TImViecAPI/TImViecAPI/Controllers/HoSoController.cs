using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Authorization;
using System.IO;
using TImViecAPI.Data;
using TImViecAPI.Model;
using System.ComponentModel.DataAnnotations;

namespace TImViecAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class HoSoController : ControllerBase
    {
        private readonly AppDbContext _context;
        private readonly IWebHostEnvironment _environment;

        public HoSoController(AppDbContext context, IWebHostEnvironment environment)
        {
            _context = context;
            _environment = environment;
        }

        public class HoSoDto
        {
            [Required(ErrorMessage = "Tên hồ sơ là bắt buộc.")]
            [StringLength(255)]
            public string? hsName { get; set; }

            // File upload sẽ được gửi qua form-data (không cần DTO)
        }

        [HttpPost("create")]
        [Authorize(Roles = "UngVien")] // Chỉ ứng viên được tạo hồ sơ
        public async Task<IActionResult> CreateHoSo([FromForm] HoSoDto dto)
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
                return Unauthorized(new { Message = "Ứng viên không tồn tại ."});
            }

                // Xử lý file upload
                var file = Request.Form.Files.GetFile("file");
            if (file == null || file.Length == 0)
            {
                return BadRequest(new { Message = "Vui lòng chọn file để upload." });
            }

            // Kiểm tra định dạng file
            var allowedExtensions = new[] { ".pdf", ".doc", ".docx" };
            var fileExtension = Path.GetExtension(file.FileName).ToLower();
            if (!allowedExtensions.Contains(fileExtension))
            {
                return BadRequest(new { Message = "Chỉ chấp nhận file .pdf, .doc, .docx." });
            }

            // Tạo đường dẫn lưu file
            var uploadsFolder = Path.Combine(_environment.ContentRootPath, "Upload");
            if (!Directory.Exists(uploadsFolder))
            {
                Directory.CreateDirectory(uploadsFolder);
            }

            // Tạo tên file duy nhất
            var fileName = $"{Guid.NewGuid()}_{file.FileName}";
            var filePath = Path.Combine(uploadsFolder, fileName);

            // Lưu file
            using (var stream = new FileStream(filePath, FileMode.Create))
            {
                await file.CopyToAsync(stream);
            }

            // Tạo và lưu HoSo
            var hoSo = new HoSo
            {
                hsName = dto.hsName,
                ViTriFile = filePath, // Lưu đường dẫn tuyệt đối
                ungvienID = ungVien.tkid
            };

            _context.HoSo.Add(hoSo);
            await _context.SaveChangesAsync();

            return Ok(new
            {
                Message = "Tạo hồ sơ thành công!",
                HsId = hoSo.hsid,
                ViTriFile = hoSo.ViTriFile
            });
        }

        [HttpGet("get/{hsid}")]
        [Authorize(Roles = "UngVien")]
        public async Task<IActionResult> GetHoSo(int hsid)
        {
            string username = User.Identity.Name;
            if (username == null)
            {
                return Unauthorized(new { Message = "Người dùng không hợp lệ hoặc không phải Ưng viên." });
            }
            var ungVien = _context.UngVien.FirstOrDefault(ntd => ntd.uvName == username);

            var hoSo = await _context.HoSo
                .FirstOrDefaultAsync(h => h.hsid == hsid);
            if (hoSo == null)
            {
                return NotFound(new { Message = "Hồ sơ không tồn tại hoặc không thuộc về bạn." });
            }

            return Ok(new
            {
                Message = "Lấy hồ sơ thành công!",
                HsId = hoSo.hsid,
                HsName = hoSo.hsName,
                ViTriFile = hoSo.ViTriFile
            });
        }

        [HttpDelete("delete/{hsid}")]
        [Authorize(Roles = "UngVien")]
        public async Task<IActionResult> DeleteHoSo(int hsid)
        {
            string username = User.Identity.Name;
            if (username == null)
            {
                return Unauthorized(new { Message = "Người dùng không hợp lệ hoặc không phải Ưng viên." });
            }
            var ungVien = _context.UngVien.FirstOrDefault(ntd => ntd.uvName == username);

            var hoSo = await _context.HoSo
                .FirstOrDefaultAsync(h => h.hsid == hsid);
            if (hoSo == null)
            {
                return NotFound(new { Message = "Hồ sơ không tồn tại hoặc không thuộc về bạn." });
            }

            // Xóa file trên đĩa
            if (!string.IsNullOrEmpty(hoSo.ViTriFile) && System.IO.File.Exists(hoSo.ViTriFile))
            {
                System.IO.File.Delete(hoSo.ViTriFile);
            }

            _context.HoSo.Remove(hoSo);
            await _context.SaveChangesAsync();

            return Ok(new
            {
                Message = "Xóa hồ sơ thành công!",
                HsId = hsid
            });
        }

        [HttpPut("update/{hsid}")]
        [Authorize(Roles = "UngVien")]
        public async Task<IActionResult> UpdateHoSo(int hsid, [FromForm] HoSoDto dto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var uvid = User.FindFirst("uvid")?.Value;
            if (string.IsNullOrEmpty(uvid) || !int.TryParse(uvid, out int uvidValue))
            {
                return Unauthorized(new { Message = "Không tìm thấy thông tin ứng viên." });
            }

            var hoSo = await _context.HoSo
                .FirstOrDefaultAsync(h => h.hsid == hsid && h.ungvienID == uvidValue);
            if (hoSo == null)
            {
                return NotFound(new { Message = "Hồ sơ không tồn tại hoặc không thuộc về bạn." });
            }

            // Cập nhật hsName
            hoSo.hsName = dto.hsName;

            // Xử lý file mới (nếu có)
            var file = Request.Form.Files.GetFile("file");
            if (file != null && file.Length > 0)
            {
                var allowedExtensions = new[] { ".pdf", ".doc", ".docx" };
                var fileExtension = Path.GetExtension(file.FileName).ToLower();
                if (!allowedExtensions.Contains(fileExtension))
                {
                    return BadRequest(new { Message = "Chỉ chấp nhận file .pdf, .doc, .docx." });
                }

                var uploadsFolder = Path.Combine(_environment.ContentRootPath, "Upload");
                if (!Directory.Exists(uploadsFolder))
                {
                    Directory.CreateDirectory(uploadsFolder);
                }

                // Xóa file cũ (nếu tồn tại)
                if (!string.IsNullOrEmpty(hoSo.ViTriFile) && System.IO.File.Exists(hoSo.ViTriFile))
                {
                    System.IO.File.Delete(hoSo.ViTriFile);
                }

                var fileName = $"{Guid.NewGuid()}_{file.FileName}";
                var filePath = Path.Combine(uploadsFolder, fileName);

                using (var stream = new FileStream(filePath, FileMode.Create))
                {
                    await file.CopyToAsync(stream);
                }

                hoSo.ViTriFile = filePath;
            }

            await _context.SaveChangesAsync();

            return Ok(new
            {
                Message = "Cập nhật hồ sơ thành công!",
                HsId = hoSo.hsid,
                HsName = hoSo.hsName,
                ViTriFile = hoSo.ViTriFile
            });
        }



        [HttpGet("list")]
        [Authorize(Roles = "UngVien")]
        public async Task<IActionResult> GetListHoSo()
        {
            // Lấy tên đăng nhập từ token
            string username = User.Identity?.Name;
            if (string.IsNullOrEmpty(username))
            {
                return Unauthorized(new { Message = "Người dùng không hợp lệ hoặc chưa đăng nhập." });
            }

            // Tìm ứng viên tương ứng trong bảng NguoiDung
            var ungVien = await _context.NguoiDung
                .FirstOrDefaultAsync(u => u.tkName == username);

            if (ungVien == null)
            {
                return Unauthorized(new { Message = "Không tìm thấy thông tin ứng viên." });
            }

            // Lấy danh sách hồ sơ chỉ của ứng viên này
            var hoSos = await _context.HoSo
                .Where(h => h.ungvienID == ungVien.tkid)
                .Select(h => new
                {
                    h.hsid,
                    h.hsName,
                    h.ViTriFile
                })
                .ToListAsync();

            return Ok(new
            {
                Message = "Lấy danh sách hồ sơ thành công!",
                Data = hoSos
            });
        }


        [HttpGet("view/{hsid}")]
        [Authorize(Roles = "UngVien")]
        public async Task<IActionResult> ViewHoSo(int hsid)
        {
            string username = User.Identity?.Name;
            if (string.IsNullOrEmpty(username))
                return Unauthorized(new { Message = "Người dùng không hợp lệ hoặc chưa đăng nhập." });

            var ungVien = await _context.NguoiDung.FirstOrDefaultAsync(u => u.tkName == username);
            if (ungVien == null)
                return Unauthorized(new { Message = "Không tìm thấy thông tin ứng viên." });

            // Tìm hồ sơ của người này
            var hoSo = await _context.HoSo.FirstOrDefaultAsync(h => h.hsid == hsid && h.ungvienID == ungVien.tkid);
            if (hoSo == null)
                return NotFound(new { Message = "Hồ sơ không tồn tại hoặc không thuộc về bạn." });

            // Kiểm tra file tồn tại
            if (!System.IO.File.Exists(hoSo.ViTriFile))
                return NotFound(new { Message = "File không tồn tại trên máy chủ." });

            var fileBytes = await System.IO.File.ReadAllBytesAsync(hoSo.ViTriFile);
            var fileExtension = Path.GetExtension(hoSo.ViTriFile).ToLower();

            string contentType = fileExtension switch
            {
                ".pdf" => "application/pdf",
                ".doc" => "application/msword",
                ".docx" => "application/vnd.openxmlformats-officedocument.wordprocessingml.document",
                _ => "application/octet-stream"
            };

            return File(fileBytes, contentType, Path.GetFileName(hoSo.ViTriFile));
        }

    }
}