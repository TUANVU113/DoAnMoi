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
        [HttpPost]
        public async Task<IActionResult> CreateNoiDungHoSo([FromBody] NoiDungHoSoDto noiDungHoSoDto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            // Kiểm tra xem hosoID có tồn tại trong bảng HoSo không
            var hoSo = await _context.HoSo.FindAsync(noiDungHoSoDto.HosoID);
            if (hoSo == null)
            {
                return NotFound(new { Message = "Hồ sơ không tồn tại." });
            }
            // Tạo đối tượng NoiDungHoSo từ DTO
            var noiDungHoSo = new NoiDungHoSo
            {
                TenUngVien = noiDungHoSoDto.TenUngVien,
                GioiTinh = noiDungHoSoDto.GioiTinh,
                NgaySinh = noiDungHoSoDto.NgaySinh,
                PhoneHoSo = noiDungHoSoDto.PhoneHoSo,
                MailHoSo = noiDungHoSoDto.MailHoSo,
                QuocGia = noiDungHoSoDto.QuocGia,
                Tinh = noiDungHoSoDto.Tinh,
                QuanHuyen = noiDungHoSoDto.QuanHuyen,
                DiaChi = noiDungHoSoDto.DiaChi,

                //noiDungHoSoDto.Tinh = noiDungHoSoDto.Tinh;
                //noiDungHoSoDto.QuanHuyen = noiDungHoSoDto.QuanHuyen;
                //noiDungHoSoDto.DiaChi = noiDungHoSoDto.DiaChi;

                //noiDungHoSo.Tinh = noiDungHoSoDto.Tinh;
                //noiDungHoSo.QuanHuyen = noiDungHoSoDto.QuanHuyen;
                //noiDungHoSo.DiaChi = noiDungHoSoDto.DiaChi;

                HocVan = noiDungHoSoDto.HocVan,
                hosoID = noiDungHoSoDto.HosoID
            };
            // Thêm vào cơ sở dữ liệu
            _context.NoiDungHoSo.Add(noiDungHoSo);
            await _context.SaveChangesAsync();

            return Ok(new { Message = "Thêm nội dung hồ sơ thành công!", NoiDungHoSoId = noiDungHoSo.ndid });
        }
        // GET: api/NoiDungHoSo (Lấy tất cả nội dung hồ sơ)
        [HttpGet]
        public async Task<IActionResult> GetAllNoiDungHoSo()
        {
            var noiDungHoSoList = await _context.NoiDungHoSo.ToListAsync();
            if (noiDungHoSoList == null || !noiDungHoSoList.Any())
            {
                return NotFound(new { Message = "Không có nội dung hồ sơ nào." });
            }

            return Ok(noiDungHoSoList);
        }

        // GET: api/NoiDungHoSo/{id} (Lấy nội dung hồ sơ theo ID)
        [HttpGet("{id}")]
        public async Task<IActionResult> GetNoiDungHoSoById(int id)
        {
            var noiDungHoSo = await _context.NoiDungHoSo.FindAsync(id);
            if (noiDungHoSo == null)
            {
                return NotFound(new { Message = "Nội dung hồ sơ không tồn tại." });
            }

            return Ok(noiDungHoSo);
        }

        // PUT: api/NoiDungHoSo/{id} (Cập nhật nội dung hồ sơ)
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateNoiDungHoSo(int id, [FromBody] NoiDungHoSoDto noiDungHoSoDto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var noiDungHoSo = await _context.NoiDungHoSo.FindAsync(id);
            if (noiDungHoSo == null)
            {
                return NotFound(new { Message = "Nội dung hồ sơ không tồn tại." });
            }

            // Kiểm tra xem hosoID mới có tồn tại không (nếu thay đổi)
            if (noiDungHoSo.hosoID != noiDungHoSoDto.HosoID)
            {
                var hoSo = await _context.HoSo.FindAsync(noiDungHoSoDto.HosoID);
                if (hoSo == null)
                {
                    return NotFound(new { Message = "Hồ sơ mới không tồn tại." });
                }
            }

            // Cập nhật các trường
            noiDungHoSo.TenUngVien = noiDungHoSoDto.TenUngVien;
            noiDungHoSo.GioiTinh = noiDungHoSoDto.GioiTinh;
            noiDungHoSo.NgaySinh = noiDungHoSoDto.NgaySinh;
            noiDungHoSo.PhoneHoSo = noiDungHoSoDto.PhoneHoSo;
            noiDungHoSo.MailHoSo = noiDungHoSoDto.MailHoSo;
            noiDungHoSo.QuocGia = noiDungHoSoDto.QuocGia;
            noiDungHoSoDto.Tinh = noiDungHoSoDto.Tinh;
            noiDungHoSoDto.QuanHuyen = noiDungHoSoDto.QuanHuyen;
            noiDungHoSoDto.DiaChi = noiDungHoSoDto.DiaChi;
            noiDungHoSo.HocVan = noiDungHoSoDto.HocVan;
            noiDungHoSo.hosoID = noiDungHoSoDto.HosoID;

            await _context.SaveChangesAsync();

            return Ok(new { Message = "Cập nhật nội dung hồ sơ thành công!" });
        }

        // DELETE: api/NoiDungHoSo/{id} (Xóa nội dung hồ sơ)
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteNoiDungHoSo(int id)
        {
            var noiDungHoSo = await _context.NoiDungHoSo.FindAsync(id);
            if (noiDungHoSo == null)
            {
                return NotFound(new { Message = "Nội dung hồ sơ không tồn tại." });
            }

            _context.NoiDungHoSo.Remove(noiDungHoSo);
            await _context.SaveChangesAsync();

            return Ok(new { Message = "Xóa nội dung hồ sơ thành công!" });
        }
    }
}

