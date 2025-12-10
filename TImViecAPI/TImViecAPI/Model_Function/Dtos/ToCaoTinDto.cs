using System.ComponentModel.DataAnnotations;

namespace TImViecAPI.Model_Function.Dtos
{
    public class ToCaoTinDto
    {
        [Required] public string LyDo { get; set; } = null!;
        [Required] public string NoiDung { get; set; } = null!;
    }
}
