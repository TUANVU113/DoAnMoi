using System.ComponentModel.DataAnnotations;

namespace TImViecAPI.Model_Function.Dtos
{
    public class LuuTinYeuThichDto
    {
        [Required]
        public int TinTuyenDungId { get; set; } // ID tin muốn lưu
    }
}
