// DTOs/HoSoCreateDto.cs
using System.ComponentModel.DataAnnotations;

namespace TImViecAPI.Model_Function.Dtos
{
    public class HoSoCreateDto
    {
        [Required(ErrorMessage = "Tên hồ sơ là bắt buộc.")]
        [StringLength(255)]
        public string? hsName { get; set; }
    }
}