using System.ComponentModel.DataAnnotations;

namespace TImViecAPI.Model_Function.Dtos
{
    public class TinTuyenDungActionDto
    {
        [Required(ErrorMessage = "Hành động là bắt buộc.")]
        [RegularExpression("^(approve|reject)$", ErrorMessage = "Hành động phải là 'approve' hoặc 'reject'.")]
        public string Action { get; set; }

        public string? Reason { get; set; } // Lý do từ chối (tùy chọn)
    }
}
