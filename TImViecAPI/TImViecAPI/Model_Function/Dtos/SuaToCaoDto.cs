using System.ComponentModel.DataAnnotations;

namespace TImViecAPI.Model_Function.Dtos
{
    public class SuaToCaoDto
    {
        [Required] public string LyDo { get; set; } = null!;
        [Required] public string NoiDung { get; set; } = null!;

        // Chỉ Admin mới được sửa trạng thái (tùy chọn)
        public string? TrangThai { get; set; }
    }
}
