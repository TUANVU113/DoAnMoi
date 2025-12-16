
﻿using Microsoft.AspNetCore.Http;
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

        [HttpPost("register-Admin")]
        public async Task<IActionResult> RegisterAdmin([FromBody] RegisterAdminDto registerDto)
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



                await transaction.CommitAsync();

                return Ok(new
                {
                    Message = "Đăng ký thành công Admin!.",
                    TkId = nguoiDung.tkid

                });
            }
            catch (Exception ex)
            {
                await transaction.RollbackAsync();
                return StatusCode(500, new { Message = "Lỗi khi lưu dữ liệu: " + ex.Message });
            }
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginDto loginDto)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);

            var nguoiDung = await _context.NguoiDung
                .FirstOrDefaultAsync(u => u.mail == loginDto.Mail);

            if (nguoiDung == null || !BCrypt.Net.BCrypt.Verify(loginDto.Password, nguoiDung.password))
            {
                return BadRequest(new { Message = "Email hoặc mật khẩu không đúng." });
            }


            // Kiểm tra vai trò
            var nhaTuyenDung = await _context.NhaTuyenDung
                .FirstOrDefaultAsync(ntd => ntd.ntdid == nguoiDung.tkid);
            var ungVien = await _context.UngVien
                .FirstOrDefaultAsync(uv => uv.uvid == nguoiDung.tkid);
            var role = nhaTuyenDung != null ? "NhaTuyenDung" : (ungVien != null ? "UngVien" : "Admin");

            int? ctID = null;
            string? ntdName = null;

            if (nhaTuyenDung != null)
            {
                ctID = nhaTuyenDung.ctID;     // ID công ty
                ntdName = nhaTuyenDung.ntdName; // Tên NTD
            }

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

            // 3️⃣ Lưu token vào cookie
            Response.Cookies.Append("jwt_token", tokenString, new CookieOptions
            {
                HttpOnly = true,       // không cho JS truy cập token
                Secure = false,        // ⚠️ dùng HTTP -> để false (khi deploy HTTPS thì true)
                SameSite = SameSiteMode.None,
                Expires = DateTime.UtcNow.AddHours(2)
            });
            return Ok(new
            {
                Message = "Đăng nhập thành công!",
                TkId = nguoiDung.tkid,
                TkName = nguoiDung.tkName,
                Role = role,

                CtID = ctID,
                NtdName = ntdName,
                Token = tokenString

            });
        }
        [HttpPost("dang-ky-ntd")]
        public async Task<IActionResult> DangKyNTD([FromBody] DangKyNTDDto dto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            // Kiểm tra trùng email/sdt
            if (await _context.NguoiDung.AnyAsync(u => u.mail == dto.Mail))
                return BadRequest("Email đã được sử dụng.");
            if (await _context.NguoiDung.AnyAsync(u => u.sdt == dto.Sdt))
                return BadRequest("Số điện thoại đã được sử dụng.");

            using var transaction = await _context.Database.BeginTransactionAsync();
            try
            {
                // 1. Tạo NguoiDung
                var nguoiDung = new NguoiDung
                {
                    tkName = dto.TkName,
                    sdt = dto.Sdt,
                    mail = dto.Mail,
                    password = BCrypt.Net.BCrypt.HashPassword(dto.Password)
                };
                _context.NguoiDung.Add(nguoiDung);
                await _context.SaveChangesAsync();

                // 2. TỰ ĐỘNG TẠO CÔNG TY TRỐNG (chỉ để lấy ID)
                var congTyTrong = new CongTy
                {
                    ctName = "Công ty chưa kê khai", // tên tạm
                                                     // tất cả các trường khác để null hoặc giá trị mặc định
                };
                _context.CongTy.Add(congTyTrong);
                await _context.SaveChangesAsync(); // lấy ctid

                // 3. Tạo NhaTuyenDung gán công ty trống
                var nhaTuyenDung = new NhaTuyenDung
                {
                    ntdid = nguoiDung.tkid,
                    ntdName = dto.NtdName ?? dto.TkName,
                    ctID = congTyTrong.ctid // gán công ty trống
                };
                _context.NhaTuyenDung.Add(nhaTuyenDung);
                await _context.SaveChangesAsync();

                await transaction.CommitAsync();

                return Ok(new
                {
                    Message = "Đăng ký thành công! Bạn có thể kê khai thông tin công ty sau trong phần hồ sơ.",
                    TkId = nguoiDung.tkid,
                    CtId = congTyTrong.ctid // trả về để frontend biết công ty cần cập nhật
                });
            }
            catch (Exception ex)
            {
                await transaction.RollbackAsync();
                return StatusCode(500, "Lỗi khi đăng ký: " + ex.Message);
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



        [HttpPost("google-login")]
        public async Task<IActionResult> GoogleLogin([FromBody] GoogleLoginDto dto)
        {
            try
            {
                var payload = await Google.Apis.Auth.GoogleJsonWebSignature.ValidateAsync(dto.Token);

                // Lấy email Google
                string email = payload.Email;
                string name = payload.Name;

                // 1. Kiểm tra user có tồn tại chưa
                var user = await _context.NguoiDung.FirstOrDefaultAsync(u => u.mail == email);

                if (user == null)
                {
                    // 👉 Chưa có tài khoản → trả về yêu cầu đăng ký bổ sung
                    return Ok(new
                    {
                        RequireRegister = true,
                        Email = email,
                        RealName = name
                    });
                }

                // 2. Nếu user tồn tại → xác định role
                var ntd = await _context.NhaTuyenDung.FirstOrDefaultAsync(x => x.ntdid == user.tkid);
                var uv = await _context.UngVien.FirstOrDefaultAsync(x => x.uvid == user.tkid);
                var role = ntd != null ? "NhaTuyenDung" : (uv != null ? "UngVien" : "Admin");

                // 3. Tạo token giống login thường
                var claims = new List<Claim>
        {
            new Claim(ClaimTypes.NameIdentifier, user.tkid.ToString()),
            new Claim(ClaimTypes.Name, user.tkName),
            new Claim(ClaimTypes.Role, role)
        };

                var jwtSettings = _configuration.GetSection("Jwt");
                var key = Encoding.UTF8.GetBytes(jwtSettings["Key"]);

                var tokenDescriptor = new SecurityTokenDescriptor
                {
                    Subject = new ClaimsIdentity(claims),
                    Expires = DateTime.UtcNow.AddHours(2),
                    Issuer = jwtSettings["Issuer"],
                    Audience = jwtSettings["Audience"],
                    SigningCredentials = new SigningCredentials(
                        new SymmetricSecurityKey(key),
                        SecurityAlgorithms.HmacSha256Signature)
                };

                var handler = new JwtSecurityTokenHandler();
                var token = handler.WriteToken(handler.CreateToken(tokenDescriptor));

                return Ok(new
                {
                    Message = "Đăng nhập Google thành công!",
                    Token = token,
                    Role = role,
                    TkId = user.tkid,
                    TkName = user.tkName
                });
            }
            catch
            {
                return BadRequest(new { Message = "Google token không hợp lệ." });
            }
        }

    }
}

