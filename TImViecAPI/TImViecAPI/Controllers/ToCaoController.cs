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
    public class ToCaoController : ControllerBase
    {
        private readonly AppDbContext _context;
        private readonly IEmailService _emailService;
        public ToCaoController(AppDbContext context, IEmailService emailService) 
        {

            _context = context;
            _emailService = emailService;
        }

        [HttpPost("to-cao-tin/{ttdid}")]
        [Authorize(Roles = "UngVien")]
        public async Task<IActionResult> ToCaoTin(int ttdid, [FromBody] ToCaoTinDto dto)
        {
            var username = User.Identity?.Name;
            var ungVien = await _context.UngVien
                .FirstOrDefaultAsync(u => u.NguoiDung.tkName == username);
            if (ungVien == null) return Unauthorized();

            var tin = await _context.TInTuyenDung.AnyAsync(t => t.ttdid == ttdid);
            if (!tin) return NotFound("Tin không tồn tại");

            var daToCao = await _context.ToCaoTin
                .AnyAsync(t => t.ungvienID == ungVien.uvid && t.ttdid == ttdid);
            if (daToCao) return BadRequest("Bạn đã tố cáo tin này rồi");

            var toCao = new ToCaoTin
            {
                ungvienID = ungVien.uvid,
                ttdid = ttdid,
                LyDo = dto.LyDo,
                NoiDung = dto.NoiDung
            };

            _context.ToCaoTin.Add(toCao);
            await _context.SaveChangesAsync();

            return Ok(new { message = "Tố cáo thành công! Admin sẽ xem xét sớm." });
        }
        // GET: api/hoso/to-cao-cua-toi
        // Chỉ ứng viên mới được xem danh sách tố cáo do chính mình gửi
        [HttpGet("to-cao-cua-toi")]
        [Authorize(Roles = "UngVien")]
        public async Task<IActionResult> GetMyToCao()
        {
            var username = User.Identity?.Name;
            var ungVien = await _context.UngVien
                .FirstOrDefaultAsync(u => u.NguoiDung.tkName == username);

            if (ungVien == null) return Unauthorized();

            var list = await _context.ToCaoTin
                .Where(t => t.ungvienID == ungVien.uvid)
                .Include(t => t.TinTuyenDung!)
                    .ThenInclude(tin => tin.NhaTuyenDung!.CongTy)
                .OrderByDescending(t => t.NgayToCao)
                .Select(t => new
                {
                    t.Id,
                    TinTuyenDung = new
                    {
                        t.TinTuyenDung.ttdid,
                        t.TinTuyenDung.TieuDe,
                        t.TinTuyenDung.NhaTuyenDung.CongTy.ctName
                    },
                    t.LyDo,
                    t.NoiDung,
                    t.TrangThai, // Chờ xử lý / Đã duyệt / Từ chối
                    NgayToCao = t.NgayToCao.ToString("dd/MM/yyyy HH:mm")
                })
                .ToListAsync();

            if (!list.Any())
            {
                return Ok(new
                {
                    Message = "Bạn chưa tố cáo tin tuyển dụng nào.",
                    Data = new List<object>()
                });
            }

            return Ok(new
            {
                Message = "Lấy lịch sử tố cáo thành công!",
                Total = list.Count,
                Data = list
            });
        }

        // GET: api/admin/to-cao-tin
        [HttpGet]
        //[Authorize(Roles = "Admin")]
        public async Task<ActionResult<IEnumerable<object>>> GetAll()
        {
            var list = await _context.ToCaoTin
                .Include(t => t.UngVien)
                .Include(t => t.TinTuyenDung)
                .ThenInclude(tin => tin.NhaTuyenDung)
                .OrderByDescending(t => t.NgayToCao)
                .Select(t => new
                {
                    t.Id,
                    TinTuyenDung = new
                    {
                        t.TinTuyenDung.ttdid,
                        t.TinTuyenDung.TieuDe,
                        t.TinTuyenDung.NhaTuyenDung.CongTy.ctName
                        
                    },
                    UngVien = new
                    {
                        t.UngVien.uvid,
                        t.UngVien.NguoiDung.mail,
                        t.UngVien.ThongTinCaNhan.HoVaTen

                    },
                    t.LyDo,
                    t.NoiDung,
                    t.TrangThai,
                    NgayToCao = t.NgayToCao.ToString("dd/MM/yyyy HH:mm")
                })
                .ToListAsync();

            return Ok(list);
        }

        // 2. GET BY ID – CHI TIẾT 1 TỐ CÁO
        [HttpGet("{id}")]
        public async Task<ActionResult> GetById(int id)
        {
            var toCao = await _context.ToCaoTin
                .Include(t => t.UngVien).ThenInclude(u => u.ThongTinCaNhan)
                .Include(t => t.TinTuyenDung).ThenInclude(tin => tin.NhaTuyenDung)
                .FirstOrDefaultAsync(t => t.Id == id);

            if (toCao == null) return NotFound();

            return Ok(new
            {
                toCao.Id,
                TinTuyenDung = new
                {
                    toCao.TinTuyenDung.ttdid,
                    toCao.TinTuyenDung.TieuDe,
                    CongTy = toCao.TinTuyenDung.NhaTuyenDung?.CongTy?.ctName
                },
                UngVien = new
                {
                    toCao.UngVien.uvid,
                    HoTen = toCao.UngVien.ThongTinCaNhan?.HoVaTen,
                    Email = toCao.UngVien.ThongTinCaNhan?.Email
                },
                toCao.LyDo,
                toCao.NoiDung,
                toCao.TrangThai,
                NgayToCao = toCao.NgayToCao.ToString("dd/MM/yyyy HH:mm")
            });
        }

        // PUT: api/admin/to-cao-tin/sua/{id}
        // Cho phép: Admin hoặc chính ứng viên tố cáo được sửa nội dung + lý do
        [HttpPut("sua/{id}")]
        //[Authorize(Roles = "Admin,UngVien")]
        public async Task<IActionResult> SuaToCao(int id, [FromBody] SuaToCaoDto dto)
        {
            var toCao = await _context.ToCaoTin
                .Include(t => t.UngVien)
                .Include(t => t.TinTuyenDung)
                .FirstOrDefaultAsync(t => t.Id == id);

            if (toCao == null)
                return NotFound("Không tìm thấy tố cáo");

            // LẤY USER HIỆN TẠI
            var username = User.Identity?.Name;
            var currentUser = await _context.NguoiDung
                .FirstOrDefaultAsync(u => u.tkName == username);

            if (currentUser == null)
                return Unauthorized();

            // KIỂM TRA QUYỀN: Chỉ Admin hoặc chính ứng viên tố cáo mới được sửa
            bool isAdmin = User.IsInRole("Admin");
            bool isOwner = toCao.ungvienID == currentUser.tkid;

            if (!isAdmin && !isOwner)
                return Forbid("Bạn không có quyền sửa tố cáo này");

            // CẬP NHẬT NỘI DUNG
            toCao.LyDo = dto.LyDo;
            toCao.NoiDung = dto.NoiDung;

            // Nếu là Admin thì có thể đổi trạng thái luôn (tùy chọn)
            if (isAdmin && !string.IsNullOrEmpty(dto.TrangThai))
            {
                if (new[] { "Chờ xử lý", "Đã xem", "Từ chối" }.Contains(dto.TrangThai))
                    toCao.TrangThai = dto.TrangThai;    
            }

            await _context.SaveChangesAsync();

            return Ok(new
            {
                message = "Cập nhật tố cáo thành công",
                data = new
                {
                    toCao.Id,
                    toCao.LyDo,
                    toCao.NoiDung,
                    toCao.TrangThai,
                    NgayToCao = toCao.NgayToCao.ToString("dd/MM/yyyy HH:mm")
                }
            });
        }

        // 3. SỬA TRẠNG THÁI (Duyệt / Từ chối)
        [HttpPut("cap-nhat-trang-thai/{id}")]
        public async Task<IActionResult> CapNhatTrangThai(int id, [FromBody] CapNhatTrangThaiToCaoDto dto)
        {
            var toCao = await _context.ToCaoTin.FindAsync(id);
            if (toCao == null) return NotFound();

            if (!new[] { "Đã duyệt", "Từ chối" }.Contains(dto.TrangThai))
                return BadRequest("Trạng thái chỉ được là: Đã duyệt hoặc Từ chối");

            toCao.TrangThai = dto.TrangThai;
            await _context.SaveChangesAsync();

            return Ok(new { message = "Cập nhật trạng thái thành công", trangThai = toCao.TrangThai });
        }

        // DTO cho sửa trạng thái
        public class CapNhatTrangThaiToCaoDto
        {
            public string TrangThai { get; set; } = null!;
        }

        // 4. XÓA TỐ CÁO
        [HttpDelete("{id}")]
        public async Task<IActionResult> Xoa(int id)
        {
            var toCao = await _context.ToCaoTin.FindAsync(id);
            if (toCao == null) return NotFound();

            _context.ToCaoTin.Remove(toCao);
            await _context.SaveChangesAsync();

            return Ok(new { message = "Xóa tố cáo thành công" });
        }

        // POST: api/admin/to-cao-tin/xu-ly/{id}
        // Chỉ Admin được xử lý
        [HttpPost("xu-ly/{id}")]
        //[Authorize(Roles = "Admin")]
        public async Task<IActionResult> XuLyToCao(int id, [FromBody] XuLyToCaoDto dto)
        {
            var toCao = await _context.ToCaoTin
                .Include(t => t.UngVien)
                .Include(t => t.TinTuyenDung!)
                    .ThenInclude(tin => tin.NhaTuyenDung!)
                        .ThenInclude(ntd => ntd.NguoiDung)
                .FirstOrDefaultAsync(t => t.Id == id);

            if (toCao == null) return NotFound("Tố cáo không tồn tại");

            // Cập nhật trạng thái tố cáo
            toCao.TrangThai = dto.HanhDong; // "Đã duyệt" hoặc "Từ chối"

            // NẾU DUYỆT → KHÓA TIN + GỬI MAIL CẢNH CÁO
            if (dto.HanhDong == "Đã khóa")
            {
                // 1. Khóa tin tuyển dụng
                toCao.TinTuyenDung.TrangThai = "Đã khóa";

                // 2. Gửi mail cảnh cáo cho NTD
                var emailNTD = toCao.TinTuyenDung.NhaTuyenDung?.NguoiDung?.mail;
                if (!string.IsNullOrEmpty(emailNTD))
                {
                    string subject = $"CẢNH CÁO: Tin tuyển dụng bị khóa do vi phạm";
                    string body = $@"
                <h2>Xin chào {toCao.TinTuyenDung.NhaTuyenDung?.CongTy?.ctName ?? "Quý nhà tuyển dụng"},</h2>
                <p>Tin tuyển dụng <strong>'{toCao.TinTuyenDung.TieuDe}'</strong> đã bị <strong>khóa vĩnh viễn</strong> do bị ứng viên tố cáo:</p>
                <ul>
                    <li><strong>Lý do:</strong> {toCao.LyDo}</li>
                    <li><strong>Nội dung tố cáo:</strong> {toCao.NoiDung}</li>
                </ul>
                <p>Vui lòng tuân thủ quy định của nền tảng. Nếu tái phạm, tài khoản sẽ bị khóa.</p>
                <p>Trân trọng,<br/>Ban Quản Trị Hệ Thống Tìm Việc Làm</p>";

                    try
                    {
                        await _emailService.SendEmailAsync(emailNTD, subject, body);
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine($"[EMAIL] Gửi cảnh cáo thất bại: {ex.Message}");
                    }
                }

                // 3. Gửi thông báo trong hệ thống cho NTD
                var thongBao = new ThongBao
                {
                    NoiDung = $"Tin tuyển dụng '{toCao.TinTuyenDung.TieuDe}' đã bị khóa do vi phạm quy định nền tảng.",
                    NgayBao = DateTime.Now.Date
                };
                _context.ThongBao.Add(thongBao);
                await _context.SaveChangesAsync();

                _context.NguoiDung_ThongBao.Add(new NguoiDung_ThongBao
                {
                    nguoidungID = toCao.TinTuyenDung.NhaTuyenDung!.ntdid,
                    thongbaoID = thongBao.tbid,
                    DaXem = false
                });
            }

            await _context.SaveChangesAsync();

            return Ok(new
            {
                message = dto.HanhDong == "Đã duyệt"
                    ? "Đã khóa tin và gửi cảnh cáo thành công!"
                    : "Đã từ chối tố cáo",
                toCaoId = toCao.Id,
                tinTuyenDungId = toCao.ttdid,
                trangThaiTin = toCao.TinTuyenDung.TrangThai
            });
        }

        // GET: api/tintuyendung/da-khoa
        // Chỉ Admin mới được xem danh sách tin bị khóa
        [HttpGet("da-khoa")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> GetTinDaKhoa()
        {
            var tins = await _context.TInTuyenDung
                .Include(ttd => ttd.NhaTuyenDung)
                    .ThenInclude(ntd => ntd.CongTy)
                .Where(ttd => ttd.TrangThai == "Đã khóa")        // ← CHỈ LẤY TIN ĐÃ KHÓA
                .Select(ttd => new
                {
                    ttd.ttdid,
                    ttd.TieuDe,
                    ttd.MieuTa,
                    ttd.DaDuyet,
                    ttd.TrangThai,
                    ttd.YeuCau,
                    ttd.Tuoi,
                    ttd.NgayDang,
                    ttd.HanNop,
                    ttd.loaihinhID,
                    ttd.chucdanhID,
                    ttd.kinhnghiemID,
                    ttd.bangcapID,
                    ttd.linhvucIID,
                    ttd.vitriID,
                    ttd.NhaTuyenDung.CongTy.ctName,
                    ttd.NhaTuyenDung.ntdName,
                    Logo = ttd.NhaTuyenDung.CongTy.Logo
                })
                .ToListAsync();

            if (!tins.Any())
            {
                return Ok(new { Message = "Không có tin tuyển dụng nào trong hệ thống.", Data = new List<object>() });
            }

            return Ok(new
            {
                Message = "Lấy danh sách tin tuyển dụng thành công!",
                Data = tins
            });
        }


    }
}
