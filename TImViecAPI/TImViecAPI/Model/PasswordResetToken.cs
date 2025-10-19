using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace TImViecAPI.Model
{
    public class PasswordResetToken
    {
        [Key]
        public int Id { get; set; }

        [ForeignKey("NguoiDung")]
        public int TkId { get; set; }

        public string Token { get; set; } = Guid.NewGuid().ToString();
        public DateTime ExpiresAt { get; set; } = DateTime.UtcNow.AddHours(1); // Hết hạn sau 1 giờ
        public bool IsUsed { get; set; } = false;

        public NguoiDung NguoiDung { get; set; } //
    }
}

