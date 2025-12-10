using System.ComponentModel.DataAnnotations;

namespace TImViecAPI.Model_Function.Dtos
{
    public class XuLyToCaoDto
    {
        [Required] public string HanhDong { get; set; } = null!;
    }
}
