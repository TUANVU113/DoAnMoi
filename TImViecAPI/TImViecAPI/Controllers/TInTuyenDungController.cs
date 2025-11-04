using System.ComponentModel.DataAnnotations;
using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using TImViecAPI.Data;


using TImViecAPI.Model;
using TImViecAPI.Model_Function.Dtos;
using static TImViecAPI.Controllers.HoSoController;
using ThongTinCaNhanDto = TImViecAPI.Model_Function.Dtos.ThongTinCaNhanDto;

namespace TImViecAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class TInTuyenDungController : ControllerBase
    {
        private readonly AppDbContext _context;

        public TInTuyenDungController(AppDbContext context)
        {
            _context = context;
        }

        public class TInTuyenDungDto
        {
            [Required(ErrorMessage = "Tiêu đề là bắt buộc.")]
            [StringLength(255)]
            public string? TieuDe { get; set; }

            [StringLength(255)]
            public string? MieuTa { get; set; }

            public bool? DaDuyet { get; set; }

            [StringLength(255)]
            public string? TrangThai { get; set; }

            public int? YeuCau { get; set; }

            public int? Tuoi { get; set; }

            public DateTime? HanNop { get; set; }

            [Required(ErrorMessage = "Loại hình ID là bắt buộc.")]
            public int? loaihinhID { get; set; }

            [Required(ErrorMessage = "Chức danh ID là bắt buộc.")]
            public int? chucdanhID { get; set; }

            [Required(ErrorMessage = "Kinh nghiệm ID là bắt buộc.")]
            public int? kinhnghiemID { get; set; }

            [Required(ErrorMessage = "Bằng cấp ID là bắt buộc.")]
            public int? bangcapID { get; set; }

            [Required(ErrorMessage = "Lĩnh vực ID là bắt buộc.")]
            public int? linhvucIID { get; set; }

            [Required(ErrorMessage = "Vị trí ID là bắt buộc.")]
            public int? vitriID { get; set; }
        }

        [HttpPost("add")]
        [Authorize(Roles = "NhaTuyenDung")] // Chỉ NTD mới tạo tin
        public async Task<IActionResult> Add([FromBody] TInTuyenDungDto dto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            
            string Users = User.Identity.Name;
            if (Users == null)
            {
                return Unauthorized(new { Message = "Người dùng không hợp lệ hoặc không phải NTD." });
            }
            var id_nhaTuyenDung = _context.NguoiDung.FirstOrDefault(ntd => ntd.tkName == Users);


            // Kiểm tra các FK tồn tại
            if (!await _context.LoaiHinhLamViec.AnyAsync(lh => lh.lhid == dto.loaihinhID) ||
                !await _context.ChucDanh.AnyAsync(cd => cd.cdid == dto.chucdanhID) ||
                !await _context.KinhNghiem.AnyAsync(kn => kn.knid == dto.kinhnghiemID) ||
                !await _context.BangCap.AnyAsync(bc => bc.bcid == dto.bangcapID) ||
                !await _context.LinhVuc.AnyAsync(lv => lv.lvid == dto.linhvucIID) ||
                !await _context.ViTri.AnyAsync(vt => vt.vtid == dto.vitriID))
            {
                return BadRequest(new { Message = "Một hoặc nhiều ID tham chiếu không tồn tại." });
            }

            var tinTuyenDung = new TInTuyenDung
            {
                TieuDe = dto.TieuDe,
                MieuTa = dto.MieuTa,
                DaDuyet = false, // Luôn đặt DaDuyet = false khi tạo mới
                TrangThai = dto.TrangThai ?? "Chờ duyệt",
                YeuCau = dto.YeuCau,
                Tuoi = dto.Tuoi,
                NgayDang = DateTime.Now,
                HanNop = dto.HanNop,
                loaihinhID = dto.loaihinhID,
                chucdanhID = dto.chucdanhID,
                kinhnghiemID = dto.kinhnghiemID,
                bangcapID = dto.bangcapID,
                linhvucIID = dto.linhvucIID,
                vitriID = dto.vitriID,
                nhaTuyenDungID = id_nhaTuyenDung.tkid
            };

            _context.TInTuyenDung.Add(tinTuyenDung);
            await _context.SaveChangesAsync();
            // Tạo thông báo cho tất cả tài khoản Admin
            var admins = await _context.NguoiDung
                .Where(u => !_context.UngVien.Any(uv => uv.uvid == u.tkid) &&
                           !_context.NhaTuyenDung.Any(ntd => ntd.ntdid == u.tkid))
                .ToListAsync();
            foreach (var admin in admins)
            {
                var thongBao = new ThongBao
                {
                    NoiDung = $"Tin tuyển dụng mới '{dto.TieuDe}' cần được duyệt.",
                    NgayBao = DateTime.UtcNow
                };
                _context.ThongBao.Add(thongBao);
                await _context.SaveChangesAsync();

                var nguoiDungThongBao = new NguoiDung_ThongBao
                {
                    nguoidungID = admin.tkid,
                    thongbaoID = thongBao.tbid,
                    DaXem = false
                };
                _context.NguoiDung_ThongBao.Add(nguoiDungThongBao);
            }
            await _context.SaveChangesAsync();

            return Ok(new { Message = "Tin tuyển dụng đã được tạo và đang chờ duyệt!", ttdid = tinTuyenDung.ttdid });
            //return Ok(new { Message = "Thêm tin tuyển dụng thành công!", ttdid = tinTuyenDung.ttdid });
        }

        [HttpPut("approve/{id}")]
        [Authorize(Roles = "Admin")] // Chỉ Admin được thực hiện
        public async Task<IActionResult> ApproveTinTuyenDung(int id, [FromBody] TinTuyenDungActionDto dto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var tinTuyenDung = await _context.TInTuyenDung.FindAsync(id);
            if (tinTuyenDung == null)
            {
                return NotFound(new { Message = "Tin tuyển dụng không tồn tại." });
            }

            // Kiểm tra null và ép kiểu DaDuyet
            if (tinTuyenDung.DaDuyet.HasValue && tinTuyenDung.DaDuyet.Value)
            {
                return BadRequest(new { Message = "Tin tuyển dụng đã được duyệt trước đó." });
            }

            // Kiểm tra null và ép kiểu nhaTuyenDungID
            if (!tinTuyenDung.nhaTuyenDungID.HasValue)
            {
                return BadRequest(new { Message = "Tin tuyển dụng không có thông tin nhà tuyển dụng." });
            }

            // Xử lý hành động
            string message;
            if (dto.Action == "approve")
            {
                tinTuyenDung.DaDuyet = true;
                tinTuyenDung.TrangThai = "Đã duyệt";
                message = $"Tin tuyển dụng '{tinTuyenDung.TieuDe}' đã được duyệt.";
            }
            else // dto.Action == "reject"
            {
                tinTuyenDung.DaDuyet = false;
                tinTuyenDung.TrangThai = "Bị từ chối";
                message = $"Tin tuyển dụng '{tinTuyenDung.TieuDe}' đã bị từ chối." +
                          (string.IsNullOrEmpty(dto.Reason) ? "" : $" Lý do: {dto.Reason}");
            }

            await _context.SaveChangesAsync();

            // Gửi thông báo cho nhà tuyển dụng
            var thongBao = new ThongBao
            {
                NoiDung = message,
                NgayBao = DateTime.UtcNow
            };
            _context.ThongBao.Add(thongBao);
            await _context.SaveChangesAsync();

            var nguoiDungThongBao = new NguoiDung_ThongBao
            {
                nguoidungID = tinTuyenDung.nhaTuyenDungID.Value,
                thongbaoID = thongBao.tbid,
                DaXem = false
            };
            _context.NguoiDung_ThongBao.Add(nguoiDungThongBao);
            await _context.SaveChangesAsync();

            return Ok(new { Message = dto.Action == "approve" ? "Duyệt tin tuyển dụng thành công!" : "Từ chối tin tuyển dụng thành công!" });
        }

        [HttpGet("list")]
        
        public async Task<IActionResult> GetAllTInTuyenDung()
        {
            var tins = await _context.TInTuyenDung
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
                    ttd.nhaTuyenDungID
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




        [HttpGet("list-by-ntd")]
        [Authorize(Roles = "NhaTuyenDung")]
        public async Task<IActionResult> GetTinByNhaTuyenDung()
        {
            // Lấy thông tin người dùng từ JWT
            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

            if (string.IsNullOrEmpty(userIdClaim))
                return Unauthorized(new { Message = "Không tìm thấy thông tin người dùng trong token." });

            int tkid = int.Parse(userIdClaim);

            // Tìm NhaTuyenDung tương ứng
            var ntd = await _context.NhaTuyenDung.FirstOrDefaultAsync(x => x.ntdid == tkid);
            if (ntd == null)
                return Unauthorized(new { Message = "Không tìm thấy tài khoản nhà tuyển dụng." });

            // Lọc tin theo ID nhà tuyển dụng
            var tins = await _context.TInTuyenDung
                .Where(ttd => ttd.nhaTuyenDungID == ntd.ntdid)
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
                    ttd.HanNop
                })
                .ToListAsync();

            return Ok(new
            {
                Message = "Lấy danh sách tin của NTD thành công!",
                Data = tins
            });
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetTinTuyenDungById(int id)
        {
            var tin = await _context.TInTuyenDung
                .Include(t => t.LoaiHinhLamViec)
                .Include(t => t.ChucDanh)
                .Include(t => t.KinhNghiem)
                .Include(t => t.BangCap)
                .Include(t => t.LinhVuc)
                .Include(t => t.ViTri)
                .FirstOrDefaultAsync(t => t.ttdid == id);

            if (tin == null)
                return NotFound(new { Message = "Không tìm thấy tin tuyển dụng." });

            return Ok(new
            {
                Message = "Lấy thông tin tin tuyển dụng thành công!",
                Data = new
                {
                    tin.ttdid,
                    tin.TieuDe,
                    tin.MieuTa,
                    tin.DaDuyet,
                    tin.TrangThai,
                    tin.YeuCau,
                    tin.Tuoi,
                    tin.NgayDang,
                    tin.HanNop,
                    tin.loaihinhID,
                    tin.chucdanhID,
                    tin.kinhnghiemID,
                    tin.bangcapID,
                    tin.linhvucIID,
                    tin.vitriID,
                    tin.nhaTuyenDungID
                }
            });
        }

        [HttpPut("update/{id}")]
        [Authorize(Roles = "NhaTuyenDung")]
        public async Task<IActionResult> UpdateTInTuyenDung(int id, [FromBody] TInTuyenDungDto dto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var tin = await _context.TInTuyenDung.FindAsync(id);
            if (tin == null)
            {
                return NotFound(new { Message = "Tin tuyển dụng không tồn tại." });
            }

            // Kiểm tra nhaTuyenDungID từ cookie khớp với tin
            string Users = User.Identity.Name;
            if (User == null)
            {
                return Unauthorized(new { Message = "Bạn không có quyền chỉnh sửa tin này." });
            }
            var nhatuyndung = _context.NhaTuyenDung.FirstOrDefault(ntd => ntd.ntdName == Users); 

            // Kiểm tra các FK tồn tại
            if (!await _context.LoaiHinhLamViec.AnyAsync(lh => lh.lhid == dto.loaihinhID) ||
                !await _context.ChucDanh.AnyAsync(cd => cd.cdid == dto.chucdanhID) ||
                !await _context.KinhNghiem.AnyAsync(kn => kn.knid == dto.kinhnghiemID) ||
                !await _context.BangCap.AnyAsync(bc => bc.bcid == dto.bangcapID) ||
                !await _context.LinhVuc.AnyAsync(lv => lv.lvid == dto.linhvucIID) ||
                !await _context.ViTri.AnyAsync(vt => vt.vtid == dto.vitriID))
            {
                return BadRequest(new { Message = "Một hoặc nhiều ID tham chiếu không tồn tại." });
            }

            tin.TieuDe = dto.TieuDe;
            tin.MieuTa = dto.MieuTa;
            tin.DaDuyet = dto.DaDuyet ?? tin.DaDuyet; // Giữ nguyên nếu null
            tin.TrangThai = dto.TrangThai;
            tin.YeuCau = dto.YeuCau;
            tin.Tuoi = dto.Tuoi;
            tin.HanNop = dto.HanNop;
            tin.loaihinhID = dto.loaihinhID;
            tin.chucdanhID = dto.chucdanhID;
            tin.kinhnghiemID = dto.kinhnghiemID;
            tin.bangcapID = dto.bangcapID;
            tin.linhvucIID = dto.linhvucIID;
            tin.vitriID = dto.vitriID;

            await _context.SaveChangesAsync();
            return Ok(new { Message = "Cập nhật tin tuyển dụng thành công!", ttdid = tin.ttdid });
        }

        [HttpDelete("delete/{id}")]
        [Authorize(Roles = "NhaTuyenDung")]
        public async Task<IActionResult> DeleteTInTuyenDung(int id)
        {
            using var transaction = await _context.Database.BeginTransactionAsync();
            try
            {
                var tin = await _context.TInTuyenDung.FindAsync(id);
                if (tin == null)
                {
                    return NotFound(new { Message = "Tin tuyển dụng không tồn tại." });
                }

                

                _context.TInTuyenDung.Remove(tin);
                await _context.SaveChangesAsync();
                await transaction.CommitAsync();
                return Ok(new { Message = "Xóa tin tuyển dụng thành công!", ttdid = id });
            }
            catch (Exception ex)
            {
                await transaction.RollbackAsync();
                return StatusCode(500, new { Message = "Lỗi khi xóa: " + ex.Message });
            }
        }
        // Trong UngTuyenController.cs


        [HttpGet("danh-sach-co-ban/{tinId}")]
        [Authorize(Roles = "NhaTuyenDung")]
        public async Task<IActionResult> DanhSachUngTuyenCoBan(int tinId)
        {
            // 1. Kiểm tra NTD + tin
            var ntd = await _context.NhaTuyenDung
                .FirstOrDefaultAsync(n => n.ntdName == User.Identity.Name);
            if (ntd == null) return Unauthorized("NTD không tồn tại.");

            var tin = await _context.TInTuyenDung
                .FirstOrDefaultAsync(t => t.ttdid == tinId && t.nhaTuyenDungID == ntd.ntdid);
            if (tin == null) return NotFound($"Tin ID {tinId} không tồn tại hoặc không thuộc về bạn.");

            // 2. LẤY DỮ LIỆU THÔ (chỉ DateTime?)
            var data = await _context.UngTuyen
                .Where(ut => ut.tintuyendungid == tinId)
                .Join(_context.UngVien_UngTuyen,
                      ut => ut.utid,
                      uu => uu.ungtuyenID,
                      (ut, uu) => new { ut.utid, ut.NgayNop, uu.ungvienID, ut.TrangThai })
                .OrderByDescending(x => x.NgayNop) // ← SẮP XẾP THEO DateTime? (EF Core hiểu)
                .ToListAsync(); // RA MEMORY

            // 3. CHUYỂN ĐỔI SANG DTO Ở MEMORY (dùng ?. thoải mái)
            var danhSach = data.Select(x => new UngTuyenCoBanDto
            {
                DonUngTuyenId = x.utid,
                UngVienId = x.ungvienID,
                NgayNop = x.NgayNop?.ToString("dd/MM/yyyy HH:mm") ?? "Không xác định",
                TrangThai = x.TrangThai ?? "Đang chờ duyệt"
            }).ToList();

            if (!danhSach.Any())
                return Ok(new { Message = $"Tin ID {tinId} chưa có ứng viên nào nộp đơn." });

            return Ok(danhSach);
        }

        [HttpGet("chi-tiet-don/{utid}")]
        [Authorize(Roles = "NhaTuyenDung")]
        public async Task<IActionResult> ChiTietDonUngTuyen(int utid)
        {
            // 1. Kiểm tra NTD
            var ntd = await _context.NhaTuyenDung
                .FirstOrDefaultAsync(n => n.ntdName == User.Identity.Name);
            if (ntd == null) return Unauthorized("NTD không tồn tại.");

            // 2. Lấy đơn ứng tuyển + kiểm tra quyền (tin phải thuộc NTD)
            var don = await _context.UngTuyen
                .Where(ut => ut.utid == utid)
                .Join(_context.TInTuyenDung,
                      ut => ut.tintuyendungid,
                      tin => tin.ttdid,
                      (ut, tin) => new { ut, tin.nhaTuyenDungID })
                .FirstOrDefaultAsync();

            if (don == null) return NotFound("Đơn ứng tuyển không tồn tại.");
            if (don.nhaTuyenDungID != ntd.ntdid) return Forbid("Bạn không có quyền xem đơn này.");

            // 3. Lấy dữ liệu chi tiết
            var data = await (
                from ut in _context.UngTuyen.Where(ut => ut.utid == utid)
                join uu in _context.UngVien_UngTuyen on ut.utid equals uu.ungtuyenID
                join uv in _context.UngVien.Include(u => u.ThongTinCaNhan) on uu.ungvienID equals uv.uvid
                join hu in _context.HoSo_UngTuyen on ut.utid equals hu.ungtuyenID
                join h in _context.HoSo on hu.hosoID equals h.hsid
                select new { ut, uv, h }
            ).FirstOrDefaultAsync();

            if (data == null) return NotFound("Không tìm thấy thông tin ứng viên hoặc hồ sơ.");

            // 4. Chuyển sang DTO
            var result = new ChiTietUngVienDto
            {
                DonUngTuyenId = data.ut.utid,
                NgayNop = data.ut.NgayNop?.ToString("dd/MM/yyyy HH:mm") ?? "Không xác định",
                TrangThai = data.ut.TrangThai ?? "Đang chờ duyệt",

                ThongTinCaNhan = new TImViecAPI.Model_Function.Dtos.ThongTinCaNhanDto
                {
                    HoVaTen = data.uv.ThongTinCaNhan?.HoVaTen ?? "Chưa cung cấp",
                    GioiTinh = data.uv.ThongTinCaNhan?.GioiTinh ?? "Chưa cung cấp",
                    NgaySinh = data.uv.ThongTinCaNhan?.NgaySinh?.ToString("dd/MM/yyyy"),
                    SDT = data.uv.ThongTinCaNhan?.SDT ?? "Chưa cung cấp",
                    Email = data.uv.ThongTinCaNhan?.Email ?? "Chưa cung cấp",
                    QuocGia = data.uv.ThongTinCaNhan?.QuocGia ?? "Việt Nam",
                    Tinh = data.uv.ThongTinCaNhan?.Tinh ?? "Chưa cung cấp",
                    Huyen = data.uv.ThongTinCaNhan?.Huyen ?? "Chưa cung cấp",
                    DiaChi = data.uv.ThongTinCaNhan?.DiaChi ?? "Chưa cung cấp",
                    CCCD = data.uv.ThongTinCaNhan?.CCCD ?? "Chưa cung cấp",
                    NoiSinh = data.uv.ThongTinCaNhan?.NoiSinh ?? "Chưa cung cấp"
                },

                HoSo = new TImViecAPI.Model_Function.Dtos.HoSoDto
                {
                    HoSoId = data.h.hsid,
                    HoSoName = data.h.hsName ?? "Chưa đặt tên",
                    FileUrl = data.h.ViTriFile != null
                        ? $"/uploads/cv/{Path.GetFileName(data.h.ViTriFile)}"
                        : null
                }
            };

            return Ok(result);
        }

        [HttpPut("cap-nhat-trang-thai/{utid}")]
        [Authorize(Roles = "NhaTuyenDung")]
        public async Task<IActionResult> CapNhatTrangThai(int utid, [FromBody] CapNhatTrangThaiDto dto)
        {
            // 1. LẤY NTD TỪ User.Identity.Name (ntdName == tkName)
            var ntd = await _context.NhaTuyenDung
                .FirstOrDefaultAsync(n => n.ntdName == User.Identity.Name);

            if (ntd == null)
                return Unauthorized("Nhà tuyển dụng không tồn tại.");

            // 2. LẤY ĐƠN ỨNG TUYỂN + KIỂM TRA QUYỀN (tin phải thuộc NTD)
            var don = await _context.UngTuyen
                .Include(ut => ut.TInTuyenDung)
                .FirstOrDefaultAsync(ut => ut.utid == utid && ut.TInTuyenDung!.nhaTuyenDungID == ntd.ntdid);

            if (don == null)
                return NotFound("Đơn ứng tuyển không tồn tại hoặc không thuộc về bạn.");

            // 3. CẬP NHẬT TRẠNG THÁI
            don.TrangThai = dto.TrangThai;
            await _context.SaveChangesAsync();

            // 4. LẤY ỨNG VIÊN (qua UngVien_UngTuyen)
            var ungVienId = await _context.UngVien_UngTuyen
                .Where(uu => uu.ungtuyenID == utid)
                .Select(uu => uu.ungvienID)
                .FirstOrDefaultAsync();

            if (ungVienId == 0)
                return Ok(new { Message = "Cập nhật trạng thái thành công, nhưng không tìm thấy ứng viên." });

            // 5. TẠO NỘI DUNG THÔNG BÁO
            string tieuDeTin = don.TInTuyenDung?.TieuDe ?? "Tin tuyển dụng";
            string noiDung = dto.TrangThai switch
            {
                "Đã duyệt" => $"Chúc mừng! Đơn ứng tuyển của bạn cho tin **\"{tieuDeTin}\"** đã được **duyệt**.",
                "Từ chối" => $"Rất tiếc, đơn ứng tuyển của bạn cho tin **\"{tieuDeTin}\"** đã bị **từ chối**.",
                "Phỏng vấn" => $"Bạn đã được mời **phỏng vấn** cho tin **\"{tieuDeTin}\"**. Vui lòng kiểm tra email hoặc liên hệ nhà tuyển dụng.",
                _ => $"Trạng thái ứng tuyển của bạn cho tin **\"{tieuDeTin}\"** đã được cập nhật thành: **{dto.TrangThai}**."
            };

            // 6. TẠO THÔNG BÁO
            var thongBao = new ThongBao
            {
                NoiDung = noiDung,
                NgayBao = DateTime.Now.Date
            };
            _context.ThongBao.Add(thongBao);
            await _context.SaveChangesAsync(); // Lưu để lấy tbid

            // 7. LIÊN KẾT VỚI ỨNG VIÊN (dùng uvid = tkid)
            _context.NguoiDung_ThongBao.Add(new NguoiDung_ThongBao
            {
                nguoidungID = ungVienId,
                thongbaoID = thongBao.tbid,
                DaXem = false
            });
            await _context.SaveChangesAsync();

            // 8. TRẢ VỀ KẾT QUẢ
            return Ok(new
            {
                Message = "Cập nhật trạng thái và gửi thông báo thành công!",
                DonUngTuyenId = utid,
                TrangThaiMoi = dto.TrangThai,
                ThongBao = new
                {
                    tbid = thongBao.tbid,
                    NoiDung = thongBao.NoiDung,
                    NgayBao = ((DateTime)thongBao.NgayBao).ToString("dd/MM/yyyy"),
                    DaXem = false
                }
            });
        }

    }
}