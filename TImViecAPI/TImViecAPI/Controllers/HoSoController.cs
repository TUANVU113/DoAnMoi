using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Authorization;
using System.IO;
using TImViecAPI.Data;
using TImViecAPI.Model;
using System.ComponentModel.DataAnnotations;
using TImViecAPI.Model_Function.Dtos;
using TImViecAPI.Models;
using System.Linq;


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

        

        [HttpPost("create")]
        [Authorize(Roles = "UngVien")] // Chỉ ứng viên được tạo hồ sơ
        public async Task<IActionResult> CreateHoSo([FromForm] HoSoCreateDto dto)
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
        public async Task<IActionResult> UpdateHoSo(int hsid, [FromForm] HoSoCreateDto dto)
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


        //[HttpGet("view/{hsid}")]
        //[Authorize(Roles = "UngVien")]
        //public async Task<IActionResult> ViewHoSo(int hsid)
        //{
        //    string username = User.Identity?.Name;
        //    if (string.IsNullOrEmpty(username))
        //        return Unauthorized(new { Message = "Người dùng không hợp lệ hoặc chưa đăng nhập." });

        //    var ungVien = await _context.NguoiDung.FirstOrDefaultAsync(u => u.tkName == username);
        //    if (ungVien == null)
        //        return Unauthorized(new { Message = "Không tìm thấy thông tin ứng viên." });

        //    // Tìm hồ sơ của người này
        //    var hoSo = await _context.HoSo.FirstOrDefaultAsync(h => h.hsid == hsid && h.ungvienID == ungVien.tkid);
        //    if (hoSo == null)
        //        return NotFound(new { Message = "Hồ sơ không tồn tại hoặc không thuộc về bạn." });

        //    // Kiểm tra file tồn tại
        //    if (!System.IO.File.Exists(hoSo.ViTriFile))
        //        return NotFound(new { Message = "File không tồn tại trên máy chủ." });

        //    var fileBytes = await System.IO.File.ReadAllBytesAsync(hoSo.ViTriFile);
        //    var fileExtension = Path.GetExtension(hoSo.ViTriFile).ToLower();

        //    string contentType = fileExtension switch
        //    {
        //        ".pdf" => "application/pdf",
        //        ".doc" => "application/msword",
        //        ".docx" => "application/vnd.openxmlformats-officedocument.wordprocessingml.document",
        //        _ => "application/octet-stream"
        //    };

        //    return File(fileBytes, contentType, Path.GetFileName(hoSo.ViTriFile));
        //}

        [HttpGet("view/{hsid}")]
        [Authorize(Roles = "UngVien,NhaTuyenDung,Admin")]
        public async Task<IActionResult> ViewHoSo(int hsid)
        {
            var username = User.Identity?.Name;
            if (string.IsNullOrEmpty(username))
                return Unauthorized(new { Message = "Người dùng không hợp lệ hoặc chưa đăng nhập." });

            var nguoiDung = await _context.NguoiDung.FirstOrDefaultAsync(u => u.tkName == username);
            if (nguoiDung == null)
                return Unauthorized(new { Message = "Không tìm thấy người dùng." });

            var hoSo = await _context.HoSo.FirstOrDefaultAsync(h => h.hsid == hsid);

            if (hoSo == null)
                return NotFound(new { Message = "Không tìm thấy hồ sơ." });

            // 🔐 Nếu là ứng viên thì chỉ được xem hồ sơ của chính mình
            if (User.IsInRole("UngVien") && hoSo.ungvienID != nguoiDung.tkid)
                return Forbid();

            // 🗂 Đọc file
            if (!System.IO.File.Exists(hoSo.ViTriFile))
                return NotFound(new { Message = "File không tồn tại." });

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

        [HttpGet("cua-toi")]
        [Authorize(Roles = "UngVien")]
        public async Task<IActionResult> LayHoSoCuaToi()
        {
            // 1. LẤY USERNAME TỪ JWT
            string? username = User.Identity?.Name;
            if (string.IsNullOrEmpty(username))
                return Unauthorized(new { Message = "Vui lòng đăng nhập." });

            // 2. TÌM NGƯỜI DÙNG TRONG BẢNG NguoiDung
            var nguoiDung = _context.NguoiDung
                .FirstOrDefault(nd => nd.tkName == username);

            if (nguoiDung == null)
                return Unauthorized(new { Message = "Ứng viên không tồn tại." });

            int ungVienId = nguoiDung.tkid; // ← ĐÚNG: tkid = uvid

            // 3. LẤY TẤT CẢ HỒ SƠ CỦA ỨNG VIÊN
            var hoSoList = await _context.HoSo
                .Where(h => h.ungvienID == ungVienId)
                .Select(h => new
                {
                    HsId = h.hsid,
                    HsName = h.hsName ?? "Chưa đặt tên",
                    ViTriFile = h.ViTriFile != null
                        ? $"/uploads/cv/{Path.GetFileName(h.ViTriFile)}"
                        : null
                    
                })
                .OrderByDescending(h => h.HsId)
                .ToListAsync();

            // 4. TRẢ VỀ KẾT QUẢ
            return Ok(new
            {
                Message = "Lấy danh sách hồ sơ thành công!",
                UngVienId = ungVienId,
                TenDangNhap = nguoiDung.tkName,
                TongSo = hoSoList.Count,
                DanhSachHoSo = hoSoList
            });
        }
        [HttpPost("upload-bang-cap")]
        [Authorize(Roles = "UngVien")]
        public async Task<IActionResult> UploadBangCap([FromForm] BangCapUploadDto dto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            // Lấy user như cũ
            var username = User.Identity?.Name;
            var ungVien = _context.NguoiDung.FirstOrDefault(u => u.tkName == username);
            if (ungVien == null) return Unauthorized("Không tìm thấy ứng viên");

            // Kiểm tra hồ sơ
            var hoSo = await _context.HoSo
                .FirstOrDefaultAsync(h => h.hsid == dto.hoSoId && h.ungvienID == ungVien.tkid);
            if (hoSo == null) return NotFound("Hồ sơ không tồn tại");

            // Xử lý file
            if (dto.file == null || dto.file.Length == 0)
                return BadRequest("Chưa chọn file");

            var allowed = new[] { ".pdf", ".jpg", ".jpeg", ".png" };
            var ext = Path.GetExtension(dto.file.FileName).ToLower();
            if (!allowed.Contains(ext))
                return BadRequest("Chỉ chấp nhận PDF, JPG, PNG");

            var fileName = Guid.NewGuid() + ext;
            var path = Path.Combine("Upload", "bangcap", fileName);
            Directory.CreateDirectory(Path.GetDirectoryName(path)!);

            await using var stream = new FileStream(path, FileMode.Create);
            await dto.file.CopyToAsync(stream);

            var bangCap = new BangCapUpload
            {
                hosoID = dto.hoSoId,
                TenBangCap = dto.tenBangCap,
                Loai = dto.loai,
                FileUrl = $"/Upload/bangcap/{fileName}"
            };

            _context.BangCapUploads.Add(bangCap);
            await _context.SaveChangesAsync();

            return Ok(new
            {
                message = "Upload thành công",
                fileUrl = bangCap.FileUrl
            });
        }

        // 1. LẤY TẤT CẢ BẰNG CẤP CỦA MỘT HỒ SƠ
        [HttpGet("bang-cap/{hoSoId}")]
        [Authorize(Roles = "UngVien")]
        public async Task<ActionResult> GetBangCapCuaHoSo(int hoSoId)
        {
            var username = User.Identity?.Name;
            var ungVien = await _context.NguoiDung.FirstOrDefaultAsync(u => u.tkName == username);
            if (ungVien == null) return Unauthorized();

            var hoSo = await _context.HoSo
                .FirstOrDefaultAsync(h => h.hsid == hoSoId && h.ungvienID == ungVien.tkid);
            if (hoSo == null) return NotFound("Hồ sơ không tồn tại");

            var list = await _context.BangCapUploads
                .Where(b => b.hosoID == hoSoId)
                .OrderByDescending(b => b.NgayUpload)
                .Select(b => new
                {
                    b.Id,
                    b.TenBangCap,
                    b.Loai,
                    b.FileUrl,
                    NgayUpload = b.NgayUpload.ToString("dd/MM/yyyy HH:mm")
                })
                .ToListAsync();

            return Ok(list);
        }

        // 2. LẤY CHI TIẾT 1 BẰNG CẤP
        [HttpGet("bang-cap/chi-tiet/{id}")]
        [Authorize(Roles = "UngVien")]
        public async Task<ActionResult> GetChiTietBangCap(int id)
        {
            var bangCap = await _context.BangCapUploads
                .Include(b => b.HoSo)
                .FirstOrDefaultAsync(b => b.Id == id);

            if (bangCap == null) return NotFound();

            // Kiểm tra quyền sở hữu
            var username = User.Identity?.Name;
            var ungVien = await _context.NguoiDung.FirstOrDefaultAsync(u => u.tkName == username);
            if (bangCap.HoSo.ungvienID != ungVien.tkid) return Forbid();

            return Ok(new
            {
                bangCap.Id,
                bangCap.TenBangCap,
                bangCap.Loai,
                bangCap.FileUrl,
                NgayUpload = bangCap.NgayUpload.ToString("dd/MM/yyyy HH:mm")
            });
        }
        // DTO cho sửa
        public class SuaBangCapDto
        {
            [Required] public string TenBangCap { get; set; } = null!;
            [Required] public string Loai { get; set; } = null!;
            public IFormFile? File { get; set; } // ← CHO PHÉP THAY ẢNH
        }

        // 3. SỬA BẰNG CẤP (chỉ sửa tên + loại, không sửa file)
        [HttpPut("bang-cap/sua/{id}")]
        [Authorize(Roles = "UngVien")]
        public async Task<IActionResult> SuaBangCap(int id, [FromForm] SuaBangCapDto dto)
        {
            var bangCap = await _context.BangCapUploads
                .Include(b => b.HoSo)
                .FirstOrDefaultAsync(b => b.Id == id);

            if (bangCap == null) return NotFound();

            var username = User.Identity?.Name;
            var ungVien = await _context.NguoiDung.FirstOrDefaultAsync(u => u.tkName == username);
            if (bangCap.HoSo.ungvienID != ungVien.tkid) return Forbid();

            // CẬP NHẬT TÊN + LOẠI
            bangCap.TenBangCap = dto.TenBangCap;
            bangCap.Loai = dto.Loai;

            // NẾU CÓ ẢNH MỚI → THAY ẢNH
            if (dto.File != null && dto.File.Length > 0)
            {
                // XÓA ẢNH CŨ
                if (!string.IsNullOrEmpty(bangCap.FileUrl))
                {
                    var oldFileName = Path.GetFileName(bangCap.FileUrl); // ← SỬA DÒNG NÀY
                    var oldPath = Path.Combine("Upload", "bangcap", oldFileName);
                    if (System.IO.File.Exists(oldPath))
                        System.IO.File.Delete(oldPath);
                }

                // LƯU ẢNH MỚI
                var allowed = new[] { ".pdf", ".jpg", ".jpeg", ".png" };
                var ext = Path.GetExtension(dto.File.FileName).ToLower(); // ← SỬA DÒNG NÀY (bị gõ nhầm thành Get344Extension)
                if (!allowed.Contains(ext)) // ← DÒNG NÀY ĐÚNG RỒI, KHÔNG CẦN SỬA
                    return BadRequest("File không hợp lệ");

                var newFileName = Guid.NewGuid().ToString() + ext; // ← SỬA DÒNG NÀY (thêm .ToString())
                var newPath = Path.Combine("Upload", "bangcap", newFileName);
                Directory.CreateDirectory(Path.GetDirectoryName(newPath)!);

                await using var stream = new FileStream(newPath, FileMode.Create);
                await dto.File.CopyToAsync(stream);

                bangCap.FileUrl = $"/Upload/bangcap/{newFileName}";
            }

            await _context.SaveChangesAsync();
            return Ok(new { message = "Cập nhật thành công", fileUrl = bangCap.FileUrl });
        }



        // 4. XÓA BẰNG CẤP (xóa cả file trên server)
        [HttpDelete("bang-cap/xoa/{id}")]
        [Authorize(Roles = "UngVien")]
        public async Task<IActionResult> XoaBangCap(int id)
        {
            var bangCap = await _context.BangCapUploads
                .Include(b => b.HoSo)
                .FirstOrDefaultAsync(b => b.Id == id);

            if (bangCap == null) return NotFound();

            var username = User.Identity?.Name;
            var ungVien = await _context.NguoiDung.FirstOrDefaultAsync(u => u.tkName == username);
            if (bangCap.HoSo.ungvienID != ungVien.tkid) return Forbid();

            // XÓA FILE TRÊN SERVER
            if (System.IO.File.Exists("wwwroot" + bangCap.FileUrl))
            {
                System.IO.File.Delete("wwwroot" + bangCap.FileUrl);
            }

            _context.BangCapUploads.Remove(bangCap);
            await _context.SaveChangesAsync();

            return Ok(new { message = "Xóa thành công" });
        }
    }
}