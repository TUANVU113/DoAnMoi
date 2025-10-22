using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.ComponentModel.DataAnnotations;
using System.Threading.Tasks;
using TImViecAPI.Data;
using TImViecAPI.Model;

namespace TImViecAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ResetPasswordController : ControllerBase
    {
        private readonly AppDbContext _context;
        private readonly IEmailService _emailService;

        public ResetPasswordController(AppDbContext context, IEmailService emailService)
        {
            _context = context;
            _emailService = emailService;
        }

        [HttpPost("request")]
        public async Task<IActionResult> RequestResetPassword([FromBody] EmailDto emailDto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var nguoiDung = await _context.NguoiDung
                .FirstOrDefaultAsync(u => u.mail == emailDto.Email);
            if (nguoiDung == null)
            {
                return NotFound(new { Message = "Email không tồn tại trong hệ thống." });
            }

            // Tạo token reset
            var token = new PasswordResetToken
            {
                TkId = nguoiDung.tkid,
                Token = Guid.NewGuid().ToString(),
                ExpiresAt = DateTime.UtcNow.AddHours(1)
            };

            _context.PasswordResetTokens.Add(token);
            await _context.SaveChangesAsync();

            // Tạo link reset (thay localhost:5000 bằng domain thật khi deploy)
            var resetLink = $"http://localhost:5173/resetpassword/confirm?token={token.Token}";

            // Gửi email thật
            await _emailService.SendEmailAsync(
                nguoiDung.mail,
                "Đặt lại mật khẩu",
                $"<h2>Đặt lại mật khẩu</h2><p>Nhấn vào link để đặt lại mật khẩu: <a href='{resetLink}'>{resetLink}</a></p><p>Link hết hạn sau 1 giờ.</p>"
            );

            return Ok(new { Message = "Link đặt lại mật khẩu đã được gửi đến email của bạn." });
        }

        [HttpPost("confirm")]
        public async Task<IActionResult> ConfirmResetPassword([FromBody] ResetPasswordDto resetDto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var tokenRecord = await _context.PasswordResetTokens
                .FirstOrDefaultAsync(t => t.Token == resetDto.Token && !t.IsUsed && t.ExpiresAt > DateTime.UtcNow);
            if (tokenRecord == null)
            {
                return BadRequest(new { Message = "Token không hợp lệ hoặc đã hết hạn." });
            }

            var nguoiDung = await _context.NguoiDung.FindAsync(tokenRecord.TkId);
            if (nguoiDung == null)
            {
                return NotFound(new { Message = "Người dùng không tồn tại." });
            }

            // Cập nhật mật khẩu mới
            nguoiDung.password = BCrypt.Net.BCrypt.HashPassword(resetDto.NewPassword);
            tokenRecord.IsUsed = true;

            await _context.SaveChangesAsync();

            return Ok(new { Message = "Mật khẩu đã được đặt lại thành công!" });
        }
    }

    // DTOs (giữ nguyên)
    public class EmailDto
    {
        [Required(ErrorMessage = "Email là bắt buộc.")]
        [EmailAddress(ErrorMessage = "Định dạng email không hợp lệ.")]
        public string? Email { get; set; }
    }

    public class ResetPasswordDto
    {
        [Required(ErrorMessage = "Token là bắt buộc.")]
        public string? Token { get; set; }

        [Required(ErrorMessage = "Mật khẩu mới là bắt buộc.")]
        [StringLength(255, MinimumLength = 6, ErrorMessage = "Mật khẩu phải từ 6 đến 255 ký tự.")]
        public string? NewPassword { get; set; }
    }
}