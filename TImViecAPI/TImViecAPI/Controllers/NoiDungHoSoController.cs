using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Hosting;
using TImViecAPI.Data;
using TImViecAPI.Model;
using TImViecAPI.Model_Function.Dtos;

namespace TImViecAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize(Roles = "UngVien")]
    public class NoiDungHoSoController : ControllerBase
    {
        private readonly AppDbContext _context;

        public NoiDungHoSoController(AppDbContext context)
        {
            _context = context;
        }
        [HttpPost("tao-cv-nhanh")]
        [RequestFormLimits(MultipartBodyLengthLimit = 10 * 1024 * 1024)]
        [RequestSizeLimit(10 * 1024 * 1024)]
        public async Task<ActionResult<TaoCVNhanhResponse>> TaoCVNhanh([FromForm] TaoCVNhanhDto dto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            // === LẤY USER ===
            var username = User.Identity?.Name;
            var nguoiDung = await _context.NguoiDung.FirstOrDefaultAsync(u => u.tkName == username);
            if (nguoiDung == null) return Unauthorized();

            var ungVien = await _context.UngVien.FirstOrDefaultAsync(u => u.uvid == nguoiDung.tkid);
            if (ungVien == null) return NotFound("Không tìm thấy ứng viên.");

            string? avatarPath = null;

            // === UPLOAD VÀO THƯ MỤC Upload TRONG PROJECT ===
            if (dto.AvatarFile != null && dto.AvatarFile.Length > 0)
            {
                var allowed = new[] { ".jpg", ".jpeg", ".png", ".gif" };
                var ext = Path.GetExtension(dto.AvatarFile.FileName).ToLower();
                if (!allowed.Contains(ext))
                    return BadRequest("Chỉ chấp nhận .jpg, .png, .gif");

                if (dto.AvatarFile.Length > 5 * 1024 * 1024)
                    return BadRequest("Ảnh tối đa 5MB");

                var fileName = $"{Guid.NewGuid()}{ext}";
                var uploadFolder = Path.Combine(Directory.GetCurrentDirectory(), "Upload");
                var filePath = Path.Combine(uploadFolder, fileName);

                // Tạo thư mục nếu chưa có
                Directory.CreateDirectory(uploadFolder);

                // Lưu file
                await using (var stream = new FileStream(filePath, FileMode.Create))
                {
                    await dto.AvatarFile.CopyToAsync(stream);
                }

                // Lưu đường dẫn vào DB
                avatarPath = $"/Upload/{fileName}";
            }

            try
            {
                var hoSo = new HoSo
                {
                    hsName = dto.TenHoSo,
                    ungvienID = ungVien.uvid,
                    ViTriFile = null
                };
                _context.HoSo.Add(hoSo);
                await _context.SaveChangesAsync();

                var noiDung = new NoiDungHoSo
                {
                    hosoID = hoSo.hsid,
                    TenUngVien = dto.TenUngVien,
                    PhoneHoSo = dto.PhoneHoSo,
                    MailHoSo = dto.MailHoSo,
                    HocVan = dto.HocVan,
                    NamKinhNghiemID = dto.NamKinhNghiemID,
                    MucLuong = dto.MucLuong,
                    ChucDanhID = dto.ChucDanhID,
                    LoaiHinhLamViecID = dto.LoaiHinhLamViecID,
                    LinhVucID = dto.LinhVucID,
                    ViTriLamViecID = dto.ViTriLamViecID,
                    MucTieu = dto.MucTieu,
                    ChucChi = dto.ChucChi,
                    KyNang = dto.KyNang,    
                    Avata = avatarPath
                };

                _context.NoiDungHoSo.Add(noiDung);
                await _context.SaveChangesAsync();

                return Ok(new TaoCVNhanhResponse
                {
                    HoSoID = hoSo.hsid,
                    NoiDungID = noiDung.ndid,
                    TenHoSo = hoSo.hsName,
                    Message = "Tạo CV thành công!",
                    NgayTao = DateTime.Now
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Lỗi: {ex.Message}");
            }
        }
        // GET: api/NoiDungHoSo (Lấy tất cả nội dung hồ sơ)
        [HttpGet("danh-sach")]
        public async Task<ActionResult<IEnumerable<HoSoDto>>> GetDanhSach()
        {
            var username = User.Identity?.Name;
            var nguoiDung = await _context.NguoiDung.FirstOrDefaultAsync(u => u.tkName == username);
            if (nguoiDung == null) return Unauthorized();

            var ungVienId = nguoiDung.tkid;

            var hoSoList = await _context.HoSo
                .Where(h => h.ungvienID == ungVienId)
                .Include(h => h.NoiDungHoSo)
                .Select(h => new HoSoDto
                {
                    HoSoId = h.hsid,
                    HoSoName = h.hsName,
                    TenUngVien = h.NoiDungHoSo!.TenUngVien,
                    Avata = h.NoiDungHoSo.Avata,
                    FileUrl = h.ViTriFile,
                    NgayTao = DateTime.Now // hoặc thêm trường vào DB
                })
                .OrderByDescending(h => h.NgayTao)
                .ToListAsync();

            return Ok(hoSoList);
        }

        [HttpGet("{hoSoId}")]
        public async Task<ActionResult<HoSoDetailDto>> GetHoSo(int hoSoId)
        {
            var username = User.Identity?.Name;
            var nguoiDung = await _context.NguoiDung.FirstOrDefaultAsync(u => u.tkName == username);
            if (nguoiDung == null) return Unauthorized();

            var hoSo = await _context.HoSo
                .Include(h => h.NoiDungHoSo)
                .FirstOrDefaultAsync(h => h.hsid == hoSoId && h.ungvienID == nguoiDung.tkid);

            if (hoSo == null) return NotFound();

            return Ok(new HoSoDetailDto
            {
                HoSoId = hoSo.hsid,
                HoSoName = hoSo.hsName,
                TenUngVien = hoSo.NoiDungHoSo!.TenUngVien,
                PhoneHoSo = hoSo.NoiDungHoSo.PhoneHoSo,
                MailHoSo = hoSo.NoiDungHoSo.MailHoSo,
                HocVan = hoSo.NoiDungHoSo.HocVan,
                NamKinhNghiemID = hoSo.NoiDungHoSo.NamKinhNghiemID,
                MucLuong = hoSo.NoiDungHoSo.MucLuong,
                ChucDanhID = hoSo.NoiDungHoSo.ChucDanhID,
                LoaiHinhLamViecID = hoSo.NoiDungHoSo.LoaiHinhLamViecID,
                LinhVucID = hoSo.NoiDungHoSo.LinhVucID,
                ViTriLamViecID = hoSo.NoiDungHoSo.ViTriLamViecID,
                MucTieu = hoSo.NoiDungHoSo.MucTieu,
                ChucChi = hoSo.NoiDungHoSo.ChucChi,
                KyNang = hoSo.NoiDungHoSo.KyNang,
                Avata = hoSo.NoiDungHoSo.Avata,
                FileUrl = hoSo.ViTriFile,
                NgayTao = DateTime.Now
            });
        }

        // ==================== UPDATE (sửa CV) ====================
        [HttpPut("{hoSoId}")]
        public async Task<IActionResult> CapNhatHoSo(int hoSoId, [FromForm] TaoCVNhanhDto dto)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);

            var username = User.Identity?.Name;
            var nguoiDung = await _context.NguoiDung.FirstOrDefaultAsync(u => u.tkName == username);
            if (nguoiDung == null) return Unauthorized();

            var hoSo = await _context.HoSo
                .Include(h => h.NoiDungHoSo)
                .FirstOrDefaultAsync(h => h.hsid == hoSoId && h.ungvienID == nguoiDung.tkid);

            if (hoSo == null) return NotFound("Không tìm thấy hồ sơ.");

            string? newAvatarPath = hoSo.NoiDungHoSo!.Avata;

            // XỬ LÝ ẢNH MỚI
            if (dto.AvatarFile != null && dto.AvatarFile.Length > 0)
            {
                var allowed = new[] { ".jpg", ".jpeg", ".png", ".gif" };
                var ext = Path.GetExtension(dto.AvatarFile.FileName).ToLower();
                if (!allowed.Contains(ext)) return BadRequest("Chỉ chấp nhận .jpg, .png, .gif");
                if (dto.AvatarFile.Length > 5 * 1024 * 1024) return BadRequest("Ảnh tối đa 5MB");

                // XÓA ẢNH CŨ
                if (!string.IsNullOrEmpty(hoSo.NoiDungHoSo.Avata))
                {
                    var oldPath = Path.Combine(Directory.GetCurrentDirectory(), "Upload",
                        hoSo.NoiDungHoSo.Avata.Replace("/Upload/", ""));
                    if (System.IO.File.Exists(oldPath)) System.IO.File.Delete(oldPath);
                }

                // LƯU ẢNH MỚI
                //var fileName = $"{Guid.NewGuid()}{ext}";
                //var filePath = Path.Combine(Directory.GetCurrentDirectory(), "Upload", fileName);
                //await using (var stream = new FileStream(filePath, FileMode.Create))
                //{
                //    await dto.AvatarFile.CopyToAsync(stream);
                //}
                //newAvatarPath = $ocyanate / Upload /{ fileName}
                //";
            }

            // CẬP NHẬT DỮ LIỆU
            hoSo.hsName = dto.TenHoSo;
            hoSo.NoiDungHoSo!.TenUngVien = dto.TenUngVien;
            hoSo.NoiDungHoSo.PhoneHoSo = dto.PhoneHoSo;
            hoSo.NoiDungHoSo.MailHoSo = dto.MailHoSo;
            hoSo.NoiDungHoSo.HocVan = dto.HocVan;
            hoSo.NoiDungHoSo.NamKinhNghiemID = dto.NamKinhNghiemID;
            hoSo.NoiDungHoSo.MucLuong = dto.MucLuong;
            hoSo.NoiDungHoSo.ChucDanhID = dto.ChucDanhID;
            hoSo.NoiDungHoSo.LoaiHinhLamViecID = dto.LoaiHinhLamViecID;
            hoSo.NoiDungHoSo.LinhVucID = dto.LinhVucID;
            hoSo.NoiDungHoSo.ViTriLamViecID = dto.ViTriLamViecID;
            hoSo.NoiDungHoSo.MucTieu = dto.MucTieu;
            hoSo.NoiDungHoSo.ChucChi = dto.ChucChi;
            hoSo.NoiDungHoSo.Avata = newAvatarPath;

            await _context.SaveChangesAsync();

            return Ok(new { Message = "Cập nhật CV thành công!" });
        }

        // ==================== DELETE (xóa CV + ảnh) ====================
        [HttpDelete("{hoSoId}")]
        public async Task<IActionResult> XoaHoSo(int hoSoId)
        {
            var username = User.Identity?.Name;
            var nguoiDung = await _context.NguoiDung.FirstOrDefaultAsync(u => u.tkName == username);
            if (nguoiDung == null) return Unauthorized();

            var hoSo = await _context.HoSo
                .Include(h => h.NoiDungHoSo)
                .FirstOrDefaultAsync(h => h.hsid == hoSoId && h.ungvienID == nguoiDung.tkid);

            if (hoSo == null) return NotFound("Không tìm thấy hồ sơ.");

            // XÓA ẢNH
            if (!string.IsNullOrEmpty(hoSo.NoiDungHoSo!.Avata))
            {
                var filePath = Path.Combine(Directory.GetCurrentDirectory(), "Upload",
                    hoSo.NoiDungHoSo.Avata.Replace("/Upload/", ""));
                if (System.IO.File.Exists(filePath)) System.IO.File.Delete(filePath);
            }

            _context.HoSo.Remove(hoSo);
            await _context.SaveChangesAsync();

            return Ok(new { Message = "Xóa CV thành công!" });
        }

    }
}

