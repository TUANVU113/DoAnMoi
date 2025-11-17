using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using TImViecAPI.Data;
using TImViecAPI.Model;
using System.Security.Claims;
using TImViecAPI.Model_Function.Dtos;
using static TImViecAPI.Controllers.TInTuyenDungController;
using TImViecAPI.Helpers;

namespace TImViecAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize(Roles = "UngVien")] // BẮT BUỘC ĐĂNG NHẬP
    public class UngTuyenController : ControllerBase
    {
        private readonly AppDbContext _context;

        public UngTuyenController(AppDbContext context)
        {
            _context = context;
        }

        [HttpPost("nop-don")]
        public async Task<IActionResult> NopDon([FromBody] UngTuyenRequestDto dto)
        {
            string username = User.Identity.Name;
            if (username == null)
            {
                return Unauthorized(new { Message = "Người dùng không hợp lệ hoặc không phải Ưng viên." });
            }
            var ungVien = _context.NguoiDung.FirstOrDefault(ntd => ntd.tkName == username);

            if (ungVien == null)
            {
                return Unauthorized(new { Message = "Ứng viên không tồn tại ." });
            }
            int ungVienId = ungVien.tkid; // ← KHAI BÁO BIẾN
            // 1. Lấy ID người dùng từ JWT
           //var nguoiDungId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value!);

            // 2. Kiểm tra ứng viên tồn tại
            //var ungVien = await _context.UngVien
            //.FirstOrDefaultAsync(uv => uv.uvid == nguoiDungId);

            //if (ungVien == null)
            //    return BadRequest(new { Message = "Ứng viên không tồn tại." });

            // 3. Kiểm tra tin tuyển dụng tồn tại + đã duyệt
            var tin = await _context.TInTuyenDung
                .Include(t => t.NhaTuyenDung).ThenInclude(n => n!.CongTy)
                .FirstOrDefaultAsync(t => t.ttdid == dto.TinTuyenDungId && t.DaDuyet == true);

            if (tin == null)
                return NotFound(new { Message = "Tin tuyển dụng không tồn tại hoặc chưa được duyệt." });

            // 4. Kiểm tra hồ sơ thuộc về ứng viên
            var hoSo = await _context.HoSo
                .FirstOrDefaultAsync(h => h.hsid == dto.HoSoId && h.ungvienID == ungVien.tkid);

            if (hoSo == null)
                return BadRequest(new { Message = "Hồ sơ không hợp lệ hoặc không thuộc về bạn." });

            // 5. Kiểm tra đã ứng tuyển chưa
            var daUngTuyen = await _context.UngVien_UngTuyen
                .AnyAsync(uu => uu.ungvienID == ungVien.tkid &&
                               _context.UngTuyen.Any(ut => ut.utid == uu.ungtuyenID && ut.tintuyendungid == dto.TinTuyenDungId));

            if (daUngTuyen)
                return Conflict(new { Message = "Bạn đã ứng tuyển tin này rồi!" });

            // 5.5. CẬP NHẬT HOẶC TẠO THÔNG TIN CÁ NHÂN
            var thongTin = await _context.thongTinCaNhans
                .FirstOrDefaultAsync(t => t.ungvienID == ungVienId);

            if (thongTin == null && HasThongTinCaNhan(dto))
            {
                thongTin = new ThongTinCaNhan { ungvienID = ungVienId };
                _context.thongTinCaNhans.Add(thongTin);
            }
            else if (thongTin != null && HasThongTinCaNhan(dto))
            {
                // Cập nhật nếu có dữ liệu mới
                UpdateThongTinCaNhan(thongTin, dto);
            }

            // Helper methods
            bool HasThongTinCaNhan(UngTuyenRequestDto dto) =>
                !string.IsNullOrEmpty(dto.HoVaTen) ||
                !string.IsNullOrEmpty(dto.SDT) ||
                !string.IsNullOrEmpty(dto.Email) ||
                dto.NgaySinh.HasValue;

            void UpdateThongTinCaNhan(ThongTinCaNhan tt, UngTuyenRequestDto dto)
            {
                if (!string.IsNullOrEmpty(dto.HoVaTen)) tt.HoVaTen = dto.HoVaTen;
                if (!string.IsNullOrEmpty(dto.GioiTinh)) tt.GioiTinh = dto.GioiTinh;
                if (dto.NgaySinh.HasValue) tt.NgaySinh = dto.NgaySinh.Value.Date;
                if (!string.IsNullOrEmpty(dto.SDT)) tt.SDT = dto.SDT;
                if (!string.IsNullOrEmpty(dto.Email)) tt.Email = dto.Email;
                if (!string.IsNullOrEmpty(dto.QuocGia)) tt.QuocGia = dto.QuocGia;
                if (!string.IsNullOrEmpty(dto.Tinh)) tt.Tinh = dto.Tinh;
                if (!string.IsNullOrEmpty(dto.Huyen)) tt.Huyen = dto.Huyen;
                if (!string.IsNullOrEmpty(dto.DiaChi)) tt.DiaChi = dto.DiaChi;
                if (!string.IsNullOrEmpty(dto.CCCD)) tt.CCCD = dto.CCCD;
                if (!string.IsNullOrEmpty(dto.NoiSinh)) tt.NoiSinh = dto.NoiSinh;
            }

            // 6. Tạo bản ghi ứng tuyển
            var ungTuyen = new UngTuyen
            {
                NgayNop = DateTime.Now,
                TrangThai = "Đang chờ duyệt",
                tintuyendungid = dto.TinTuyenDungId
            };

            _context.UngTuyen.Add(ungTuyen);
            await _context.SaveChangesAsync(); // Lưu để lấy utid

            // 7. Liên kết ứng viên ↔ ứng tuyển
            _context.UngVien_UngTuyen.Add(new UngVien_UngTuyen
            {
                ungvienID = ungVien.tkid,
                ungtuyenID = ungTuyen.utid
            });

            // 8. Liên kết hồ sơ ↔ ứng tuyển
            _context.HoSo_UngTuyen.Add(new HoSo_UngTuyen
            {
                hosoID = dto.HoSoId,
                ungtuyenID = ungTuyen.utid
            });

            await _context.SaveChangesAsync();

            // Gửi thông báo cho ứng viên
            var thongBao = new ThongBao
            {
                NoiDung = $"Bạn đã nộp đơn ứng tuyển thành công cho tin \"{tin.TieuDe}\" tại {tin.NhaTuyenDung?.CongTy?.ctName ?? "Công ty"}.",
                NgayBao = DateTime.Now
            };
            _context.ThongBao.Add(thongBao);
            await _context.SaveChangesAsync(); // Lưu để lấy tbid

            // Liên kết ứng viên ↔ thông báo
            _context.NguoiDung_ThongBao.Add(new NguoiDung_ThongBao
            {
                nguoidungID = ungVien.tkid,
                thongbaoID = thongBao.tbid,
                DaXem = false
            });

            await _context.SaveChangesAsync();

            // 9. Trả về kết quả
            return Ok(new UngTuyenResponseDto
            {
                UngTuyenId = ungTuyen.utid,
                NgayNop = ungTuyen.NgayNop ?? DateTime.Now,
                TrangThai = ungTuyen.TrangThai,
                TieuDeTin = tin.TieuDe,
                TenCongTy = tin.NhaTuyenDung?.CongTy?.ctName ?? "Không rõ"
            });
        }

        // lấy thông tin ứng tuyển theo Id
        [HttpGet("cua-toi")]
        public async Task<IActionResult> CuaToi()
        {
            string? username = User.Identity?.Name;
            if (string.IsNullOrEmpty(username))
                return Unauthorized(new { Message = "Vui lòng đăng nhập." });

            var nguoiDung = _context.NguoiDung.FirstOrDefault(nd => nd.tkName == username);
            if (nguoiDung == null)
                return Unauthorized(new { Message = "Ứng viên không tồn tại." });

            int ungVienId = nguoiDung.tkid;

            var danhSach = await _context.UngVien_UngTuyen
                .Where(uu => uu.ungvienID == ungVienId)
                .Join(_context.UngTuyen,
                    uu => uu.ungtuyenID,
                    ut => ut.utid,
                    (uu, ut) => ut)
                .Include(ut => ut.TInTuyenDung)
                    .ThenInclude(t => t!.NhaTuyenDung)
                    .ThenInclude(n => n!.CongTy)
                .Select(ut => new UngTuyenResponseDto
                {
                    UngTuyenId = ut.utid,
                    NgayNop = ut.NgayNop ?? DateTime.Now,
                    TrangThai = ut.TrangThai ?? "Chưa xác định",
                    TieuDeTin = ut.TInTuyenDung!.TieuDe ?? "Không có tiêu đề",
                    TenCongTy = ut.TInTuyenDung!.NhaTuyenDung != null &&
                                ut.TInTuyenDung.NhaTuyenDung.CongTy != null
                        ? ut.TInTuyenDung.NhaTuyenDung.CongTy.ctName
                        : "Không rõ"
                })
                .OrderByDescending(ut => ut.NgayNop)
                .ToListAsync();

            return Ok(danhSach);
        }

        [HttpGet("goi-y-thong-minh")]
        [Authorize(Roles = "UngVien")]
        public async Task<ActionResult<IEnumerable<TinTuyenDungDto>>> GoiYThongMinh()
        {
            var username = User.Identity?.Name;

            var ungVien = await _context.UngVien
                .Include(u => u.HoSoList).ThenInclude(h => h.NoiDungHoSo)
                .FirstOrDefaultAsync(u => u.NguoiDung.tkName == username);

            if (ungVien == null || ungVien.HoSoList == null || !ungVien.HoSoList.Any())
                return Ok(new List<TinTuyenDungDto>());

            // Lấy hồ sơ mới nhất
            var latestHoSo = ungVien.HoSoList.OrderByDescending(h => h.hsid).First();
            if (latestHoSo.NoiDungHoSo == null)
                return Ok(new List<TinTuyenDungDto>());

            var cv = latestHoSo.NoiDungHoSo;
            var cvVector = JobMatchingHelper.ToVector(cv);
            var linhVucId = cv.LinhVucID ?? 0;

            // B1: Lọc tin cùng lĩnh vực (gom cụm theo lĩnh vực)
            var candidateJobs = await _context.TInTuyenDung
                .Where(t => t.linhvucIID == linhVucId
                            && t.TrangThai == "Đã duyệt"
                            && t.HanNop >= DateTime.Today)
                .ToListAsync();

            // Nếu không có tin cùng lĩnh vực → fallback lấy tất cả (không để trống)
            if (!candidateJobs.Any())
            {
                candidateJobs = await _context.TInTuyenDung
                    .Where(t => t.TrangThai == "Đã duyệt" && t.HanNop >= DateTime.Today)
                    .ToListAsync();
            }

            // B2: So sánh trực tiếp → lấy top 5 giống nhất
            var top5 = candidateJobs
                .Select(job => new
                {
                    Job = job,
                    Score = JobMatchingHelper.CosineSimilarity(cvVector, JobMatchingHelper.ToVector(job))
                })
                .OrderByDescending(x => x.Score)
                .Take(5)
                .Select(x => new TinTuyenDungDto
                {
                    TinId = x.Job.ttdid,
                    TieuDe = x.Job.TieuDe,
                    CongTy = x.Job.NhaTuyenDung?.CongTy?.ctName ?? "Chưa cung cấp",
                    ChucDanh = x.Job.ChucDanh?.cdName,
                    NgayDang = x.Job.NgayDang?.ToString("dd/MM/yyyy") ?? "Không xác định",
                    HanNop = x.Job.HanNop?.ToString("dd/MM/yyyy") ?? "Không xác định",
                    PhuHop = Math.Round(x.Score * 100, 1)  // % phù hợp
                })
                .ToList();

            return Ok(top5);
        }
    }
}