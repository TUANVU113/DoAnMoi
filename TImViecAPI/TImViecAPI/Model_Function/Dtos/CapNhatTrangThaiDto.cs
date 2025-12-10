using System.ComponentModel.DataAnnotations;

namespace TImViecAPI.Model_Function.Dtos
{
    public class CapNhatTrangThaiDto
    {
        [Required]
        public string TrangThai { get; set; } = string.Empty;
        public string? NoiDungEmail { get; set; }
    }
}
