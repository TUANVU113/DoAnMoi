using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using System.Text;
using TImViecAPI.Data;
using TImViecAPI.Model;
using TImViecAPI.Model_Function.Dtos;
using Microsoft.AspNetCore.Authentication;  // Thêm: Cho SignInAsync
using Microsoft.AspNetCore.Authentication.Cookies;  // Thêm: Cho Cookie scheme
using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;

namespace TImViecAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class RegisterController : ControllerBase
    {
        private readonly AppDbContext _context;
        private readonly IConfiguration _configuration;
        //private object _configuration;
   
        public RegisterController(AppDbContext context, IConfiguration configuration)
        {
            _context = context;
            _configuration = configuration;
        }

        [HttpPost("register")]
        public async Task<IActionResult> Register([FromBody] RegisterDto registerDto)
        {
            // Kiểm tra ModelState (validation từ RegisterDto)
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            // Kiểm tra email đã tồn tại
            if (await _context.NguoiDung.AnyAsync(u => u.mail == registerDto.Mail))
            {
                return BadRequest(new { Message = "Email đã được sử dụng." });
            }

            // Kiểm tra số điện thoại đã tồn tại
            if (await _context.NguoiDung.AnyAsync(u => u.sdt == registerDto.Sdt))
            {
                return BadRequest(new { Message = "Số điện thoại đã được sử dụng." });
            }

            using var transaction = await _context.Database.BeginTransactionAsync();
            try
            {
                // Tạo và lưu NguoiDung
                var nguoiDung = new NguoiDung
                {
                    tkName = registerDto.TkName,
                    sdt = registerDto.Sdt,
                    mail = registerDto.Mail,
                    password = BCrypt.Net.BCrypt.HashPassword(registerDto.Password)
                };
                _context.NguoiDung.Add(nguoiDung);
                await _context.SaveChangesAsync();  // Lưu để lấy tkid

                // Tạo bản ghi trống trong UngVien với uvid = tkid
                var ungVien = new UngVien
                {
                    uvid = nguoiDung.tkid, // Chỉ gán uvid, các trường khác null
                    uvName = registerDto.UvName,
                    NgaySinh = registerDto.NgaySinh,
                    QuocGia = registerDto.QuocGia,
                    linhvucID = registerDto.LinhvucID
                };
                _context.UngVien.Add(ungVien);
                await _context.SaveChangesAsync();

                await transaction.CommitAsync();

                return Ok(new 
                { 
                    Message = "Đăng ký thành công! Hồ sơ ứng viên đã được tạo (có thể bổ sung sau).", 
                    TkId = nguoiDung.tkid,
                    UvId = ungVien.uvid,  // Xác nhận uvid = tkid
                    LinhvucID = ungVien.linhvucID
                });
            }
            catch (Exception ex)
            {
                await transaction.RollbackAsync();
                return StatusCode(500, new { Message = "Lỗi khi lưu dữ liệu: " + ex.Message });
            }
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginDto loginDto )
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);

            var nguoiDung = await _context.NguoiDung
                .FirstOrDefaultAsync(u => u.mail == loginDto.Mail);

            if (nguoiDung == null || !BCrypt.Net.BCrypt.Verify(loginDto.Password, nguoiDung.password))
            {
                return BadRequest(new { Message = "Email hoặc mật khẩu không đúng." });
            }

            //// Tạo JWT token
            
            //var jwtSettings = _configuration.GetSection("Jwt");
            //var key = Encoding.UTF8.GetBytes(jwtSettings["Key"]);
            //var tokenDescriptor = new SecurityTokenDescriptor
            //{
            //    Subject = new ClaimsIdentity(new[]
            //    {
            //        new Claim(ClaimTypes.NameIdentifier, nguoiDung.tkid.ToString()),  // tkid để dùng sau
            //        new Claim(ClaimTypes.Name, nguoiDung.tkName)
            //    }),
            //    Expires = DateTime.UtcNow.AddMinutes(double.Parse(jwtSettings["ExpiresInMinutes"])),
            //    Issuer = jwtSettings["Issuer"],
            //    Audience = jwtSettings["Audience"],
            //    SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature)
            //};
            // Kiểm tra vai trò
            var nhaTuyenDung = await _context.NhaTuyenDung
                .FirstOrDefaultAsync(ntd => ntd.ntdid == nguoiDung.tkid);
            var ungVien = await _context.UngVien
                .FirstOrDefaultAsync(uv => uv.uvid == nguoiDung.tkid);
            var role = nhaTuyenDung != null ? "NhaTuyenDung" : (ungVien != null ? "UngVien" : "Unknown");
            if (role == "Unknown")
            {
                return BadRequest(new { Message = "Tài khoản không thuộc vai trò nào." });
            }

            // Tạo claims dựa trên vai trò
            //var claims = new List<Claim>
            //{
            //    new Claim(ClaimTypes.NameIdentifier, nguoiDung.tkid.ToString()),
            //    new Claim(ClaimTypes.Name, nguoiDung.tkName),
            //    new Claim(ClaimTypes.Role, role)
            //};

           
            var claims = new List<Claim>
            {
                    new Claim(ClaimTypes.NameIdentifier, nguoiDung.tkid.ToString()),
                    new Claim(ClaimTypes.Name, nguoiDung.tkName),
                    new Claim(ClaimTypes.Role, role)
            };

            var jwtSettings = _configuration.GetSection("Jwt");
            var key = Encoding.UTF8.GetBytes(jwtSettings["Key"]);

            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(claims), // 👈 GÁN CLAIMS Ở ĐÂY
                Expires = DateTime.UtcNow.AddMinutes(double.Parse(jwtSettings["ExpiresInMinutes"])),
                Issuer = jwtSettings["Issuer"],
                Audience = jwtSettings["Audience"],
                SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature)
            };

            var tokenHandler = new JwtSecurityTokenHandler();
            var token = tokenHandler.CreateToken(tokenDescriptor);
            var tokenString = tokenHandler.WriteToken(token);


            return Ok(new
            {
                Message = "Đăng nhập thành công!",
                TkId = nguoiDung.tkid,
                TkName = nguoiDung.tkName,
                Role = role,
                Token = tokenString  // Trả JWT cho client
            });
        }
        [HttpPost("register-ntd")]
        public async Task<IActionResult> RegisterNTD([FromBody] RegisterNTDDto registerDto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            // Kiểm tra trùng lặp NguoiDung
            if (await _context.NguoiDung.AnyAsync(u => u.mail == registerDto.Mail))
            {
                return BadRequest(new { Message = "Email đã được sử dụng." });
            }
            if (await _context.NguoiDung.AnyAsync(u => u.sdt == registerDto.Sdt))
            {
                return BadRequest(new { Message = "Số điện thoại đã được sử dụng." });
            }

            // Kiểm tra CtID tồn tại (nếu cung cấp)
            if (registerDto.CtID.HasValue && !await _context.CongTy.AnyAsync(ct => ct.ctid == registerDto.CtID.Value))
            {
                return BadRequest(new { Message = "Công ty không tồn tại." });
            }

            using var transaction = await _context.Database.BeginTransactionAsync();
            try
            {
                // Lưu NguoiDung
                var nguoiDung = new NguoiDung
                {
                    tkName = registerDto.TkName,
                    sdt = registerDto.Sdt,
                    mail = registerDto.Mail,
                    password = BCrypt.Net.BCrypt.HashPassword(registerDto.Password)
                };
                _context.NguoiDung.Add(nguoiDung);
                await _context.SaveChangesAsync();  // Lấy tkid

                // Lưu NhaTuyenDung với ntdid = tkid
                var nhaTuyenDung = new NhaTuyenDung
                {
                    ntdid = nguoiDung.tkid,  // Khớp tkid
                    ntdName = registerDto.NtdName,
                    ctID = registerDto.CtID  // Null nếu không gửi
                };
                _context.NhaTuyenDung.Add(nhaTuyenDung);
                await _context.SaveChangesAsync();

                await transaction.CommitAsync();

                return Ok(new
                {
                    Message = "Đăng ký NTD thành công!",
                    TkId = nguoiDung.tkid,
                    NtdId = nhaTuyenDung.ntdid  // Xác nhận bằng nhau
                });
            }
            catch (Exception ex)
            {
                await transaction.RollbackAsync();
                return StatusCode(500, new { Message = "Lỗi khi lưu dữ liệu: " + ex.Message });
            }
        }

        [HttpGet("whoami")]
        [Authorize(Roles = "NhaTuyenDung")]
        public IActionResult WhoAmI()
        {
            var ntdId = User.FindFirst("NhaTuyenDungID")?.Value;
            var role = User.FindFirst(ClaimTypes.Role)?.Value;
            return Ok(new { NtdId = ntdId, Role = role });
        }

        [HttpGet("whoami-uv")]
        [Authorize(Roles = "UngVien")] // Chỉ cho phép vai trò UngVien
        public IActionResult WhoAmIUV()
        {
            var uvid = User.FindFirst("uvid")?.Value; // Lấy uvid từ claim
            var role = User.FindFirst(ClaimTypes.Role)?.Value; // Lấy role để xác nhận
            return Ok(new { UvId = uvid, Role = role });
        }


        // --- THÊM MỚI: Endpoint logout (tùy chọn, để xóa cookie) ---
        [HttpPost("logout")]
        public async Task<IActionResult> Logout()
        {
            await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
            return Ok(new { Message = "Đăng xuất thành công!" });
        }
    }
}
