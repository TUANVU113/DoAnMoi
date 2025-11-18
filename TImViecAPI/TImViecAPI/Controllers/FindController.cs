using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Linq;
using System.Threading.Tasks;
using TImViecAPI.Data;
using TImViecAPI.Model;
using TImViecAPI.Model_Function.Dtos;

namespace TImViecAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class FindController : ControllerBase
    {
        private readonly AppDbContext _context;

        public FindController(AppDbContext context)
        {
            _context = context;
        }

        [HttpPost("search")]
        public async Task<IActionResult> Search([FromBody] TinTuyenDungSearchDto dto)
        {
            // Chuẩn hóa phân trang
            dto.Page = dto.Page < 1 ? 1 : dto.Page;
            dto.PageSize = dto.PageSize < 1 ? 10 : dto.PageSize > 50 ? 50 : dto.PageSize;

            var query = _context.TInTuyenDung
                .Include(t => t.NhaTuyenDung).ThenInclude(n => n!.CongTy)
                .Include(t => t.LoaiHinhLamViec)
                .Include(t => t.ChucDanh)
                .Include(t => t.KinhNghiem)
                .Include(t => t.BangCap)
                .Include(t => t.LinhVuc)
                .Include(t => t.ViTri)
                .Where(t => t.DaDuyet == true)
                .AsQueryable();

            // 🔍 1. Từ khóa
            if (!string.IsNullOrWhiteSpace(dto.Keyword))
            {
                var kw = dto.Keyword.Trim().ToLower();
                query = query.Where(t =>
                    t.TieuDe!.ToLower().Contains(kw) ||
                    t.MieuTa!.ToLower().Contains(kw) ||
                    (t.NhaTuyenDung != null &&
                     t.NhaTuyenDung.CongTy != null &&
                     t.NhaTuyenDung.CongTy.ctName.ToLower().Contains(kw))
                );
            }

            // 🎯 2. Lọc
            if (dto.LoaiHinhId.HasValue)
                query = query.Where(t => t.loaihinhID == dto.LoaiHinhId);
            if (dto.ChucDanhId.HasValue)
                query = query.Where(t => t.chucdanhID == dto.ChucDanhId);
            if (dto.KinhNghiemId.HasValue)
                query = query.Where(t => t.kinhnghiemID == dto.KinhNghiemId);
            if (dto.BangCapId.HasValue)
                query = query.Where(t => t.bangcapID == dto.BangCapId);
            if (dto.LinhVucId.HasValue)
                query = query.Where(t => t.linhvucIID == dto.LinhVucId);
            if (dto.ViTriId.HasValue)
                query = query.Where(t => t.vitriID == dto.ViTriId);
            if (dto.NhaTuyenDungId.HasValue)
                query = query.Where(t => t.nhaTuyenDungID == dto.NhaTuyenDungId);
            if (dto.CongTyId.HasValue)
                query = query.Where(t => t.NhaTuyenDung != null &&
                                         t.NhaTuyenDung.CongTy != null &&
                                         t.NhaTuyenDung.CongTy.ctid == dto.CongTyId);

            // 📅 3. Ngày đăng
            if (dto.FromDate.HasValue)
                query = query.Where(t => t.NgayDang >= dto.FromDate.Value.Date);
            if (dto.ToDate.HasValue)
                query = query.Where(t => t.NgayDang <= dto.ToDate.Value.Date.AddDays(1).AddTicks(-1));

            // 🔢 4. Sắp xếp
            query = dto.SortBy?.ToLower() switch
            {
                "tencongty" => dto.SortOrder == "asc"
                    ? query.OrderBy(t => t.NhaTuyenDung!.CongTy!.ctName)
                    : query.OrderByDescending(t => t.NhaTuyenDung!.CongTy!.ctName),

                "tieude" => dto.SortOrder == "asc"
                    ? query.OrderBy(t => t.TieuDe)
                    : query.OrderByDescending(t => t.TieuDe),

                _ => dto.SortOrder == "asc"
                    ? query.OrderBy(t => t.NgayDang)
                    : query.OrderByDescending(t => t.NgayDang)
            };

            // 📄 5. Phân trang
            var total = await query.CountAsync();
            var data = await query
                .Skip((dto.Page - 1) * dto.PageSize)
                .Take(dto.PageSize)
                .Select(t => new
                {
                    Id = t.ttdid,
                    TieuDe = t.TieuDe ?? "Không có tiêu đề",
                    MieuTa = t.MieuTa != null && t.MieuTa.Length > 200 ? t.MieuTa.Substring(0, 200) + "..." : t.MieuTa ?? "",
                    TrangThai = t.TrangThai ?? "Đang tuyển",
                    NgayDang = t.NgayDang,
                    HanNop = t.HanNop,
                    TenCongTy = t.NhaTuyenDung != null && t.NhaTuyenDung.CongTy != null
                        ? t.NhaTuyenDung.CongTy.ctName : "Không rõ",
                    Logo = t.NhaTuyenDung != null && t.NhaTuyenDung.CongTy != null
                        ? t.NhaTuyenDung.CongTy.Logo : null,
                    LoaiHinh = t.LoaiHinhLamViec != null ? t.LoaiHinhLamViec.lhName : "Không xác định",
                    ChucDanh = t.ChucDanh != null ? t.ChucDanh.cdName : "Không xác định",
                    KinhNghiem = t.KinhNghiem != null ? t.KinhNghiem.knName : "Không yêu cầu",
                    BangCap = t.BangCap != null ? t.BangCap.bcName : "Không yêu cầu",
                    LinhVuc = t.LinhVuc != null ? t.LinhVuc.lvName : "Khác",
                    ViTri = t.ViTri != null ? t.ViTri.vtName : "Không xác định",
                    SoLuongUngTuyen = _context.UngTuyen.Count(u => u.tintuyendungid == t.ttdid)
                })
                .ToListAsync();

            return Ok(new
            {
                Total = total,
                Page = dto.Page,
                PageSize = dto.PageSize,
                TotalPages = (int)Math.Ceiling(total / (double)dto.PageSize),
                Data = data
            });
        }
    }
}
